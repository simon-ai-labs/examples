using System.Text.Json;
using System.Text.Json.Nodes;

namespace FilingService.TestApps;

[FilingTestApp("34a-2021", "api/34a/2021")]
public sealed class E34a2021Service : FilingTestServiceBase
{
    private readonly E34a2021Client client;

    public E34a2021Service(E34a2021Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "34a-2021";
    public override string RoutePrefix => "api/34a/2021";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        E34a2021CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        E34a2021CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        E34a2021FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        E34a2021FilingRequest request,
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
                ValidateAsync(new E34a2021CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new E34a2021CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new E34a2021FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new E34a2021FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("34a-2022", "api/34a/2022")]
public sealed class E34a2022Service : FilingTestServiceBase
{
    private readonly E34a2022Client client;

    public E34a2022Service(E34a2022Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "34a-2022";
    public override string RoutePrefix => "api/34a/2022";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        E34a2022CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        E34a2022CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        E34a2022FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        E34a2022FilingRequest request,
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
                ValidateAsync(new E34a2022CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new E34a2022CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new E34a2022FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new E34a2022FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("34a-2023", "api/34a/2023")]
public sealed class E34a2023Service : FilingTestServiceBase
{
    private readonly E34a2023Client client;

    public E34a2023Service(E34a2023Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "34a-2023";
    public override string RoutePrefix => "api/34a/2023";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        E34a2023CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        E34a2023CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        E34a2023FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        E34a2023FilingRequest request,
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
                ValidateAsync(new E34a2023CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new E34a2023CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new E34a2023FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new E34a2023FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("34a-2024", "api/34a/2024")]
public sealed class E34a2024Service : FilingTestServiceBase
{
    private readonly E34a2024Client client;

    public E34a2024Service(E34a2024Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "34a-2024";
    public override string RoutePrefix => "api/34a/2024";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        E34a2024CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        E34a2024CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        E34a2024FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        E34a2024FilingRequest request,
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
                ValidateAsync(new E34a2024CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new E34a2024CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new E34a2024FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new E34a2024FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("34a-2025", "api/34a/2025")]
public sealed class E34a2025Service : FilingTestServiceBase
{
    private readonly E34a2025Client client;

    public E34a2025Service(E34a2025Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "34a-2025";
    public override string RoutePrefix => "api/34a/2025";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        E34a2025CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        E34a2025CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        E34a2025FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        E34a2025FilingRequest request,
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
                ValidateAsync(new E34a2025CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new E34a2025CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new E34a2025FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new E34a2025FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("antr-ansaessigkeit-besch-1", "api/antr-ansaessigkeit-besch/1")]
public sealed class AntrAnsBesch1Service : FilingTestServiceBase
{
    private readonly AntrAnsBesch1Client client;

    public AntrAnsBesch1Service(AntrAnsBesch1Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "antr-ansaessigkeit-besch-1";
    public override string RoutePrefix => "api/antr-ansaessigkeit-besch/1";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        AntrAnsBesch1CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        AntrAnsBesch1CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        AntrAnsBesch1FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        AntrAnsBesch1FilingRequest request,
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
                ValidateAsync(new AntrAnsBesch1CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new AntrAnsBesch1CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new AntrAnsBesch1FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new AntrAnsBesch1FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("astghzb-1", "api/astghzb/1")]
public sealed class AstGhzB1Service : FilingTestServiceBase
{
    private readonly AstGhzB1Client client;

    public AstGhzB1Service(AstGhzB1Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "astghzb-1";
    public override string RoutePrefix => "api/astghzb/1";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        AstGhzB1CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        AstGhzB1CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        AstGhzB1FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        AstGhzB1FilingRequest request,
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
                ValidateAsync(new AstGhzB1CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new AstGhzB1CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new AstGhzB1FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new AstGhzB1FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("aufzeichnung146a-1", "api/aufzeichnung146a/1")]
public sealed class Aufz146a1Service : FilingTestServiceBase
{
    private readonly Aufz146a1Client client;

    public Aufz146a1Service(Aufz146a1Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "aufzeichnung146a-1";
    public override string RoutePrefix => "api/aufzeichnung146a/1";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        Aufz146a1CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        Aufz146a1CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        Aufz146a1FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        Aufz146a1FilingRequest request,
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
                ValidateAsync(new Aufz146a1CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new Aufz146a1CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new Aufz146a1FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new Aufz146a1FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("bauleistung-stab-anmeldung-1", "api/bauleistung-stab-anmeldung/1")]
public sealed class BauStAbAnm1Service : FilingTestServiceBase
{
    private readonly BauStAbAnm1Client client;

    public BauStAbAnm1Service(BauStAbAnm1Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "bauleistung-stab-anmeldung-1";
    public override string RoutePrefix => "api/bauleistung-stab-anmeldung/1";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        BauStAbAnm1CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        BauStAbAnm1CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        BauStAbAnm1FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        BauStAbAnm1FilingRequest request,
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
                ValidateAsync(new BauStAbAnm1CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new BauStAbAnm1CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new BauStAbAnm1FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new BauStAbAnm1FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("bauleistung-stab-erstattung-1", "api/bauleistung-stab-erstattung/1")]
public sealed class BauStAbErs1Service : FilingTestServiceBase
{
    private readonly BauStAbErs1Client client;

    public BauStAbErs1Service(BauStAbErs1Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "bauleistung-stab-erstattung-1";
    public override string RoutePrefix => "api/bauleistung-stab-erstattung/1";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        BauStAbErs1CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        BauStAbErs1CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        BauStAbErs1FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        BauStAbErs1FilingRequest request,
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
                ValidateAsync(new BauStAbErs1CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new BauStAbErs1CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new BauStAbErs1FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new BauStAbErs1FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("e-le-v-ermaessigung-2026", "api/e-le-v-ermaessigung/2026")]
public sealed class ELeVErm2026Service : FilingTestServiceBase
{
    private readonly ELeVErm2026Client client;

    public ELeVErm2026Service(ELeVErm2026Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "e-le-v-ermaessigung-2026";
    public override string RoutePrefix => "api/e-le-v-ermaessigung/2026";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        ELeVErm2026CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        ELeVErm2026CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        ELeVErm2026FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        ELeVErm2026FilingRequest request,
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
                ValidateAsync(new ELeVErm2026CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new ELeVErm2026CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new ELeVErm2026FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new ELeVErm2026FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("einspruch-22", "api/einspruch/22")]
public sealed class Einspruch22Service : FilingTestServiceBase
{
    private readonly Einspruch22Client client;

    public Einspruch22Service(Einspruch22Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "einspruch-22";
    public override string RoutePrefix => "api/einspruch/22";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        Einspruch22CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        Einspruch22CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        Einspruch22FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        Einspruch22FilingRequest request,
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
                ValidateAsync(new Einspruch22CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new Einspruch22CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new Einspruch22FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new Einspruch22FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("erbschaftsteuer-4", "api/erbschaftsteuer/4")]
public sealed class Erbschaftsteuer4Service : FilingTestServiceBase
{
    private readonly Erbschaftsteuer4Client client;

    public Erbschaftsteuer4Service(Erbschaftsteuer4Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "erbschaftsteuer-4";
    public override string RoutePrefix => "api/erbschaftsteuer/4";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        Erbschaftsteuer4CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        Erbschaftsteuer4CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        Erbschaftsteuer4FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        Erbschaftsteuer4FilingRequest request,
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
                ValidateAsync(new Erbschaftsteuer4CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new Erbschaftsteuer4CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new Erbschaftsteuer4FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new Erbschaftsteuer4FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("erbschaftsteuer-5", "api/erbschaftsteuer/5")]
public sealed class Erbschaftsteuer5Service : FilingTestServiceBase
{
    private readonly Erbschaftsteuer5Client client;

    public Erbschaftsteuer5Service(Erbschaftsteuer5Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "erbschaftsteuer-5";
    public override string RoutePrefix => "api/erbschaftsteuer/5";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        Erbschaftsteuer5CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        Erbschaftsteuer5CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        Erbschaftsteuer5FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        Erbschaftsteuer5FilingRequest request,
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
                ValidateAsync(new Erbschaftsteuer5CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new Erbschaftsteuer5CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new Erbschaftsteuer5FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new Erbschaftsteuer5FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("est-2020", "api/est/2020")]
public sealed class ESt2020Service : FilingTestServiceBase
{
    private readonly ESt2020Client client;

    public ESt2020Service(ESt2020Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "est-2020";
    public override string RoutePrefix => "api/est/2020";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        ESt2020CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        ESt2020CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        ESt2020FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        ESt2020FilingRequest request,
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
                ValidateAsync(new ESt2020CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new ESt2020CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new ESt2020FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new ESt2020FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("est-2021", "api/est/2021")]
public sealed class ESt2021Service : FilingTestServiceBase
{
    private readonly ESt2021Client client;

    public ESt2021Service(ESt2021Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "est-2021";
    public override string RoutePrefix => "api/est/2021";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        ESt2021CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        ESt2021CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        ESt2021FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        ESt2021FilingRequest request,
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
                ValidateAsync(new ESt2021CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new ESt2021CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new ESt2021FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new ESt2021FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("est-2022", "api/est/2022")]
public sealed class ESt2022Service : FilingTestServiceBase
{
    private readonly ESt2022Client client;

    public ESt2022Service(ESt2022Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "est-2022";
    public override string RoutePrefix => "api/est/2022";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        ESt2022CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        ESt2022CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        ESt2022FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        ESt2022FilingRequest request,
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
                ValidateAsync(new ESt2022CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new ESt2022CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new ESt2022FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new ESt2022FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("est-2023", "api/est/2023")]
public sealed class ESt2023Service : FilingTestServiceBase
{
    private readonly ESt2023Client client;

    public ESt2023Service(ESt2023Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "est-2023";
    public override string RoutePrefix => "api/est/2023";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        ESt2023CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        ESt2023CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        ESt2023FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        ESt2023FilingRequest request,
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
                ValidateAsync(new ESt2023CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new ESt2023CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new ESt2023FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new ESt2023FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("est-2024", "api/est/2024")]
public sealed class ESt2024Service : FilingTestServiceBase
{
    private readonly ESt2024Client client;

    public ESt2024Service(ESt2024Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "est-2024";
    public override string RoutePrefix => "api/est/2024";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        ESt2024CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        ESt2024CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        ESt2024FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        ESt2024FilingRequest request,
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
                ValidateAsync(new ESt2024CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new ESt2024CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new ESt2024FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new ESt2024FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("est-2025", "api/est/2025")]
public sealed class ESt2025Service : FilingTestServiceBase
{
    private readonly ESt2025Client client;

    public ESt2025Service(ESt2025Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "est-2025";
    public override string RoutePrefix => "api/est/2025";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        ESt2025CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        ESt2025CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        ESt2025FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        ESt2025FilingRequest request,
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
                ValidateAsync(new ESt2025CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new ESt2025CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new ESt2025FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new ESt2025FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("estbeschraenkt-2020", "api/estbeschraenkt/2020")]
public sealed class EStbeschraenkt2020Service : FilingTestServiceBase
{
    private readonly EStbeschraenkt2020Client client;

    public EStbeschraenkt2020Service(EStbeschraenkt2020Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "estbeschraenkt-2020";
    public override string RoutePrefix => "api/estbeschraenkt/2020";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        EStbeschraenkt2020CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        EStbeschraenkt2020CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        EStbeschraenkt2020FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        EStbeschraenkt2020FilingRequest request,
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
                ValidateAsync(new EStbeschraenkt2020CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new EStbeschraenkt2020CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new EStbeschraenkt2020FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new EStbeschraenkt2020FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("estbeschraenkt-2021", "api/estbeschraenkt/2021")]
public sealed class EStbeschraenkt2021Service : FilingTestServiceBase
{
    private readonly EStbeschraenkt2021Client client;

    public EStbeschraenkt2021Service(EStbeschraenkt2021Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "estbeschraenkt-2021";
    public override string RoutePrefix => "api/estbeschraenkt/2021";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        EStbeschraenkt2021CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        EStbeschraenkt2021CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        EStbeschraenkt2021FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        EStbeschraenkt2021FilingRequest request,
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
                ValidateAsync(new EStbeschraenkt2021CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new EStbeschraenkt2021CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new EStbeschraenkt2021FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new EStbeschraenkt2021FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

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

[FilingTestApp("estbeschraenkt-2023", "api/estbeschraenkt/2023")]
public sealed class EStbeschraenkt2023Service : FilingTestServiceBase
{
    private readonly EStbeschraenkt2023Client client;

    public EStbeschraenkt2023Service(EStbeschraenkt2023Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "estbeschraenkt-2023";
    public override string RoutePrefix => "api/estbeschraenkt/2023";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        EStbeschraenkt2023CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        EStbeschraenkt2023CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        EStbeschraenkt2023FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        EStbeschraenkt2023FilingRequest request,
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
                ValidateAsync(new EStbeschraenkt2023CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new EStbeschraenkt2023CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new EStbeschraenkt2023FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new EStbeschraenkt2023FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("estbeschraenkt-2024", "api/estbeschraenkt/2024")]
public sealed class EStbeschraenkt2024Service : FilingTestServiceBase
{
    private readonly EStbeschraenkt2024Client client;

    public EStbeschraenkt2024Service(EStbeschraenkt2024Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "estbeschraenkt-2024";
    public override string RoutePrefix => "api/estbeschraenkt/2024";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        EStbeschraenkt2024CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        EStbeschraenkt2024CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        EStbeschraenkt2024FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        EStbeschraenkt2024FilingRequest request,
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
                ValidateAsync(new EStbeschraenkt2024CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new EStbeschraenkt2024CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new EStbeschraenkt2024FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new EStbeschraenkt2024FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("estbeschraenkt-2025", "api/estbeschraenkt/2025")]
public sealed class EStbeschraenkt2025Service : FilingTestServiceBase
{
    private readonly EStbeschraenkt2025Client client;

    public EStbeschraenkt2025Service(EStbeschraenkt2025Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "estbeschraenkt-2025";
    public override string RoutePrefix => "api/estbeschraenkt/2025";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        EStbeschraenkt2025CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        EStbeschraenkt2025CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        EStbeschraenkt2025FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        EStbeschraenkt2025FilingRequest request,
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
                ValidateAsync(new EStbeschraenkt2025CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new EStbeschraenkt2025CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new EStbeschraenkt2025FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new EStbeschraenkt2025FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("euer-2021", "api/euer/2021")]
public sealed class Euer2021Service : FilingTestServiceBase
{
    private readonly Euer2021Client client;

    public Euer2021Service(Euer2021Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "euer-2021";
    public override string RoutePrefix => "api/euer/2021";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        Euer2021CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        Euer2021CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        Euer2021FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        Euer2021FilingRequest request,
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
                ValidateAsync(new Euer2021CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new Euer2021CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new Euer2021FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new Euer2021FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("euer-2022", "api/euer/2022")]
public sealed class Euer2022Service : FilingTestServiceBase
{
    private readonly Euer2022Client client;

    public Euer2022Service(Euer2022Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "euer-2022";
    public override string RoutePrefix => "api/euer/2022";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        Euer2022CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        Euer2022CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        Euer2022FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        Euer2022FilingRequest request,
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
                ValidateAsync(new Euer2022CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new Euer2022CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new Euer2022FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new Euer2022FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("euer-2023", "api/euer/2023")]
public sealed class Euer2023Service : FilingTestServiceBase
{
    private readonly Euer2023Client client;

    public Euer2023Service(Euer2023Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "euer-2023";
    public override string RoutePrefix => "api/euer/2023";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        Euer2023CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        Euer2023CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        Euer2023FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        Euer2023FilingRequest request,
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
                ValidateAsync(new Euer2023CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new Euer2023CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new Euer2023FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new Euer2023FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("euer-2024", "api/euer/2024")]
public sealed class Euer2024Service : FilingTestServiceBase
{
    private readonly Euer2024Client client;

    public Euer2024Service(Euer2024Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "euer-2024";
    public override string RoutePrefix => "api/euer/2024";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        Euer2024CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        Euer2024CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        Euer2024FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        Euer2024FilingRequest request,
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
                ValidateAsync(new Euer2024CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new Euer2024CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new Euer2024FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new Euer2024FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

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

[FilingTestApp("fein90-2021", "api/fein90/2021")]
public sealed class Fein902021Service : FilingTestServiceBase
{
    private readonly Fein902021Client client;

    public Fein902021Service(Fein902021Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "fein90-2021";
    public override string RoutePrefix => "api/fein90/2021";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        Fein902021CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        Fein902021CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        Fein902021FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        Fein902021FilingRequest request,
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
                ValidateAsync(new Fein902021CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new Fein902021CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new Fein902021FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new Fein902021FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("fein90-2022", "api/fein90/2022")]
public sealed class Fein902022Service : FilingTestServiceBase
{
    private readonly Fein902022Client client;

    public Fein902022Service(Fein902022Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "fein90-2022";
    public override string RoutePrefix => "api/fein90/2022";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        Fein902022CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        Fein902022CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        Fein902022FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        Fein902022FilingRequest request,
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
                ValidateAsync(new Fein902022CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new Fein902022CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new Fein902022FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new Fein902022FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("feinbeteiligte-1", "api/feinbeteiligte/1")]
public sealed class FeinBeteiligte1Service : FilingTestServiceBase
{
    private readonly FeinBeteiligte1Client client;

    public FeinBeteiligte1Service(FeinBeteiligte1Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "feinbeteiligte-1";
    public override string RoutePrefix => "api/feinbeteiligte/1";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        FeinBeteiligte1CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        FeinBeteiligte1CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        FeinBeteiligte1FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        FeinBeteiligte1FilingRequest request,
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
                ValidateAsync(new FeinBeteiligte1CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new FeinBeteiligte1CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new FeinBeteiligte1FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new FeinBeteiligte1FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("grundsteuerwert-2", "api/grundsteuerwert/2")]
public sealed class Gsw2Service : FilingTestServiceBase
{
    private readonly Gsw2Client client;

    public Gsw2Service(Gsw2Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "grundsteuerwert-2";
    public override string RoutePrefix => "api/grundsteuerwert/2";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        Gsw2CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        Gsw2CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        Gsw2FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        Gsw2FilingRequest request,
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
                ValidateAsync(new Gsw2CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new Gsw2CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new Gsw2FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new Gsw2FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("ibanmeldung-1", "api/ibanmeldung/1")]
public sealed class Iban1Service : FilingTestServiceBase
{
    private readonly Iban1Client client;

    public Iban1Service(Iban1Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "ibanmeldung-1";
    public override string RoutePrefix => "api/ibanmeldung/1";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        Iban1CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        Iban1CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        Iban1FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        Iban1FilingRequest request,
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
                ValidateAsync(new Iban1CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new Iban1CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new Iban1FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new Iban1FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("kapest-invstg-2016", "api/kapest-invstg/2016")]
public sealed class KapEStInvStG2016Service : FilingTestServiceBase
{
    private readonly KapEStInvStG2016Client client;

    public KapEStInvStG2016Service(KapEStInvStG2016Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "kapest-invstg-2016";
    public override string RoutePrefix => "api/kapest-invstg/2016";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        KapEStInvStG2016CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        KapEStInvStG2016CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        KapEStInvStG2016FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        KapEStInvStG2016FilingRequest request,
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
                ValidateAsync(new KapEStInvStG2016CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new KapEStInvStG2016CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new KapEStInvStG2016FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new KapEStInvStG2016FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("kapest-invstg-2017", "api/kapest-invstg/2017")]
public sealed class KapEStInvStG2017Service : FilingTestServiceBase
{
    private readonly KapEStInvStG2017Client client;

    public KapEStInvStG2017Service(KapEStInvStG2017Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "kapest-invstg-2017";
    public override string RoutePrefix => "api/kapest-invstg/2017";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        KapEStInvStG2017CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        KapEStInvStG2017CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        KapEStInvStG2017FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        KapEStInvStG2017FilingRequest request,
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
                ValidateAsync(new KapEStInvStG2017CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new KapEStInvStG2017CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new KapEStInvStG2017FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new KapEStInvStG2017FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("kapest-invstg-2018", "api/kapest-invstg/2018")]
public sealed class KapEStInvStG2018Service : FilingTestServiceBase
{
    private readonly KapEStInvStG2018Client client;

    public KapEStInvStG2018Service(KapEStInvStG2018Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "kapest-invstg-2018";
    public override string RoutePrefix => "api/kapest-invstg/2018";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        KapEStInvStG2018CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        KapEStInvStG2018CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        KapEStInvStG2018FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        KapEStInvStG2018FilingRequest request,
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
                ValidateAsync(new KapEStInvStG2018CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new KapEStInvStG2018CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new KapEStInvStG2018FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new KapEStInvStG2018FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("kapest-invstg-2019", "api/kapest-invstg/2019")]
public sealed class KapEStInvStG2019Service : FilingTestServiceBase
{
    private readonly KapEStInvStG2019Client client;

    public KapEStInvStG2019Service(KapEStInvStG2019Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "kapest-invstg-2019";
    public override string RoutePrefix => "api/kapest-invstg/2019";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        KapEStInvStG2019CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        KapEStInvStG2019CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        KapEStInvStG2019FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        KapEStInvStG2019FilingRequest request,
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
                ValidateAsync(new KapEStInvStG2019CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new KapEStInvStG2019CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new KapEStInvStG2019FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new KapEStInvStG2019FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("kapest-invstg-2020", "api/kapest-invstg/2020")]
public sealed class KapEStInvStG2020Service : FilingTestServiceBase
{
    private readonly KapEStInvStG2020Client client;

    public KapEStInvStG2020Service(KapEStInvStG2020Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "kapest-invstg-2020";
    public override string RoutePrefix => "api/kapest-invstg/2020";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        KapEStInvStG2020CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        KapEStInvStG2020CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        KapEStInvStG2020FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        KapEStInvStG2020FilingRequest request,
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
                ValidateAsync(new KapEStInvStG2020CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new KapEStInvStG2020CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new KapEStInvStG2020FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new KapEStInvStG2020FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("kapest-invstg-2021", "api/kapest-invstg/2021")]
public sealed class KapEStInvStG2021Service : FilingTestServiceBase
{
    private readonly KapEStInvStG2021Client client;

    public KapEStInvStG2021Service(KapEStInvStG2021Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "kapest-invstg-2021";
    public override string RoutePrefix => "api/kapest-invstg/2021";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        KapEStInvStG2021CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        KapEStInvStG2021CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        KapEStInvStG2021FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        KapEStInvStG2021FilingRequest request,
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
                ValidateAsync(new KapEStInvStG2021CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new KapEStInvStG2021CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new KapEStInvStG2021FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new KapEStInvStG2021FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("kapest-invstg-2022", "api/kapest-invstg/2022")]
public sealed class KapEStInvStG2022Service : FilingTestServiceBase
{
    private readonly KapEStInvStG2022Client client;

    public KapEStInvStG2022Service(KapEStInvStG2022Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "kapest-invstg-2022";
    public override string RoutePrefix => "api/kapest-invstg/2022";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        KapEStInvStG2022CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        KapEStInvStG2022CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        KapEStInvStG2022FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        KapEStInvStG2022FilingRequest request,
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
                ValidateAsync(new KapEStInvStG2022CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new KapEStInvStG2022CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new KapEStInvStG2022FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new KapEStInvStG2022FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("kapest-invstg-2023", "api/kapest-invstg/2023")]
public sealed class KapEStInvStG2023Service : FilingTestServiceBase
{
    private readonly KapEStInvStG2023Client client;

    public KapEStInvStG2023Service(KapEStInvStG2023Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "kapest-invstg-2023";
    public override string RoutePrefix => "api/kapest-invstg/2023";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        KapEStInvStG2023CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        KapEStInvStG2023CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        KapEStInvStG2023FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        KapEStInvStG2023FilingRequest request,
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
                ValidateAsync(new KapEStInvStG2023CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new KapEStInvStG2023CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new KapEStInvStG2023FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new KapEStInvStG2023FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("kapest-invstg-2024", "api/kapest-invstg/2024")]
public sealed class KapEStInvStG2024Service : FilingTestServiceBase
{
    private readonly KapEStInvStG2024Client client;

    public KapEStInvStG2024Service(KapEStInvStG2024Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "kapest-invstg-2024";
    public override string RoutePrefix => "api/kapest-invstg/2024";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        KapEStInvStG2024CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        KapEStInvStG2024CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        KapEStInvStG2024FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        KapEStInvStG2024FilingRequest request,
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
                ValidateAsync(new KapEStInvStG2024CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new KapEStInvStG2024CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new KapEStInvStG2024FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new KapEStInvStG2024FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("kapest-invstg-2025", "api/kapest-invstg/2025")]
public sealed class KapEStInvStG2025Service : FilingTestServiceBase
{
    private readonly KapEStInvStG2025Client client;

    public KapEStInvStG2025Service(KapEStInvStG2025Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "kapest-invstg-2025";
    public override string RoutePrefix => "api/kapest-invstg/2025";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        KapEStInvStG2025CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        KapEStInvStG2025CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        KapEStInvStG2025FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        KapEStInvStG2025FilingRequest request,
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
                ValidateAsync(new KapEStInvStG2025CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new KapEStInvStG2025CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new KapEStInvStG2025FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new KapEStInvStG2025FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("kapest-invstg-2026", "api/kapest-invstg/2026")]
public sealed class KapEStInvStG2026Service : FilingTestServiceBase
{
    private readonly KapEStInvStG2026Client client;

    public KapEStInvStG2026Service(KapEStInvStG2026Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "kapest-invstg-2026";
    public override string RoutePrefix => "api/kapest-invstg/2026";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        KapEStInvStG2026CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        KapEStInvStG2026CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        KapEStInvStG2026FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        KapEStInvStG2026FilingRequest request,
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
                ValidateAsync(new KapEStInvStG2026CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new KapEStInvStG2026CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new KapEStInvStG2026FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new KapEStInvStG2026FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("kapesta-2016", "api/kapesta/2016")]
public sealed class KapEStA2016Service : FilingTestServiceBase
{
    private readonly KapEStA2016Client client;

    public KapEStA2016Service(KapEStA2016Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "kapesta-2016";
    public override string RoutePrefix => "api/kapesta/2016";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        KapEStA2016CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        KapEStA2016CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        KapEStA2016FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        KapEStA2016FilingRequest request,
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
                ValidateAsync(new KapEStA2016CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new KapEStA2016CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new KapEStA2016FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new KapEStA2016FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("kapesta-2017", "api/kapesta/2017")]
public sealed class KapEStA2017Service : FilingTestServiceBase
{
    private readonly KapEStA2017Client client;

    public KapEStA2017Service(KapEStA2017Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "kapesta-2017";
    public override string RoutePrefix => "api/kapesta/2017";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        KapEStA2017CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        KapEStA2017CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        KapEStA2017FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        KapEStA2017FilingRequest request,
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
                ValidateAsync(new KapEStA2017CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new KapEStA2017CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new KapEStA2017FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new KapEStA2017FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("kapesta-2018", "api/kapesta/2018")]
public sealed class KapEStA2018Service : FilingTestServiceBase
{
    private readonly KapEStA2018Client client;

    public KapEStA2018Service(KapEStA2018Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "kapesta-2018";
    public override string RoutePrefix => "api/kapesta/2018";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        KapEStA2018CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        KapEStA2018CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        KapEStA2018FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        KapEStA2018FilingRequest request,
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
                ValidateAsync(new KapEStA2018CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new KapEStA2018CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new KapEStA2018FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new KapEStA2018FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("kapesta-2019", "api/kapesta/2019")]
public sealed class KapEStA2019Service : FilingTestServiceBase
{
    private readonly KapEStA2019Client client;

    public KapEStA2019Service(KapEStA2019Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "kapesta-2019";
    public override string RoutePrefix => "api/kapesta/2019";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        KapEStA2019CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        KapEStA2019CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        KapEStA2019FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        KapEStA2019FilingRequest request,
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
                ValidateAsync(new KapEStA2019CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new KapEStA2019CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new KapEStA2019FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new KapEStA2019FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("kapesta-2020", "api/kapesta/2020")]
public sealed class KapEStA2020Service : FilingTestServiceBase
{
    private readonly KapEStA2020Client client;

    public KapEStA2020Service(KapEStA2020Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "kapesta-2020";
    public override string RoutePrefix => "api/kapesta/2020";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        KapEStA2020CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        KapEStA2020CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        KapEStA2020FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        KapEStA2020FilingRequest request,
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
                ValidateAsync(new KapEStA2020CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new KapEStA2020CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new KapEStA2020FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new KapEStA2020FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("kapesta-2021", "api/kapesta/2021")]
public sealed class KapEStA2021Service : FilingTestServiceBase
{
    private readonly KapEStA2021Client client;

    public KapEStA2021Service(KapEStA2021Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "kapesta-2021";
    public override string RoutePrefix => "api/kapesta/2021";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        KapEStA2021CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        KapEStA2021CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        KapEStA2021FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        KapEStA2021FilingRequest request,
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
                ValidateAsync(new KapEStA2021CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new KapEStA2021CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new KapEStA2021FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new KapEStA2021FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("kapesta-2022", "api/kapesta/2022")]
public sealed class KapEStA2022Service : FilingTestServiceBase
{
    private readonly KapEStA2022Client client;

    public KapEStA2022Service(KapEStA2022Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "kapesta-2022";
    public override string RoutePrefix => "api/kapesta/2022";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        KapEStA2022CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        KapEStA2022CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        KapEStA2022FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        KapEStA2022FilingRequest request,
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
                ValidateAsync(new KapEStA2022CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new KapEStA2022CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new KapEStA2022FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new KapEStA2022FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("kapesta-2023", "api/kapesta/2023")]
public sealed class KapEStA2023Service : FilingTestServiceBase
{
    private readonly KapEStA2023Client client;

    public KapEStA2023Service(KapEStA2023Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "kapesta-2023";
    public override string RoutePrefix => "api/kapesta/2023";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        KapEStA2023CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        KapEStA2023CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        KapEStA2023FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        KapEStA2023FilingRequest request,
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
                ValidateAsync(new KapEStA2023CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new KapEStA2023CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new KapEStA2023FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new KapEStA2023FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("kapesta-2024", "api/kapesta/2024")]
public sealed class KapEStA2024Service : FilingTestServiceBase
{
    private readonly KapEStA2024Client client;

    public KapEStA2024Service(KapEStA2024Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "kapesta-2024";
    public override string RoutePrefix => "api/kapesta/2024";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        KapEStA2024CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        KapEStA2024CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        KapEStA2024FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        KapEStA2024FilingRequest request,
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
                ValidateAsync(new KapEStA2024CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new KapEStA2024CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new KapEStA2024FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new KapEStA2024FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("kapesta-2025", "api/kapesta/2025")]
public sealed class KapEStA2025Service : FilingTestServiceBase
{
    private readonly KapEStA2025Client client;

    public KapEStA2025Service(KapEStA2025Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "kapesta-2025";
    public override string RoutePrefix => "api/kapesta/2025";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        KapEStA2025CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        KapEStA2025CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        KapEStA2025FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        KapEStA2025FilingRequest request,
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
                ValidateAsync(new KapEStA2025CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new KapEStA2025CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new KapEStA2025FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new KapEStA2025FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("kapesta-2026", "api/kapesta/2026")]
public sealed class KapEStA2026Service : FilingTestServiceBase
{
    private readonly KapEStA2026Client client;

    public KapEStA2026Service(KapEStA2026Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "kapesta-2026";
    public override string RoutePrefix => "api/kapesta/2026";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        KapEStA2026CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        KapEStA2026CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        KapEStA2026FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        KapEStA2026FilingRequest request,
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
                ValidateAsync(new KapEStA2026CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new KapEStA2026CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new KapEStA2026FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new KapEStA2026FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("ktt-anmeldung-hh-3", "api/ktt-anmeldung-hh/3")]
public sealed class KttHh3Service : FilingTestServiceBase
{
    private readonly KttHh3Client client;

    public KttHh3Service(KttHh3Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "ktt-anmeldung-hh-3";
    public override string RoutePrefix => "api/ktt-anmeldung-hh/3";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        KttHh3CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        KttHh3CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        KttHh3FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        KttHh3FilingRequest request,
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
                ValidateAsync(new KttHh3CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new KttHh3CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new KttHh3FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new KttHh3FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("lsta-2016", "api/lsta/2016")]
public sealed class LStA2016Service : FilingTestServiceBase
{
    private readonly LStA2016Client client;

    public LStA2016Service(LStA2016Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "lsta-2016";
    public override string RoutePrefix => "api/lsta/2016";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        LStA2016CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        LStA2016CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        LStA2016FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        LStA2016FilingRequest request,
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
                ValidateAsync(new LStA2016CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new LStA2016CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new LStA2016FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new LStA2016FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("lsta-2017", "api/lsta/2017")]
public sealed class LStA2017Service : FilingTestServiceBase
{
    private readonly LStA2017Client client;

    public LStA2017Service(LStA2017Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "lsta-2017";
    public override string RoutePrefix => "api/lsta/2017";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        LStA2017CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        LStA2017CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        LStA2017FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        LStA2017FilingRequest request,
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
                ValidateAsync(new LStA2017CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new LStA2017CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new LStA2017FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new LStA2017FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("lsta-2018", "api/lsta/2018")]
public sealed class LStA2018Service : FilingTestServiceBase
{
    private readonly LStA2018Client client;

    public LStA2018Service(LStA2018Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "lsta-2018";
    public override string RoutePrefix => "api/lsta/2018";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        LStA2018CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        LStA2018CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        LStA2018FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        LStA2018FilingRequest request,
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
                ValidateAsync(new LStA2018CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new LStA2018CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new LStA2018FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new LStA2018FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("lsta-2019", "api/lsta/2019")]
public sealed class LStA2019Service : FilingTestServiceBase
{
    private readonly LStA2019Client client;

    public LStA2019Service(LStA2019Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "lsta-2019";
    public override string RoutePrefix => "api/lsta/2019";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        LStA2019CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        LStA2019CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        LStA2019FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        LStA2019FilingRequest request,
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
                ValidateAsync(new LStA2019CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new LStA2019CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new LStA2019FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new LStA2019FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("lsta-2020", "api/lsta/2020")]
public sealed class LStA2020Service : FilingTestServiceBase
{
    private readonly LStA2020Client client;

    public LStA2020Service(LStA2020Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "lsta-2020";
    public override string RoutePrefix => "api/lsta/2020";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        LStA2020CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        LStA2020CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        LStA2020FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        LStA2020FilingRequest request,
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
                ValidateAsync(new LStA2020CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new LStA2020CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new LStA2020FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new LStA2020FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("lsta-2021", "api/lsta/2021")]
public sealed class LStA2021Service : FilingTestServiceBase
{
    private readonly LStA2021Client client;

    public LStA2021Service(LStA2021Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "lsta-2021";
    public override string RoutePrefix => "api/lsta/2021";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        LStA2021CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        LStA2021CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        LStA2021FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        LStA2021FilingRequest request,
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
                ValidateAsync(new LStA2021CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new LStA2021CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new LStA2021FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new LStA2021FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

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

[FilingTestApp("lsta-2023", "api/lsta/2023")]
public sealed class LStA2023Service : FilingTestServiceBase
{
    private readonly LStA2023Client client;

    public LStA2023Service(LStA2023Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "lsta-2023";
    public override string RoutePrefix => "api/lsta/2023";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        LStA2023CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        LStA2023CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        LStA2023FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        LStA2023FilingRequest request,
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
                ValidateAsync(new LStA2023CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new LStA2023CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new LStA2023FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new LStA2023FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("lsta-2024", "api/lsta/2024")]
public sealed class LStA2024Service : FilingTestServiceBase
{
    private readonly LStA2024Client client;

    public LStA2024Service(LStA2024Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "lsta-2024";
    public override string RoutePrefix => "api/lsta/2024";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        LStA2024CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        LStA2024CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        LStA2024FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        LStA2024FilingRequest request,
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
                ValidateAsync(new LStA2024CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new LStA2024CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new LStA2024FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new LStA2024FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("lsta-2025", "api/lsta/2025")]
public sealed class LStA2025Service : FilingTestServiceBase
{
    private readonly LStA2025Client client;

    public LStA2025Service(LStA2025Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "lsta-2025";
    public override string RoutePrefix => "api/lsta/2025";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        LStA2025CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        LStA2025CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        LStA2025FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        LStA2025FilingRequest request,
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
                ValidateAsync(new LStA2025CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new LStA2025CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new LStA2025FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new LStA2025FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("lsta-2026", "api/lsta/2026")]
public sealed class LStA2026Service : FilingTestServiceBase
{
    private readonly LStA2026Client client;

    public LStA2026Service(LStA2026Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "lsta-2026";
    public override string RoutePrefix => "api/lsta/2026";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        LStA2026CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        LStA2026CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        LStA2026FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        LStA2026FilingRequest request,
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
                ValidateAsync(new LStA2026CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new LStA2026CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new LStA2026FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new LStA2026FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("mindestst-unternehmen-eu-1", "api/mindestst-unternehmen-eu/1")]
public sealed class MindestStEu1Service : FilingTestServiceBase
{
    private readonly MindestStEu1Client client;

    public MindestStEu1Service(MindestStEu1Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "mindestst-unternehmen-eu-1";
    public override string RoutePrefix => "api/mindestst-unternehmen-eu/1";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        MindestStEu1CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        MindestStEu1CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        MindestStEu1FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        MindestStEu1FilingRequest request,
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
                ValidateAsync(new MindestStEu1CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new MindestStEu1CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new MindestStEu1FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new MindestStEu1FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("schenkungsteuer-4", "api/schenkungsteuer/4")]
public sealed class Schenkungsteuer4Service : FilingTestServiceBase
{
    private readonly Schenkungsteuer4Client client;

    public Schenkungsteuer4Service(Schenkungsteuer4Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "schenkungsteuer-4";
    public override string RoutePrefix => "api/schenkungsteuer/4";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        Schenkungsteuer4CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        Schenkungsteuer4CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        Schenkungsteuer4FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        Schenkungsteuer4FilingRequest request,
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
                ValidateAsync(new Schenkungsteuer4CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new Schenkungsteuer4CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new Schenkungsteuer4FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new Schenkungsteuer4FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("schenkungsteuer-5", "api/schenkungsteuer/5")]
public sealed class Schenkungsteuer5Service : FilingTestServiceBase
{
    private readonly Schenkungsteuer5Client client;

    public Schenkungsteuer5Service(Schenkungsteuer5Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "schenkungsteuer-5";
    public override string RoutePrefix => "api/schenkungsteuer/5";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        Schenkungsteuer5CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        Schenkungsteuer5CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        Schenkungsteuer5FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        Schenkungsteuer5FilingRequest request,
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
                ValidateAsync(new Schenkungsteuer5CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new Schenkungsteuer5CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new Schenkungsteuer5FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new Schenkungsteuer5FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("stab-s50a-2024", "api/stab-s50a/2024")]
public sealed class StAbS50a2024Service : FilingTestServiceBase
{
    private readonly StAbS50a2024Client client;

    public StAbS50a2024Service(StAbS50a2024Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "stab-s50a-2024";
    public override string RoutePrefix => "api/stab-s50a/2024";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        StAbS50a2024CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        StAbS50a2024CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        StAbS50a2024FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        StAbS50a2024FilingRequest request,
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
                ValidateAsync(new StAbS50a2024CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new StAbS50a2024CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new StAbS50a2024FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new StAbS50a2024FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("stab-s50a-2025", "api/stab-s50a/2025")]
public sealed class StAbS50a2025Service : FilingTestServiceBase
{
    private readonly StAbS50a2025Client client;

    public StAbS50a2025Service(StAbS50a2025Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "stab-s50a-2025";
    public override string RoutePrefix => "api/stab-s50a/2025";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        StAbS50a2025CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        StAbS50a2025CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        StAbS50a2025FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        StAbS50a2025FilingRequest request,
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
                ValidateAsync(new StAbS50a2025CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new StAbS50a2025CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new StAbS50a2025FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new StAbS50a2025FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("stab-s50a-2026", "api/stab-s50a/2026")]
public sealed class StAbS50a2026Service : FilingTestServiceBase
{
    private readonly StAbS50a2026Client client;

    public StAbS50a2026Service(StAbS50a2026Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "stab-s50a-2026";
    public override string RoutePrefix => "api/stab-s50a/2026";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        StAbS50a2026CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        StAbS50a2026CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        StAbS50a2026FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        StAbS50a2026FilingRequest request,
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
                ValidateAsync(new StAbS50a2026CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new StAbS50a2026CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new StAbS50a2026FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new StAbS50a2026FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("uenst-anmeldung-be-3", "api/uenst-anmeldung-be/3")]
public sealed class UenstBe3Service : FilingTestServiceBase
{
    private readonly UenstBe3Client client;

    public UenstBe3Service(UenstBe3Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "uenst-anmeldung-be-3";
    public override string RoutePrefix => "api/uenst-anmeldung-be/3";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        UenstBe3CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        UenstBe3CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        UenstBe3FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        UenstBe3FilingRequest request,
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
                ValidateAsync(new UenstBe3CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new UenstBe3CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new UenstBe3FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new UenstBe3FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

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

[FilingTestApp("ust-2022", "api/ust/2022")]
public sealed class Ust2022Service : FilingTestServiceBase
{
    private readonly Ust2022Client client;

    public Ust2022Service(Ust2022Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "ust-2022";
    public override string RoutePrefix => "api/ust/2022";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        Ust2022CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        Ust2022CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        Ust2022FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        Ust2022FilingRequest request,
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
                ValidateAsync(new Ust2022CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new Ust2022CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new Ust2022FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new Ust2022FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("ust-2023", "api/ust/2023")]
public sealed class Ust2023Service : FilingTestServiceBase
{
    private readonly Ust2023Client client;

    public Ust2023Service(Ust2023Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "ust-2023";
    public override string RoutePrefix => "api/ust/2023";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        Ust2023CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        Ust2023CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        Ust2023FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        Ust2023FilingRequest request,
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
                ValidateAsync(new Ust2023CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new Ust2023CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new Ust2023FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new Ust2023FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("ust-2024", "api/ust/2024")]
public sealed class Ust2024Service : FilingTestServiceBase
{
    private readonly Ust2024Client client;

    public Ust2024Service(Ust2024Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "ust-2024";
    public override string RoutePrefix => "api/ust/2024";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        Ust2024CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        Ust2024CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        Ust2024FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        Ust2024FilingRequest request,
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
                ValidateAsync(new Ust2024CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new Ust2024CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new Ust2024FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new Ust2024FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("ust-2025", "api/ust/2025")]
public sealed class Ust2025Service : FilingTestServiceBase
{
    private readonly Ust2025Client client;

    public Ust2025Service(Ust2025Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "ust-2025";
    public override string RoutePrefix => "api/ust/2025";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        Ust2025CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        Ust2025CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        Ust2025FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        Ust2025FilingRequest request,
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
                ValidateAsync(new Ust2025CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new Ust2025CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new Ust2025FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new Ust2025FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("ust-2026", "api/ust/2026")]
public sealed class Ust2026Service : FilingTestServiceBase
{
    private readonly Ust2026Client client;

    public Ust2026Service(Ust2026Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "ust-2026";
    public override string RoutePrefix => "api/ust/2026";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        Ust2026CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        Ust2026CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        Ust2026FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        Ust2026FilingRequest request,
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
                ValidateAsync(new Ust2026CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new Ust2026CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new Ust2026FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new Ust2026FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("ustakfz-einzel-2", "api/ustakfz-einzel/2")]
public sealed class UStAKfz2Service : FilingTestServiceBase
{
    private readonly UStAKfz2Client client;

    public UStAKfz2Service(UStAKfz2Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "ustakfz-einzel-2";
    public override string RoutePrefix => "api/ustakfz-einzel/2";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        UStAKfz2CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        UStAKfz2CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        UStAKfz2FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        UStAKfz2FilingRequest request,
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
                ValidateAsync(new UStAKfz2CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new UStAKfz2CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new UStAKfz2FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new UStAKfz2FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("ustva-2016", "api/ustva/2016")]
public sealed class UstVa2016Service : FilingTestServiceBase
{
    private readonly UstVa2016Client client;

    public UstVa2016Service(UstVa2016Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "ustva-2016";
    public override string RoutePrefix => "api/ustva/2016";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        UstVa2016CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        UstVa2016CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        UstVa2016FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        UstVa2016FilingRequest request,
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
                ValidateAsync(new UstVa2016CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new UstVa2016CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new UstVa2016FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new UstVa2016FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("ustva-2017", "api/ustva/2017")]
public sealed class UstVa2017Service : FilingTestServiceBase
{
    private readonly UstVa2017Client client;

    public UstVa2017Service(UstVa2017Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "ustva-2017";
    public override string RoutePrefix => "api/ustva/2017";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        UstVa2017CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        UstVa2017CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        UstVa2017FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        UstVa2017FilingRequest request,
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
                ValidateAsync(new UstVa2017CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new UstVa2017CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new UstVa2017FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new UstVa2017FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

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

[FilingTestApp("ustva-2019", "api/ustva/2019")]
public sealed class UstVa2019Service : FilingTestServiceBase
{
    private readonly UstVa2019Client client;

    public UstVa2019Service(UstVa2019Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "ustva-2019";
    public override string RoutePrefix => "api/ustva/2019";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        UstVa2019CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        UstVa2019CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        UstVa2019FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        UstVa2019FilingRequest request,
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
                ValidateAsync(new UstVa2019CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new UstVa2019CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new UstVa2019FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new UstVa2019FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("ustva-2020", "api/ustva/2020")]
public sealed class UstVa2020Service : FilingTestServiceBase
{
    private readonly UstVa2020Client client;

    public UstVa2020Service(UstVa2020Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "ustva-2020";
    public override string RoutePrefix => "api/ustva/2020";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        UstVa2020CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        UstVa2020CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        UstVa2020FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        UstVa2020FilingRequest request,
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
                ValidateAsync(new UstVa2020CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new UstVa2020CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new UstVa2020FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new UstVa2020FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("ustva-2021", "api/ustva/2021")]
public sealed class UstVa2021Service : FilingTestServiceBase
{
    private readonly UstVa2021Client client;

    public UstVa2021Service(UstVa2021Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "ustva-2021";
    public override string RoutePrefix => "api/ustva/2021";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        UstVa2021CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        UstVa2021CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        UstVa2021FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        UstVa2021FilingRequest request,
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
                ValidateAsync(new UstVa2021CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new UstVa2021CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new UstVa2021FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new UstVa2021FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("ustva-2022", "api/ustva/2022")]
public sealed class UstVa2022Service : FilingTestServiceBase
{
    private readonly UstVa2022Client client;

    public UstVa2022Service(UstVa2022Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "ustva-2022";
    public override string RoutePrefix => "api/ustva/2022";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        UstVa2022CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        UstVa2022CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        UstVa2022FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        UstVa2022FilingRequest request,
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
                ValidateAsync(new UstVa2022CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new UstVa2022CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new UstVa2022FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new UstVa2022FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("ustva-2023", "api/ustva/2023")]
public sealed class UstVa2023Service : FilingTestServiceBase
{
    private readonly UstVa2023Client client;

    public UstVa2023Service(UstVa2023Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "ustva-2023";
    public override string RoutePrefix => "api/ustva/2023";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        UstVa2023CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        UstVa2023CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        UstVa2023FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        UstVa2023FilingRequest request,
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
                ValidateAsync(new UstVa2023CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new UstVa2023CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new UstVa2023FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new UstVa2023FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("ustva-2024", "api/ustva/2024")]
public sealed class UstVa2024Service : FilingTestServiceBase
{
    private readonly UstVa2024Client client;

    public UstVa2024Service(UstVa2024Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "ustva-2024";
    public override string RoutePrefix => "api/ustva/2024";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        UstVa2024CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        UstVa2024CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        UstVa2024FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        UstVa2024FilingRequest request,
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
                ValidateAsync(new UstVa2024CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new UstVa2024CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new UstVa2024FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new UstVa2024FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("ustva-2025", "api/ustva/2025")]
public sealed class UstVa2025Service : FilingTestServiceBase
{
    private readonly UstVa2025Client client;

    public UstVa2025Service(UstVa2025Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "ustva-2025";
    public override string RoutePrefix => "api/ustva/2025";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        UstVa2025CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        UstVa2025CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        UstVa2025FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        UstVa2025FilingRequest request,
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
                ValidateAsync(new UstVa2025CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new UstVa2025CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new UstVa2025FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new UstVa2025FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("ustva-2026", "api/ustva/2026")]
public sealed class UstVa2026Service : FilingTestServiceBase
{
    private readonly UstVa2026Client client;

    public UstVa2026Service(UstVa2026Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "ustva-2026";
    public override string RoutePrefix => "api/ustva/2026";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        UstVa2026CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        UstVa2026CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        UstVa2026FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        UstVa2026FilingRequest request,
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
                ValidateAsync(new UstVa2026CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new UstVa2026CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new UstVa2026FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new UstVa2026FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("vgn-anmeldung-be-2", "api/vgn-anmeldung-be/2")]
public sealed class VgnBe2Service : FilingTestServiceBase
{
    private readonly VgnBe2Client client;

    public VgnBe2Service(VgnBe2Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "vgn-anmeldung-be-2";
    public override string RoutePrefix => "api/vgn-anmeldung-be/2";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        VgnBe2CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        VgnBe2CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        VgnBe2FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        VgnBe2FilingRequest request,
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
                ValidateAsync(new VgnBe2CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new VgnBe2CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new VgnBe2FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new VgnBe2FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("vgn-anmeldung-hb-2", "api/vgn-anmeldung-hb/2")]
public sealed class VgnHb2Service : FilingTestServiceBase
{
    private readonly VgnHb2Client client;

    public VgnHb2Service(VgnHb2Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "vgn-anmeldung-hb-2";
    public override string RoutePrefix => "api/vgn-anmeldung-hb/2";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        VgnHb2CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        VgnHb2CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        VgnHb2FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        VgnHb2FilingRequest request,
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
                ValidateAsync(new VgnHb2CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new VgnHb2CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new VgnHb2FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new VgnHb2FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("vgn-anmeldung-hh-2", "api/vgn-anmeldung-hh/2")]
public sealed class VgnHh2Service : FilingTestServiceBase
{
    private readonly VgnHh2Client client;

    public VgnHh2Service(VgnHh2Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "vgn-anmeldung-hh-2";
    public override string RoutePrefix => "api/vgn-anmeldung-hh/2";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        VgnHh2CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        VgnHh2CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        VgnHh2FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        VgnHh2FilingRequest request,
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
                ValidateAsync(new VgnHh2CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new VgnHh2CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new VgnHh2FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new VgnHh2FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("vm-ergebnis-liste-vn-sicht-11", "api/vm-ergebnis-liste-vn-sicht/11")]
public sealed class VmErgListeVn11Service : FilingTestServiceBase
{
    private readonly VmErgListeVn11Client client;

    public VmErgListeVn11Service(VmErgListeVn11Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "vm-ergebnis-liste-vn-sicht-11";
    public override string RoutePrefix => "api/vm-ergebnis-liste-vn-sicht/11";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        VmErgListeVn11CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        VmErgListeVn11CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        VmErgListeVn11FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        VmErgListeVn11FilingRequest request,
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
                ValidateAsync(new VmErgListeVn11CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new VmErgListeVn11CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new VmErgListeVn11FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new VmErgListeVn11FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("wtb-anmeldung-hb-2", "api/wtb-anmeldung-hb/2")]
public sealed class WtbHb2Service : FilingTestServiceBase
{
    private readonly WtbHb2Client client;

    public WtbHb2Service(WtbHb2Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "wtb-anmeldung-hb-2";
    public override string RoutePrefix => "api/wtb-anmeldung-hb/2";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        WtbHb2CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        WtbHb2CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        WtbHb2FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        WtbHb2FilingRequest request,
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
                ValidateAsync(new WtbHb2CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new WtbHb2CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new WtbHb2FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new WtbHb2FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}

[FilingTestApp("zmdo-5", "api/zmdo/5")]
public sealed class Zmdo5Service : FilingTestServiceBase
{
    private readonly Zmdo5Client client;

    public Zmdo5Service(Zmdo5Client client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override string AppName => "zmdo-5";
    public override string RoutePrefix => "api/zmdo/5";

    public override Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default) =>
        this.client.GetOpenApiDocumentAsync(cancellationToken);

    public Task<JsonDocument> ValidateAsync(
        Zmdo5CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.ValidateAsync(request, cancellationToken);

    public Task<JsonDocument> PreviewAsync(
        Zmdo5CheckRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.PreviewAsync(request, cancellationToken);

    public Task<JsonDocument> SimulateAsync(
        Zmdo5FilingRequest request,
        CancellationToken cancellationToken = default) =>
        this.client.SimulateAsync(request, cancellationToken);

    public Task<JsonDocument> SubmitAsync(
        Zmdo5FilingRequest request,
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
                ValidateAsync(new Zmdo5CheckRequest(data), cancellationToken),
            (data, cancellationToken) =>
                PreviewAsync(new Zmdo5CheckRequest(data), cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SimulateAsync(
                    new Zmdo5FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: true),
                    cancellationToken),
            (data, certificateBase64, certificatePin, cancellationToken) =>
                SubmitAsync(
                    new Zmdo5FilingRequest(
                        data,
                        certificateBase64,
                        certificatePin,
                        Simulation: false),
                    cancellationToken));
}


