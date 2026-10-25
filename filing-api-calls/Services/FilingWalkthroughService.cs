using System.Text.Json.Nodes;

namespace FilingServices.Samples.Services;

public sealed class FilingWalkthroughService(FilingApiClient api, string? certificatePath, string? certificatePin)
{
    private const string Route = "/api/ustva/2026";

    public async Task<int> RunAsync()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "TestFilings", "ustva-2026.json");
        var data = JsonNode.Parse(await File.ReadAllTextAsync(path))!;

        Step("Step 1: Validating the UStVA 2026 test filing");
        var result = await api.PostAsync($"{Route}/validate", new JsonObject { ["data"] = data.DeepClone() });
        PrintResult(result);

        Step("Step 2: Validating an invalid variant (four-digit PLZ)");
        var invalid = data.DeepClone();
        invalid["steuerfall"]!["unternehmer"]!["plz"] = "0815";
        PrintResult(await api.PostAsync($"{Route}/validate", new JsonObject { ["data"] = invalid }));

        Step("Step 3: Rendering the PDF preview");
        result = await api.PostAsync($"{Route}/preview", new JsonObject { ["data"] = data.DeepClone() });
        PrintResult(result);
        if (result?["pdf"]?["content"]?.GetValue<string>() is { Length: > 0 } pdf)
        {
            await File.WriteAllBytesAsync("ustva-2026-preview.pdf", Convert.FromBase64String(pdf));
            Console.WriteLine("Saved ustva-2026-preview.pdf");
        }

        if (string.IsNullOrWhiteSpace(certificatePath) || string.IsNullOrWhiteSpace(certificatePin))
        {
            Console.WriteLine("\nStep 4 skipped: set FILING_CERTIFICATE_PATH and FILING_CERTIFICATE_PIN for a test transmission.");
            return 0;
        }

        // simulation: true sends the filing to ELSTER as a test case. Do not retry this call
        // automatically: after a failure the delivery may already have reached ELSTER.
        Step("Step 4: Test transmission to ELSTER (simulation: true)");
        result = await api.PostAsync($"{Route}/filings", new JsonObject
        {
            ["data"] = data.DeepClone(),
            ["certificate"] = new JsonObject
            {
                ["pfx"] = Convert.ToBase64String(await File.ReadAllBytesAsync(certificatePath)),
                ["pin"] = certificatePin
            },
            ["simulation"] = true
        });
        PrintResult(result);
        Console.WriteLine($"Transfer ticket: {result?["transmissionProtocol"]?["transferTicket"]}");
        return 0;
    }

    private static void PrintResult(JsonNode? result)
    {
        var error = result?["error"]?.GetValue<bool>() ?? true;
        Console.WriteLine(error ? "Rejected:" : "Valid.");
        foreach (var item in result?["feedback"]?.AsArray() ?? [])
        {
            Console.WriteLine($"  - [{item?["severity"]}] {item?["code"]}: {item?["message"]}");
        }
    }

    private static void Step(string text) => Console.WriteLine($"\n▶ {text}");
}
