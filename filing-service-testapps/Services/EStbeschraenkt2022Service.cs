using FilingService.TestApps.Clients;
using FilingService.TestApps.Configuration;
using FilingService.TestApps.Infrastructure;
using FilingService.TestApps.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FilingService.TestApps.Services;
[FilingTestApp("estbeschraenkt-2022", "api/estbeschraenkt/2022")]
public sealed class EStbeschraenkt2022Service : FilingTestServiceBase
{
    private readonly EStbeschraenkt2022Client client;

    public EStbeschraenkt2022Service(EStbeschraenkt2022Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "estbeschraenkt-2022";
    public override string RoutePrefix => "api/estbeschraenkt/2022";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        EStbeschraenkt2022CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        EStbeschraenkt2022CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        EStbeschraenkt2022FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        EStbeschraenkt2022FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SubmitAsync(request, cancellationToken);

    public override Task<int> RunAsync(
        FilingTestOperation operation,
        string? payloadPath,
        string? certificatePath,
        string? pin) =>
        ExecuteAsync(
            operation,
            payloadPath,
            certificatePath,
            pin,
            (data, cancellationToken) =>
                ValidateAsync(new EStbeschraenkt2022CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new EStbeschraenkt2022CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new EStbeschraenkt2022FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new EStbeschraenkt2022FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}


