using System.Text.Json;
using System.Text.Json.Nodes;

namespace FilingService.TestApps;

[FilingTestApp("lsta-2026", "api/lsta/2026")]
public sealed class LStA2026Client(HttpClient httpClient) : FilingApiClientBase(httpClient)
{
    public override string AppName => "lsta-2026";
    public override string RoutePrefix => "api/lsta/2026";

    public Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        GetOpenApiDocumentCoreAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        LStA2026CheckRequest request,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync(
            $"{RoutePrefix}/validate",
            new JsonObject { ["data"] = request.Data.DeepClone() },
            cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        LStA2026CheckRequest request,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync(
            $"{RoutePrefix}/preview",
            new JsonObject { ["data"] = request.Data.DeepClone() },
            cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        LStA2026FilingRequest request,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync(
            $"{RoutePrefix}/filings",
            CreateFilingPayload(request),
            cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        LStA2026FilingRequest request,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync(
            $"{RoutePrefix}/filings",
            CreateFilingPayload(request),
            cancellationToken);

    private static JsonObject CreateFilingPayload(LStA2026FilingRequest request) =>
        new()
        {
            ["data"] = request.Data.DeepClone(),
            ["certificate"] = new JsonObject
            {
                ["pfx"] = request.CertificateBase64,
                ["pin"] = request.Pin
            },
            ["simulation"] = request.Simulation
        };
}

