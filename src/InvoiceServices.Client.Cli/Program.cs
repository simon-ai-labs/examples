using System.CommandLine;
using InvoiceServices.Client;
using InvoiceServices.Client.Cli;
using InvoiceServices.Client.Mcp;
using InvoiceServices.Client.OpenApi;
using InvoiceServices.Client.OpenApi.Generated.Models;
using Microsoft.Kiota.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

// Configuration: appsettings.json, user secrets (Development), then environment variables such as
// InvoiceServices__ApiKey. The key is never accepted on the command line, where it would end up in shell history.
var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = [],
    ContentRootPath = AppContext.BaseDirectory,
    EnvironmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? Environments.Development,
});
builder.Services.AddInvoiceServicesClient(builder.Configuration);
builder.Services.AddSingleton(new InvoiceScenario(Console.Out));
var host = builder.Build();
await using var hostLifetime = (IAsyncDisposable)host;
try
{
    await host.StartAsync();
}
catch (Exception exception) when (ValidationFailures(exception) is { Count: > 0 } failures)
{
    foreach (var failure in failures) Console.Error.WriteLine($"Configuration error: {failure}");
    return 1;
}

var file = new Option<FileInfo>("--file", "-f")
{
    Description = "Invoice XML to send. Defaults to the official KoSIT XRechnung UBL test invoice.",
    DefaultValueFactory = _ => new FileInfo(Path.Combine(AppContext.BaseDirectory, "TestInvoices", "01.06_minimal_test_ubl.xml")),
};
var output = new Option<DirectoryInfo>("--out", "-o")
{
    Description = "Directory for converted and rendered documents.",
    DefaultValueFactory = _ => new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "output")),
};

var docs = new Command("docs", "Read /docs, list the documented operations and check the generated client for drift.");
docs.SetAction(async (_, ct) =>
{
    var description = await host.Services.GetRequiredService<InvoiceApiDocsReader>().ReadAsync(ct);
    Console.WriteLine($"{description.Title} {description.Version}");
    Console.WriteLine($"  docs: {description.DocsUrl}");
    Console.WriteLine($"  spec: {description.SpecificationUrl}");
    Console.WriteLine($"  security: {string.Join(", ", description.SecuritySchemes)}");
    foreach (var operation in description.Operations)
        Console.WriteLine($"  {operation.Method,-6} {operation.Path,-24} {string.Join(' ', operation.ResponseCodes)}");
    if (description.MatchesGeneratedClient)
    {
        Console.WriteLine("Generated client matches the live API.");
        return 0;
    }
    Console.WriteLine("Generated client is out of date (run Build/generate-openapi-client.ps1):");
    foreach (var change in description.Drift) Console.WriteLine($"  {change}");
    return 2;
});

Command Transport(string name, string description)
{
    var command = new Command(name, description) { file, output };
    command.SetAction(async (parse, ct) =>
    {
        var operations = host.Services.GetRequiredKeyedService<IInvoiceOperations>(name);
        await host.Services.GetRequiredService<InvoiceScenario>()
            .RunAsync(operations, parse.GetRequiredValue(file), parse.GetRequiredValue(output), ct);
        return 0;
    });
    return command;
}

var mcpTools = new Command("tools", "List the tools offered by /mcp with their input schemas.");
mcpTools.SetAction(async (_, ct) =>
{
    foreach (var tool in await host.Services.GetRequiredService<IInvoiceMcpClient>().ListToolsAsync(ct))
        Console.WriteLine($"{tool.Name}: {tool.Description}\n  input: {tool.JsonSchema.GetRawText()}");
    return 0;
});
var mcp = Transport(InvoiceTransports.Mcp, "Run validate, read, convert and render through /mcp.");
mcp.Subcommands.Add(mcpTools);

var smoke = new Command("smoke", "End-to-end check: docs drift, then the same scenario over REST and MCP with identical results.") { file, output };
smoke.SetAction(async (parse, ct) =>
{
    var description = await host.Services.GetRequiredService<InvoiceApiDocsReader>().ReadAsync(ct);
    Console.WriteLine(description.MatchesGeneratedClient
        ? "docs: generated client matches the live API"
        : $"docs: DRIFT {string.Join("; ", description.Drift)}");
    var scenario = host.Services.GetRequiredService<InvoiceScenario>();
    var rest = await scenario.RunAsync(host.Services.GetRequiredKeyedService<IInvoiceOperations>(InvoiceTransports.Rest),
        parse.GetRequiredValue(file), parse.GetRequiredValue(output), ct);
    var viaMcp = await scenario.RunAsync(host.Services.GetRequiredKeyedService<IInvoiceOperations>(InvoiceTransports.Mcp),
        parse.GetRequiredValue(file), parse.GetRequiredValue(output), ct);
    var differences = rest.Differences(viaMcp);
    foreach (var difference in differences) Console.WriteLine($"REST and MCP differ: {difference}");
    var ok = description.MatchesGeneratedClient && differences.Count == 0;
    Console.WriteLine(ok ? "SMOKE PASSED" : "SMOKE FAILED");
    return ok ? 0 : 1;
});

var root = new RootCommand("Client for the E-Invoicing Service: typed REST client generated from /docs and an MCP client for /mcp.")
{
    docs,
    Transport(InvoiceTransports.Rest, "Run validate, read, convert and render through the REST API."),
    mcp,
    smoke,
};
int exitCode;
try
{
    exitCode = await root.Parse(args).InvokeAsync(new InvocationConfiguration { EnableDefaultExceptionHandler = false });
}
catch (Exception exception) when (Describe(exception) is { } message)
{
    Console.Error.WriteLine(message);
    exitCode = 1;
}
await host.StopAsync();
return exitCode;

// Service-side failures are expected outcomes for a client; print them without a stack trace.
static string? Describe(Exception exception) => exception switch
{
    ProblemDetails problem => $"Service rejected the request ({problem.ResponseStatusCode}): {problem.Title} {problem.Detail}",
    ApiException api => api.ResponseStatusCode switch
    {
        401 => "Service rejected the API key (401). Set InvoiceServices__ApiKey or user secret InvoiceServices:ApiKey.",
        403 => "The Marketplace subscription does not allow this request (403).",
        429 => "Rate limit reached (429). Retry later.",
        var status => $"Service returned HTTP {status}.",
    },
    InvoiceMcpToolException tool => tool.Message,
    HttpRequestException { StatusCode: System.Net.HttpStatusCode.Unauthorized } =>
        "Service rejected the API key (401). Set InvoiceServices__ApiKey or user secret InvoiceServices:ApiKey.",
    HttpRequestException http => $"Service request failed: {http.Message}",
    _ => null,
};

// Every resilience pipeline reads the options at startup, so one invalid value can surface several times.
static IReadOnlyList<string> ValidationFailures(Exception exception) => exception switch
{
    OptionsValidationException validation => [.. validation.Failures.Distinct()],
    AggregateException aggregate => [.. aggregate.InnerExceptions.SelectMany(ValidationFailures).Distinct()],
    _ => [],
};
