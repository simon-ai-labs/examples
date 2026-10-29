using System.Text.Json.Nodes;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using static InvoiceServices.Samples.Services.WalkthroughOutput;

namespace InvoiceServices.Samples.Services;

/// <summary>
/// MCP walkthrough over Streamable HTTP without request or response classes. The arguments of each
/// call are built at runtime from the tool's JSON input schema, results are read as JSON.
/// </summary>
public sealed class InvoiceMcpService(Uri endpoint, string? apiKey, string invoicePath, string? canonicalInvoiceJsonPath)
{
    private const string Transport = "mcp";

    private Dictionary<string, Tool> _tools = [];

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

        Json(await CallAsync("invoice_capabilities", []));

        if (!string.IsNullOrWhiteSpace(canonicalInvoiceJsonPath))
        {
            var canonicalInvoice = JsonNode.Parse(await File.ReadAllTextAsync(canonicalInvoiceJsonPath))
                ?? throw new InvalidOperationException("The canonical invoice JSON file is empty.");
            var generated = await CallAsync("generate_invoice", new()
            {
                ["request.invoice"] = canonicalInvoice,
                ["request.target.format"] = "xRechnung",
                ["request.target.version"] = "3.0.2",
                ["request.target.profile"] = "xRechnung",
                ["request.target.syntax"] = "ubl"
            });
            Json(generated);
        }
        else
        {
            Console.WriteLine("\nSet INVOICE_CANONICAL_JSON_PATH to also call generate_invoice.");
        }

        var invoice = await File.ReadAllBytesAsync(invoicePath);
        var document = new Dictionary<string, object?>
        {
            ["request.document.content"] = invoice,
            ["request.document.fileName"] = Path.GetFileName(invoicePath),
            ["request.document.contentType"] = "application/xml",
            ["request.document.expectedRepresentation.format"] = "xRechnung",
            ["request.document.expectedRepresentation.version"] = "3.0.2",
            ["request.document.expectedRepresentation.profile"] = "xRechnung",
            ["request.document.expectedRepresentation.syntax"] = "ubl"
        };

        Json(await CallAsync("validate_invoice", document));
        Json(await CallAsync("read_invoice", document));

        var converted = await CallAsync("convert_invoice", new(document)
        {
            ["request.target.format"] = "zugferd",
            ["request.target.version"] = "2.5.2",
            ["request.target.profile"] = "en16931",
            ["request.target.syntax"] = "cii"
        });
        Json(converted);
        await SaveAsync(Transport, converted?["document"]?["fileName"]?.ToString() ?? "converted_zugferd_cii.xml",
            converted?["document"]?["content"]?.ToString());

        var rendered = await CallAsync("render_invoice", new(document) { ["request.outputFormat"] = "html" });
        await SaveAsync(Transport, rendered?["fileName"]?.ToString() ?? "rendered_invoice.html", rendered?["content"]?.ToString());

        Write(ConsoleColor.Green, "\n✔ MCP walkthrough completed.");
        return 0;
    }

    private async Task<JsonNode?> CallAsync(string toolName, Dictionary<string, object?> values)
    {
        var tool = _tools.GetValueOrDefault(toolName)
            ?? throw new InvalidOperationException($"The MCP server does not offer the tool '{toolName}'.");
        var schema = JsonNode.Parse(tool.InputSchema.GetRawText())!;
        var arguments = new SchemaRequestBuilder(schema).Build(schema, values)
            .ToDictionary(argument => argument.Key, argument => (object?)argument.Value);

        Step($"Calling MCP tool {toolName}");
        // The Marketplace quota layer requires a fresh Idempotency-Key for each document tool call.
        await using var client = await ConnectAsync();
        var result = await client.CallToolAsync(toolName, arguments);
        var text = result.Content.OfType<TextContentBlock>().FirstOrDefault()?.Text;
        if (result.IsError == true)
        {
            throw new InvalidOperationException($"MCP tool '{toolName}' failed: {text}");
        }

        return result.StructuredContent is { } structured
            ? JsonNode.Parse(structured.GetRawText())
            : JsonNode.Parse(text ?? throw new InvalidOperationException($"MCP tool '{toolName}' returned no content."));
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
