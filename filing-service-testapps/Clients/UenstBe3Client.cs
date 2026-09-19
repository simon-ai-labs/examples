using FilingService.TestApps.Infrastructure;
using FilingService.TestApps.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FilingService.TestApps.Clients;
[FilingTestApp("uenst-anmeldung-be-3", "api/uenst-anmeldung-be/3")]
public sealed class UenstBe3Client(HttpClient httpClient) : FilingApiClientBase(httpClient)
{
    public override string AppName => "uenst-anmeldung-be-3";
    public override string RoutePrefix => "api/uenst-anmeldung-be/3";

    public Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        GetOpenApiDocumentCoreAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        UenstBe3CheckRequest request,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync(
            $"{RoutePrefix}/validate",
            new JsonObject { ["data"] = request.Data.DeepClone() },
            cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        UenstBe3CheckRequest request,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync(
            $"{RoutePrefix}/preview",
            new JsonObject { ["data"] = request.Data.DeepClone() },
            cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        UenstBe3FilingRequest request,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync(
            $"{RoutePrefix}/filings",
            CreateFilingPayload(request),
            cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        UenstBe3FilingRequest request,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync(
            $"{RoutePrefix}/filings",
            CreateFilingPayload(request),
            cancellationToken);

    private static JsonObject CreateFilingPayload(UenstBe3FilingRequest request) =>
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


