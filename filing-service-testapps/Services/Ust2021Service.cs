using FilingService.TestApps.Clients;
using FilingService.TestApps.Configuration;
using FilingService.TestApps.Infrastructure;
using FilingService.TestApps.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FilingService.TestApps.Services;
[FilingTestApp("ust-2021", "api/ust/2021")]
public sealed class Ust2021Service : FilingTestServiceBase
{
    private readonly Ust2021Client client;

    public Ust2021Service(Ust2021Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "ust-2021";
    public override string RoutePrefix => "api/ust/2021";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        Ust2021CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        Ust2021CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        Ust2021FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        Ust2021FilingRequest request,
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
                ValidateAsync(new Ust2021CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new Ust2021CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new Ust2021FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new Ust2021FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}


