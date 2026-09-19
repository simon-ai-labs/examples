using FilingService.TestApps.Clients;
using FilingService.TestApps.Configuration;
using FilingService.TestApps.Infrastructure;
using FilingService.TestApps.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FilingService.TestApps.Services;
[FilingTestApp("euer-2025", "api/euer/2025")]
public sealed class Euer2025Service : FilingTestServiceBase
{
    private readonly Euer2025Client client;

    public Euer2025Service(Euer2025Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "euer-2025";
    public override string RoutePrefix => "api/euer/2025";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        Euer2025CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        Euer2025CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        Euer2025FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        Euer2025FilingRequest request,
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
                ValidateAsync(new Euer2025CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new Euer2025CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new Euer2025FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new Euer2025FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}


