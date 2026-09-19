using FilingService.TestApps.Clients;
using FilingService.TestApps.Configuration;
using FilingService.TestApps.Infrastructure;
using FilingService.TestApps.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FilingService.TestApps.Services;
[FilingTestApp("lsta-2022", "api/lsta/2022")]
public sealed class LStA2022Service : FilingTestServiceBase
{
    private readonly LStA2022Client client;

    public LStA2022Service(LStA2022Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "lsta-2022";
    public override string RoutePrefix => "api/lsta/2022";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        LStA2022CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        LStA2022CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        LStA2022FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        LStA2022FilingRequest request,
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
                ValidateAsync(new LStA2022CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new LStA2022CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new LStA2022FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new LStA2022FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}


