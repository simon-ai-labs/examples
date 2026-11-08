using System.Net.Http.Json;
using System.Text.Json.Nodes;
using static FilingServices.Samples.Services.WalkthroughOutput;

namespace FilingServices.Samples.Services;

/// <summary>
/// Runs the business cases over REST without request or response classes. The routes come from
/// <c>/openapi/index.json</c>, the request bodies from the examples of each Stand's own contract.
/// </summary>
public sealed class FilingRestService(HttpClient http, string? apiKey) : IFilingTransport
{
    public string Name => "rest";

    public bool Supports(string operation) => true;

    public async Task<IReadOnlyList<FilingStand>> LoadStandsAsync()
    {
        var indexUrl = new Uri(http.BaseAddress!, "openapi/index.json");
        Step($"Reading the contract catalog from {indexUrl}");
        var index = JsonNode.Parse(await http.GetStringAsync(indexUrl))!;
        var documents = FilingCatalog.LatestDocuments(index);
        Console.WriteLine($"{index["documents"]!.AsArray().Count} contracts, {documents.Count} of them hold the latest Stand of a Datenart.");

        // A handful of downloads at a time; the contracts of the large forms are several megabytes.
        var contracts = new JsonNode[documents.Count];
        await Parallel.ForEachAsync(Enumerable.Range(0, documents.Count), new ParallelOptions { MaxDegreeOfParallelism = 6 },
            async (i, cancellationToken) =>
                contracts[i] = JsonNode.Parse(await http.GetStringAsync($"openapi/{documents[i]}.json", cancellationToken))!);

        return FilingCatalog.Latest(documents.SelectMany((document, i) => FilingCatalog.ReadStands(document, contracts[i])));
    }

    public async Task<FilingResponse> CallAsync(FilingStand stand, FilingOperation operation, JsonNode body)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, operation.Path.TrimStart('/'))
        {
            Content = JsonContent.Create(body)
        };
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            request.Headers.Add("X-API-Key", apiKey);
        }

        using var response = await http.SendAsync(request);
        var text = await response.Content.ReadAsStringAsync();
        return new FilingResponse((int)response.StatusCode, string.IsNullOrWhiteSpace(text) ? null : JsonNode.Parse(text));
    }
}
