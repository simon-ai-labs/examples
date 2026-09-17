using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FilingService.TestApps;

public abstract class FilingApiClientBase(HttpClient httpClient)
{
    protected HttpClient HttpClient { get; } = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    public abstract string AppName { get; }
    public abstract string RoutePrefix { get; }
    public string OpenApiPath => $"openapi/{AppName}.json";

    public Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Get, OpenApiPath, null, cancellationToken);

    public Task<JsonDocument> ValidateAsync(JsonNode data, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Post, $"{RoutePrefix}/validate", new JsonObject { ["data"] = data.DeepClone() }, cancellationToken);

    public Task<JsonDocument> PreviewAsync(JsonNode data, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Post, $"{RoutePrefix}/preview", new JsonObject { ["data"] = data.DeepClone() }, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        JsonNode data,
        string certificateBase64,
        string pin,
        CancellationToken cancellationToken = default) =>
        SendAsync(
            HttpMethod.Post,
            $"{RoutePrefix}/filings",
            CreateFilingRequest(data, certificateBase64, pin, simulation: true),
            cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        JsonNode data,
        string certificateBase64,
        string pin,
        CancellationToken cancellationToken = default) =>
        SendAsync(
            HttpMethod.Post,
            $"{RoutePrefix}/filings",
            CreateFilingRequest(data, certificateBase64, pin, simulation: false),
            cancellationToken);

    private static JsonObject CreateFilingRequest(
        JsonNode data,
        string certificateBase64,
        string pin,
        bool simulation) =>
        new()
        {
            ["data"] = data.DeepClone(),
            ["certificate"] = new JsonObject
            {
                ["pfx"] = certificateBase64,
                ["pin"] = pin
            },
            ["simulation"] = simulation
        };

    private async Task<JsonDocument> SendAsync(
        HttpMethod method,
        string path,
        JsonNode? payload,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, path);
        if (payload is not null)
        {
            request.Content = JsonContent.Create(payload);
        }

        using var response = await HttpClient.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"{method} {path} failed with {(int)response.StatusCode} {response.ReasonPhrase}: {body}");
        }

        return JsonDocument.Parse(string.IsNullOrWhiteSpace(body) ? "{}" : body);
    }
}
