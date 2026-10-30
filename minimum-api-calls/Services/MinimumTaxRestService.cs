using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using static MinimumTaxServices.Samples.Services.WalkthroughOutput;

namespace MinimumTaxServices.Samples.Services;

/// <summary>
/// Runs the business cases over REST without request or response classes. It reads the OpenAPI
/// description that the Scalar page at /docs/ renders, finds each operation there and builds the
/// request body from the operation's schema at runtime. Responses are read as JSON.
/// </summary>
public sealed partial class MinimumTaxRestService(HttpClient http, string? apiKey, string documentsPath)
    : IMinimumTaxTransport
{
    private JsonNode _description = new JsonObject();
    private SchemaRequestBuilder _builder = new(new JsonObject());

    public string Name => "rest";

    public async Task<int> RunAsync()
    {
        Write(ConsoleColor.White, "\n==================== REST walkthrough ====================");
        await LoadDescriptionAsync();
        return await new MinimumTaxBusinessCases(this, documentsPath).RunAsync();
    }

    public async Task<JsonNode?> CallAsync(string operation, IReadOnlyDictionary<string, object?> values)
    {
        // Operations are found by their path below /v1/, e.g. "gir/validate" → POST /v1/gir/validate.
        var (path, method, definition) = (
            from pathEntry in _description["paths"]!.AsObject()
            where pathEntry.Key.EndsWith("/v1/" + operation, StringComparison.Ordinal)
            from methodEntry in pathEntry.Value!.AsObject()
            select (pathEntry.Key, new HttpMethod(methodEntry.Key.ToUpperInvariant()), methodEntry.Value!)).FirstOrDefault();
        if (path is null)
        {
            throw new InvalidOperationException($"The operation '{operation}' is not documented in the API description.");
        }

        using var request = new HttpRequestMessage(method, path.TrimStart('/'));
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            request.Headers.Add("X-API-Key", apiKey);
        }

        if (definition["requestBody"]?["content"]?["application/json"]?["schema"] is { } schema)
        {
            request.Content = JsonContent.Create(_builder.Build(schema, values));
        }

        using var response = await http.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"{method} {path} failed ({(int)response.StatusCode} {response.StatusCode}): {body}", null, response.StatusCode);
        }

        return JsonNode.Parse(body);
    }

    /// <summary>Reads /docs/, follows it to the OpenAPI description and lists the documented operations.</summary>
    private async Task LoadDescriptionAsync()
    {
        var docsUrl = new Uri(http.BaseAddress!, "docs/");
        Step($"Reading the API description from {docsUrl}");
        var page = await http.GetStringAsync(docsUrl);
        var source = ScalarSource().Match(page);
        if (!source.Success)
        {
            throw new InvalidOperationException($"No OpenAPI description is referenced on {docsUrl}.");
        }

        // Scalar resolves a relative source URL against the site root.
        var url = Regex.Unescape(source.Groups["url"].Value);
        var specificationUrl = Uri.TryCreate(url, UriKind.Absolute, out var absolute) && absolute.Scheme is "http" or "https"
            ? absolute
            : new Uri(http.BaseAddress!, url.TrimStart('/'));
        _description = JsonNode.Parse(await http.GetStringAsync(specificationUrl))
            ?? throw new InvalidOperationException($"{specificationUrl} is empty.");
        _builder = new SchemaRequestBuilder(_description);

        Console.WriteLine($"{_description["info"]?["title"]} {_description["info"]?["version"]} ({specificationUrl})");
        foreach (var (path, operations) in _description["paths"]!.AsObject())
        {
            foreach (var (method, _) in operations!.AsObject())
            {
                Console.WriteLine($"  {method.ToUpperInvariant(),-6} {path}");
            }
        }
    }

    [GeneratedRegex("""
        "sources"\s*:\s*\[\s*\{[^}]*"url"\s*:\s*"(?<url>[^"]+)"
        """, RegexOptions.CultureInvariant)]
    private static partial Regex ScalarSource();
}
