using System.Text.Json.Nodes;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using static InvoiceServices.Samples.Services.WalkthroughOutput;

namespace InvoiceServices.Samples.Services;

/// <summary>
/// Runs the business cases over MCP (Streamable HTTP) without request or response classes. The
/// arguments of each call are built at runtime from the tool's JSON input schema, results are read as JSON.
/// </summary>
public sealed class InvoiceMcpService(Uri endpoint, string? apiKey, string invoicePath, string? canonicalInvoiceJsonPath)
    : IInvoiceTransport
{
    private Dictionary<string, Tool> _tools = [];

    public string Name => "mcp";

    public async Task<int> RunAsync()
    {
        Write(ConsoleColor.White, "\n==================== MCP walkthrough =====================");
        Step($"Listing the tools of {endpoint}");
        await using (var client = await ConnectAsync())
        {
            _tools = (await client.ListToolsAsync()).ToDictionary(tool => tool.Name, tool => tool.ProtocolTool);
        }

        foreach (var tool in _tools.Values)
        {
            Console.WriteLine($"  - {tool.Name}: {tool.Description}");
        }

        return await new InvoiceBusinessCases(this, invoicePath, canonicalInvoiceJsonPath).RunAsync();
    }

    /// <summary>
    /// The tools are named after the operations (<c>validate_invoice</c>, <c>invoice_capabilities</c>) and take
    /// the REST request body as their <c>request</c> argument.
    /// </summary>
    public async Task<JsonNode?> CallAsync(string operation, IReadOnlyDictionary<string, object?> values)
    {
        var tool = _tools.Values.FirstOrDefault(candidate => candidate.Name == $"{operation}_invoice" || candidate.Name == $"invoice_{operation}")
            ?? throw new InvalidOperationException($"The MCP server offers no tool for '{operation}'.");
        var schema = JsonNode.Parse(tool.InputSchema.GetRawText())!;
        var arguments = new SchemaRequestBuilder(schema)
            .Build(schema, values.ToDictionary(value => $"request.{value.Key}", value => value.Value))
            .ToDictionary(argument => argument.Key, argument => (object?)argument.Value);

        // The Marketplace quota layer requires a fresh Idempotency-Key for each document tool call.
        await using var client = await ConnectAsync();
        var result = await client.CallToolAsync(tool.Name, arguments);
        var text = result.Content.OfType<TextContentBlock>().FirstOrDefault()?.Text;
        if (result.IsError == true)
        {
            throw new InvalidOperationException($"MCP tool '{tool.Name}' failed: {text}");
        }

        return result.StructuredContent is { } structured
            ? JsonNode.Parse(structured.GetRawText())
            : JsonNode.Parse(text ?? throw new InvalidOperationException($"MCP tool '{tool.Name}' returned no content."));
    }

    private Task<McpClient> ConnectAsync()
    {
        var headers = new Dictionary<string, string> { ["Idempotency-Key"] = Guid.NewGuid().ToString("N") };
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            headers["X-API-Key"] = apiKey;
        }

        return McpClient.CreateAsync(new HttpClientTransport(new HttpClientTransportOptions
        {
            Endpoint = endpoint,
            TransportMode = HttpTransportMode.StreamableHttp,
            AdditionalHeaders = headers
        }));
    }
}
