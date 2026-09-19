using System.Text.Json;
using System.Text.Json.Nodes;

namespace FilingService.TestApps;

public abstract class FilingTestServiceBase
{
    public abstract string AppName { get; }
    public abstract string RoutePrefix { get; }
    public string OpenApiPath => $"openapi/{AppName}.json";

    public abstract Task<JsonDocument> GetOpenApiDocumentAsync(CancellationToken cancellationToken = default);

    public abstract Task<int> RunAsync(
        FilingTestOperation operation,
        string? payloadPath,
        string? certificatePath,
        string? pin);

    protected async Task<int> ExecuteAsync(
        FilingTestOperation operation,
        string? payloadPath,
        string? certificatePath,
        string? pin,
        Func<JsonNode, CancellationToken, Task<JsonDocument>> validate,
        Func<JsonNode, CancellationToken, Task<JsonDocument>> preview,
        Func<JsonNode, string, string, CancellationToken, Task<JsonDocument>> simulate,
        Func<JsonNode, string, string, CancellationToken, Task<JsonDocument>> submit)
    {
        if (operation == FilingTestOperation.Contract)
        {
            using var document = await GetOpenApiDocumentAsync();
            var root = document.RootElement;
            var pathCount = root.TryGetProperty("paths", out var paths) ? paths.EnumerateObject().Count() : 0;
            var schemaCount = root.TryGetProperty("components", out var components)
                && components.TryGetProperty("schemas", out var schemas)
                ? schemas.EnumerateObject().Count()
                : 0;
            Console.WriteLine($"{AppName}: {OpenApiPath} ({pathCount} paths, {schemaCount} schemas)");
            return 0;
        }

        if (string.IsNullOrWhiteSpace(payloadPath))
        {
            throw new ArgumentException($"--payload is required for {operation}.");
        }

        var data = await LoadPayloadAsync(payloadPath);
        var response = operation switch
        {
            FilingTestOperation.Validate => await validate(data, CancellationToken.None),
            FilingTestOperation.Preview => await preview(data, CancellationToken.None),
            FilingTestOperation.Simulate => await simulate(
                data,
                await LoadCertificateAsync(certificatePath),
                RequirePin(pin),
                CancellationToken.None),
            FilingTestOperation.Submit => await submit(
                data,
                await LoadCertificateAsync(certificatePath),
                RequirePin(pin),
                CancellationToken.None),
            _ => throw new ArgumentOutOfRangeException(nameof(operation))
        };

        using (response)
        {
            Console.WriteLine(JsonSerializer.Serialize(
                response.RootElement,
                new JsonSerializerOptions { WriteIndented = true }));
        }

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
        !string.IsNullOrEmpty(pin)
            ? pin
            : throw new ArgumentException("--pin or FILING_SERVICE_PIN is required.");
}

