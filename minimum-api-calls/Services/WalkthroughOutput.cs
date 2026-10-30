using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MinimumTaxServices.Samples.Services;

/// <summary>Console output and result files shared by the REST and MCP walkthroughs.</summary>
public static class WalkthroughOutput
{
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

    /// <summary>Writes an XML document returned by the service to <see cref="OutputDirectory"/>.</summary>
    public static async Task SaveXmlAsync(string transport, string fileName, string? xml)
    {
        if (string.IsNullOrEmpty(xml))
        {
            throw new InvalidOperationException($"The service returned no content for {fileName}.");
        }

        var directory = Directory.CreateDirectory(Path.Combine(OutputDirectory, transport));
        var path = Path.Combine(directory.FullName, fileName);
        await File.WriteAllTextAsync(path, xml, new UTF8Encoding(false));
        Console.WriteLine($"✔ Saved {path}");
    }

    /// <summary>Writes a JSON result, such as the typed GIR model, to <see cref="OutputDirectory"/>.</summary>
    public static async Task SaveJsonAsync(string transport, string fileName, JsonNode node)
    {
        var directory = Directory.CreateDirectory(Path.Combine(OutputDirectory, transport));
        var path = Path.Combine(directory.FullName, fileName);
        await File.WriteAllTextAsync(path, node.ToJsonString(JsonOptions));
        Console.WriteLine($"✔ Saved {path}");
    }
}
