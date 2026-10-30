using System.Text.Json.Nodes;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using static MinimumTaxServices.Samples.Services.WalkthroughOutput;

namespace MinimumTaxServices.Samples.Services;

/// <summary>
/// Runs the business cases over MCP (Streamable HTTP) without request or response classes. The
/// arguments of each call are built at runtime from the tool's JSON input schema, results are read as JSON.
/// </summary>
public sealed class MinimumTaxMcpService(Uri endpoint, string? apiKey, string documentsPath)
    : IMinimumTaxTransport
{
    private Dictionary<string, Tool> _tools = [];
    private McpClient? _client;

    public string Name => "mcp";

    public async Task<int> RunAsync()
    {
        Write(ConsoleColor.White, "\n==================== MCP walkthrough =====================");
        Step($"Listing the tools of {endpoint}");
        // One MCP session serves every business case.
        await using var client = await ConnectAsync();
        _client = client;
        _tools = (await client.ListToolsAsync()).ToDictionary(tool => tool.Name, tool => tool.ProtocolTool);

        foreach (var tool in _tools.Values)
        {
            Console.WriteLine($"  - {tool.Name}: {tool.Description}");
        }

        return await new MinimumTaxBusinessCases(this, documentsPath).RunAsync();
    }

    /// <summary>
    /// The tools are named after the operations (<c>gir/validate</c> → <c>validate_gir</c>,
    /// <c>capabilities</c> → <c>minimum_tax_capabilities</c>) and take the REST request body as their
    /// <c>request</c> argument.
    /// </summary>
    public async Task<JsonNode?> CallAsync(string operation, IReadOnlyDictionary<string, object?> values)
    {
        var name = operation.Split('/') is [var family, var action] ? $"{action}_{family}" : operation;
        var tool = _tools.Values.FirstOrDefault(candidate => candidate.Name == name || candidate.Name.EndsWith($"_{name}", StringComparison.Ordinal))
            ?? throw new InvalidOperationException($"The MCP server offers no tool for '{operation}'.");
        var schema = JsonNode.Parse(tool.InputSchema.GetRawText())!;
        var arguments = new SchemaRequestBuilder(schema)
            .Build(schema, values.ToDictionary(value => $"request.{value.Key}", value => value.Value))
            .ToDictionary(argument => argument.Key, argument => (object?)argument.Value);

        var result = await _client!.CallToolAsync(tool.Name, arguments);
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
        var headers = new Dictionary<string, string>();
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
