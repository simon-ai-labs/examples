using System.Text.Json;
using System.Text.Json.Nodes;

namespace FilingService.TestApps;

[FilingTestApp("ustva-2019", "api/ustva/2019")]
public sealed class UstVa2019Client(HttpClient httpClient) : FilingApiClientBase(httpClient)
{
    public override string AppName => "ustva-2019";
    public override string RoutePrefix => "api/ustva/2019";

    public Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        GetOpenApiDocumentCoreAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        UstVa2019CheckRequest request,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync(
            $"{RoutePrefix}/validate",
            new JsonObject { ["data"] = request.Data.DeepClone() },
            cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        UstVa2019CheckRequest request,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync(
            $"{RoutePrefix}/preview",
            new JsonObject { ["data"] = request.Data.DeepClone() },
            cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        UstVa2019FilingRequest request,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync(
            $"{RoutePrefix}/filings",
            CreateFilingPayload(request),
            cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        UstVa2019FilingRequest request,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync(
            $"{RoutePrefix}/filings",
            CreateFilingPayload(request),
            cancellationToken);

    private static JsonObject CreateFilingPayload(UstVa2019FilingRequest request) =>
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

