using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FilingService.TestApps;

/// <summary>Shared HTTP transport only. Fachliche operations belong to the concrete client.</summary>
public abstract class FilingApiClientBase(HttpClient httpClient)
{
    protected HttpClient HttpClient { get; } = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    public abstract string AppName { get; }
    public abstract string RoutePrefix { get; }
    public string OpenApiPath => $"openapi/{AppName}.json";

    protected Task<JsonDocument> GetOpenApiDocumentCoreAsync(CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Get, OpenApiPath, null, cancellationToken);

    protected Task<JsonDocument> PostJsonAsync(
        string relativePath,
        JsonNode payload,
        CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Post, relativePath, payload, cancellationToken);

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

