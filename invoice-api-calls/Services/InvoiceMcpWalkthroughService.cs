using System.Text.Json;
using InvoiceServices.Samples.Configuration;
using ModelContextProtocol.Client;

namespace InvoiceServices.Samples.Services;

/// <summary>
/// Demonstrates calling the invoice service as an MCP server over Streamable HTTP.
/// The REST walkthrough remains available as the default program mode.
/// </summary>
public sealed class InvoiceMcpWalkthroughService(Uri endpoint, string? apiKey)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private const string OfficialTestInvoiceFileName = "01.06_minimal_test_ubl.xml";

    public async Task<int> RunAsync()
    {
        var headers = new Dictionary<string, string>();
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            headers["X-API-Key"] = apiKey;
        }

        var transport = new HttpClientTransport(new HttpClientTransportOptions
        {
            Endpoint = endpoint,
            TransportMode = HttpTransportMode.StreamableHttp,
            AdditionalHeaders = headers
        });

        await using var client = await McpClient.CreateAsync(transport);
        var tools = await client.ListToolsAsync();

        Console.WriteLine($"Connected to MCP endpoint: {endpoint}");
        Console.WriteLine("Available invoice tools:");
        foreach (var tool in tools)
        {
            Console.WriteLine($"  - {tool.Name}: {tool.Description}");
        }

        await InvokeAsync(client, "invoice_capabilities", new Dictionary<string, object?>());

        var invoicePath = Path.Combine(AppContext.BaseDirectory, "TestInvoices", OfficialTestInvoiceFileName);
        if (!File.Exists(invoicePath))
        {
            throw new FileNotFoundException("The bundled KoSIT test invoice is missing.", invoicePath);
        }

        var invoiceContent = Convert.ToBase64String(await File.ReadAllBytesAsync(invoicePath));
        var document = new Dictionary<string, object?>
        {
            ["content"] = invoiceContent,
            ["fileName"] = OfficialTestInvoiceFileName,
            ["contentType"] = "application/xml",
            ["expectedRepresentation"] = new Dictionary<string, object?>
            {
                ["format"] = "xRechnung",
                ["version"] = "3.0.2",
                ["profile"] = "xRechnung",
                ["syntax"] = "ubl"
            }
        };

        await InvokeAsync(client, "validate_invoice", new Dictionary<string, object?>
        {
            ["request"] = new Dictionary<string, object?> { ["document"] = document }
        });

        await InvokeAsync(client, "read_invoice", new Dictionary<string, object?>
        {
            ["request"] = new Dictionary<string, object?> { ["document"] = document }
        });

        await InvokeAsync(client, "convert_invoice", new Dictionary<string, object?>
        {
            ["request"] = new Dictionary<string, object?>
            {
                ["document"] = document,
                ["target"] = new Dictionary<string, object?>
                {
                    ["format"] = "zugferd",
                    ["version"] = "2.5.2",
                    ["profile"] = "en16931",
                    ["syntax"] = "cii"
                }
            }
        });

        await InvokeAsync(client, "render_invoice", new Dictionary<string, object?>
        {
            ["request"] = new Dictionary<string, object?>
            {
                ["document"] = document,
                ["outputFormat"] = "html"
            }
        });

        Console.WriteLine("MCP walkthrough completed.");
        return 0;
    }

    private static async Task InvokeAsync(
        McpClient client,
        string toolName,
        IReadOnlyDictionary<string, object?> arguments)
    {
        var tool = (await client.ListToolsAsync())
            .FirstOrDefault(candidate => string.Equals(candidate.Name, toolName, StringComparison.Ordinal))
            ?? throw new InvalidOperationException($"The MCP server does not expose the expected tool '{toolName}'.");

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n▶ Calling MCP tool: {toolName}");
        Console.ResetColor();

        var result = await tool.InvokeAsync(arguments);
        Console.WriteLine(JsonSerializer.Serialize(result, JsonOptions));
    }
}
