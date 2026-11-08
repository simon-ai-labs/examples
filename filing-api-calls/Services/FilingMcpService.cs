using System.Text.Json.Nodes;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using static FilingServices.Samples.Services.WalkthroughOutput;

namespace FilingServices.Samples.Services;

/// <summary>
/// Runs the business cases over MCP (Streamable HTTP). The routes come from <c>list_filing_routes</c>,
/// the contracts from <c>get_filing_contract</c>, and every call goes through the tool of its operation.
/// </summary>
public sealed class FilingMcpService(McpClient client) : IFilingTransport
{
    /// <summary>The tool behind each REST operation. The Abfrage and SpezRecht operations are REST only.</summary>
    private static readonly Dictionary<string, string> Tools = new()
    {
        ["validate"] = "validate_filing",
        ["preview"] = "preview_filing",
        ["decode"] = "decode_filing_document",
        // simulate_filing, never submit_filing: a sample has no business sending an Echtfall.
        ["filings"] = "simulate_filing"
    };

    public string Name => "mcp";

    public static async Task<McpClient> ConnectAsync(Uri endpoint, string? apiKey)
    {
        var headers = new Dictionary<string, string>();
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            headers["X-API-Key"] = apiKey;
        }

        var client = await McpClient.CreateAsync(new HttpClientTransport(new HttpClientTransportOptions
        {
            Endpoint = endpoint,
            TransportMode = HttpTransportMode.StreamableHttp,
            AdditionalHeaders = headers
        }));
        foreach (var tool in await client.ListToolsAsync())
        {
            Console.WriteLine($"  - {tool.Name}");
        }

        return client;
    }

    public bool Supports(string operation) => Tools.ContainsKey(operation);

    public async Task<IReadOnlyList<FilingStand>> LoadStandsAsync()
    {
        Step("Listing the routes with list_filing_routes");
        var routes = await CallToolAsync("list_filing_routes", new Dictionary<string, object?>());
        var documents = FilingCatalog.LatestDocumentsFromRoutes(routes);
        Console.WriteLine($"{routes["routes"]!.AsArray().Count} routes, {documents.Count} contracts hold the latest Stand of a Datenart.");

        var stands = new List<FilingStand>();
        foreach (var document in documents)
        {
            // Like the filing tools, get_filing_contract answers {status, body}; the body is the OpenAPI document.
            var answer = await CallToolAsync("get_filing_contract", new Dictionary<string, object?> { ["document"] = document });
            var contract = answer["body"]
                ?? throw new InvalidOperationException($"get_filing_contract returned no contract for '{document}' ({answer["status"]}).");
            stands.AddRange(FilingCatalog.ReadStands(document, contract));
        }

        return FilingCatalog.Latest(stands);
    }

    public async Task<FilingResponse> CallAsync(FilingStand stand, FilingOperation operation, JsonNode body)
    {
        // A route with a parameter (E-Bilanz) is called with the value already in place.
        var route = operation.Path[..operation.Path.LastIndexOf('/')];
        var answer = await CallToolAsync(Tools[operation.Name], new Dictionary<string, object?>
        {
            ["route"] = route,
            ["request"] = body
        });

        // Every filing tool answers {status, body}, exactly what the REST route would have answered.
        return new FilingResponse(answer["status"]?.GetValue<int>() ?? 0, answer["body"]);
    }

    private async Task<JsonNode> CallToolAsync(string tool, IReadOnlyDictionary<string, object?> arguments)
    {
        var result = await client.CallToolAsync(tool, arguments);
        if (result.StructuredContent is { } structured)
        {
            return JsonNode.Parse(structured.GetRawText())!;
        }

        var text = result.Content.OfType<TextContentBlock>().FirstOrDefault()?.Text
            ?? throw new InvalidOperationException($"MCP tool '{tool}' returned no content.");
        return JsonNode.Parse(text)!;
    }
}
