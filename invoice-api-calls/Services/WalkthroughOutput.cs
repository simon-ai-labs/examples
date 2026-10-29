using System.Text.Json;
using System.Text.Json.Nodes;

namespace InvoiceServices.Samples.Services;

/// <summary>Console output and result files shared by the REST and MCP walkthroughs.</summary>
public static class WalkthroughOutput
{
    private const int MaxStringLength = 120;
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public static string OutputDirectory { get; } = Path.Combine(Directory.GetCurrentDirectory(), "output");

    public static void Step(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n▶ {message}");
        Console.ResetColor();
    }

    public static void Write(ConsoleColor color, string message)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    /// <summary>Prints a result with long strings such as Base64 documents shortened.</summary>
    public static void Json(JsonNode? node) => Console.WriteLine(Shorten(node)?.ToJsonString(JsonOptions) ?? "null");

    /// <summary>Writes a Base64 document from a result to <see cref="OutputDirectory"/>.</summary>
    public static async Task SaveAsync(string transport, string fileName, string? base64Content)
    {
        if (string.IsNullOrEmpty(base64Content))
        {
            throw new InvalidOperationException($"The service returned no content for {fileName}.");
        }

        var directory = Directory.CreateDirectory(Path.Combine(OutputDirectory, transport));
        var path = Path.Combine(directory.FullName, Path.GetFileName(fileName));
        var bytes = Convert.FromBase64String(base64Content);
        await File.WriteAllBytesAsync(path, bytes);
        Console.WriteLine($"✔ Saved {path} ({bytes.Length:N0} bytes)");
    }

    private static JsonNode? Shorten(JsonNode? node) => node switch
    {
        JsonObject obj => new JsonObject(obj.Select(property =>
            KeyValuePair.Create(property.Key, Shorten(property.Value)))),
        JsonArray array => new JsonArray([.. array.Select(Shorten)]),
        JsonValue value when value.TryGetValue<string>(out var text) && text.Length > MaxStringLength =>
            JsonValue.Create($"{text[..MaxStringLength]}… ({text.Length:N0} chars)"),
        _ => node?.DeepClone()
    };
}
