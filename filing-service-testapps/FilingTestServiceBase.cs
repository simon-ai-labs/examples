using System.Text.Json;
using System.Text.Json.Nodes;

namespace FilingService.TestApps;

public abstract class FilingTestServiceBase(FilingApiClientBase client)
{
    protected FilingApiClientBase Client { get; } = client ?? throw new ArgumentNullException(nameof(client));
    public abstract string AppName { get; }

    public async Task<int> RunAsync(
        FilingTestOperation operation,
        string? payloadPath,
        string? certificatePath,
        string? pin)
    {
        if (operation == FilingTestOperation.Contract)
        {
            using var document = await Client.GetOpenApiDocumentAsync();
            var root = document.RootElement;
            var pathCount = root.TryGetProperty("paths", out var paths) ? paths.EnumerateObject().Count() : 0;
            var schemaCount = root.TryGetProperty("components", out var components)
                && components.TryGetProperty("schemas", out var schemas)
                ? schemas.EnumerateObject().Count()
                : 0;
            Console.WriteLine($"{AppName}: {Client.OpenApiPath} ({pathCount} paths, {schemaCount} schemas)");
            return 0;
        }

        if (string.IsNullOrWhiteSpace(payloadPath))
        {
            throw new ArgumentException($"--payload is required for {operation}.");
        }

        var payload = await LoadPayloadAsync(payloadPath);
        using var response = operation switch
        {
            FilingTestOperation.Validate => await Client.ValidateAsync(payload),
            FilingTestOperation.Preview => await Client.PreviewAsync(payload),
            FilingTestOperation.Simulate => await Client.SimulateAsync(
                payload,
                await LoadCertificateAsync(certificatePath),
                RequirePin(pin)),
            FilingTestOperation.Submit => await Client.SubmitAsync(
                payload,
                await LoadCertificateAsync(certificatePath),
                RequirePin(pin)),
            _ => throw new ArgumentOutOfRangeException(nameof(operation))
        };

        Console.WriteLine(JsonSerializer.Serialize(response.RootElement, new JsonSerializerOptions { WriteIndented = true }));
        return 0;
    }

    private static async Task<JsonNode> LoadPayloadAsync(string path)
    {
        await using var stream = File.OpenRead(path);
        return await JsonNode.ParseAsync(stream)
            ?? throw new InvalidDataException($"Payload file '{path}' is empty or invalid JSON.");
    }

    private static async Task<string> LoadCertificateAsync(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("--certificate is required for simulate/submit.");
        }

        return Convert.ToBase64String(await File.ReadAllBytesAsync(path));
    }

    private static string RequirePin(string? pin) =>
        !string.IsNullOrEmpty(pin) ? pin : throw new ArgumentException("--pin or FILING_SERVICE_PIN is required.");
}

public abstract class FilingTestService<TClient>(TClient client) : FilingTestServiceBase(client)
    where TClient : FilingApiClientBase
{
}
