using FilingService.TestApps.Infrastructure;
using FilingService.TestApps.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FilingService.TestApps.Clients;
[FilingTestApp("kapesta-2023", "api/kapesta/2023")]
public sealed class KapEStA2023Client(HttpClient httpClient) : FilingApiClientBase(httpClient)
{
    public override string AppName => "kapesta-2023";
    public override string RoutePrefix => "api/kapesta/2023";

    public Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        GetOpenApiDocumentCoreAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        KapEStA2023CheckRequest request,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync(
            $"{RoutePrefix}/validate",
            new JsonObject { ["data"] = request.Data.DeepClone() },
            cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        KapEStA2023CheckRequest request,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync(
            $"{RoutePrefix}/preview",
            new JsonObject { ["data"] = request.Data.DeepClone() },
            cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        KapEStA2023FilingRequest request,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync(
            $"{RoutePrefix}/filings",
            CreateFilingPayload(request),
            cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        KapEStA2023FilingRequest request,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync(
            $"{RoutePrefix}/filings",
            CreateFilingPayload(request),
            cancellationToken);

    private static JsonObject CreateFilingPayload(KapEStA2023FilingRequest request) =>
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


