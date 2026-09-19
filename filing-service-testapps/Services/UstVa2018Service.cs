using FilingService.TestApps.Clients;
using FilingService.TestApps.Configuration;
using FilingService.TestApps.Infrastructure;
using FilingService.TestApps.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FilingService.TestApps.Services;
[FilingTestApp("ustva-2018", "api/ustva/2018")]
public sealed class UstVa2018Service : FilingTestServiceBase
{
    private readonly UstVa2018Client client;

    public UstVa2018Service(UstVa2018Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "ustva-2018";
    public override string RoutePrefix => "api/ustva/2018";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        UstVa2018CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        UstVa2018CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        UstVa2018FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        UstVa2018FilingRequest request,
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
                ValidateAsync(new UstVa2018CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new UstVa2018CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new UstVa2018FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new UstVa2018FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}


