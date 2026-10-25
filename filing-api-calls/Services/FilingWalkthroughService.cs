using System.Text.Json.Nodes;

namespace FilingServices.Samples.Services;

public sealed class FilingWalkthroughService(FilingApiClient api, string? certificatePath, string? certificatePin)
{
    private const string RoutePrefix = "/api/ustva/2026";
    private const string ContractName = "ustva-2026";
    private const string TestFilingFileName = "ustva-2026.json";
    private const string PreviewPdfFileName = "ustva-2026-preview.pdf";

    public async Task<int> RunAllStepsAsync()
    {
        if (!await Step1_ReadinessAsync())
        {
            return 1;
        }

        await Step2_DiscoverContractsAsync();
        var data = await Step3_LoadTestFilingAsync();

        if (!await Step4_ValidateAsync(data))
        {
            return 1;
        }

        if (!await Step5_ValidateExpectedFailureAsync(data))
        {
            return 1;
        }

        await Step6_PreviewAsync(data);
        await Step7_SimulateFilingAsync(data);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("==================================================================");
        Console.WriteLine("  Walkthrough completed with the UStVA 2026 test filing!          ");
        Console.WriteLine("==================================================================");
        Console.ResetColor();
        return 0;
    }

    public async Task<bool> Step1_ReadinessAsync()
    {
        WriteStep("▶ Step 1: Checking service readiness (GET /health/ready)...");

        var (ready, body) = await api.GetReadinessAsync();
        if (!ready)
        {
            // A container that is not ready answers every filing endpoint with 503.
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✘ The service is not ready: {body}");
            Console.ResetColor();
            return false;
        }

        Console.WriteLine($"Readiness: {body}");
        Console.WriteLine("✔ Step 1 completed successfully.\n");
        return true;
    }

    public async Task Step2_DiscoverContractsAsync()
    {
        WriteStep("▶ Step 2: Discovering the OpenAPI contracts (GET /openapi/index.json)...");

        var index = await api.GetOpenApiIndexAsync();
        var documents = index?["documents"]?.AsArray() ?? [];
        Console.WriteLine($"The service publishes {documents.Count} Datenart/Stand contracts, e.g.:");
        foreach (var document in documents.Take(5))
        {
            Console.WriteLine($"  - {document?["name"]} → {document?["path"]}");
        }

        var ustva = documents.FirstOrDefault(d => d?["name"]?.GetValue<string>() == ContractName);
        Console.WriteLine(ustva is null
            ? $"Notice: '{ContractName}' is not listed by this deployment."
            : $"Contract used by this walkthrough: {ustva["path"]}");
        Console.WriteLine("✔ Step 2 completed successfully.\n");
    }

    public async Task<JsonNode> Step3_LoadTestFilingAsync()
    {
        WriteStep("▶ Step 3: Loading the synthetic UStVA 2026 test filing...");

        var path = Path.Combine(AppContext.BaseDirectory, "TestFilings", TestFilingFileName);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("The bundled UStVA test filing is missing.", path);
        }

        var data = JsonNode.Parse(await File.ReadAllTextAsync(path))
            ?? throw new InvalidOperationException("The bundled UStVA test filing is empty.");
        var voranmeldung = data["steuerfall"]?["umsatzsteuervoranmeldung"];
        Console.WriteLine($"✔ Loaded {TestFilingFileName}: Zeitraum {voranmeldung?["jahr"]}/{voranmeldung?["zeitraum"]}, "
            + $"Kz81 = {voranmeldung?["kz81"]}, Kz83 = {voranmeldung?["kz83"]}");
        Console.WriteLine("✔ Step 3 completed successfully.\n");
        return data;
    }

    public async Task<bool> Step4_ValidateAsync(JsonNode data)
    {
        WriteStep($"▶ Step 4: Validating the filing (POST {RoutePrefix}/validate)...");

        var result = await api.ValidateAsync(RoutePrefix, data);
        var hasError = result?["error"]?.GetValue<bool>() ?? true;
        Console.WriteLine($"Document type: {result?["documentType"]}, data type version: {result?["dataTypeVersion"]}");
        PrintFeedback(result);

        if (hasError)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("✘ The test filing was expected to pass the schema and plausibility checks.");
            Console.ResetColor();
            return false;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✔ The filing passed the ELSTER schema and plausibility checks.");
        Console.ResetColor();
        Console.WriteLine("✔ Step 4 completed successfully.\n");
        return true;
    }

    public async Task<bool> Step5_ValidateExpectedFailureAsync(JsonNode validData)
    {
        WriteStep("▶ Step 5: Validating an invalid variant (four-digit PLZ) and expecting findings...");

        var invalidData = validData.DeepClone();
        invalidData["steuerfall"]!["unternehmer"]!["plz"] = "0815";

        // A filing the service rejects on the caller's behalf is not an HTTP failure:
        // it answers 200 with "error": true and the findings in "feedback".
        var result = await api.ValidateAsync(RoutePrefix, invalidData);
        var hasError = result?["error"]?.GetValue<bool>() ?? false;
        PrintFeedback(result);

        if (!hasError)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("✘ The invalid variant was accepted, but findings were expected.");
            Console.ResetColor();
            return false;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✔ The invalid variant was rejected as expected.");
        Console.ResetColor();
        Console.WriteLine("✔ Step 5 completed successfully.\n");
        return true;
    }

    public async Task Step6_PreviewAsync(JsonNode data)
    {
        WriteStep($"▶ Step 6: Rendering the PDF preview (POST {RoutePrefix}/preview)...");

        var result = await api.PreviewAsync(RoutePrefix, data);
        PrintFeedback(result);

        var content = result?["pdf"]?["content"]?.GetValue<string>();
        if (string.IsNullOrEmpty(content))
        {
            Console.WriteLine("Notice: The service returned no PDF for this filing.");
        }
        else
        {
            var bytes = Convert.FromBase64String(content);
            await File.WriteAllBytesAsync(PreviewPdfFileName, bytes);
            Console.WriteLine($"✔ Saved {PreviewPdfFileName} ({bytes.Length} bytes)");
        }

        Console.WriteLine("✔ Step 6 completed successfully.\n");
    }

    public async Task Step7_SimulateFilingAsync(JsonNode data)
    {
        WriteStep($"▶ Step 7: Test transmission to ELSTER (POST {RoutePrefix}/filings, simulation: true)...");

        if (string.IsNullOrWhiteSpace(certificatePath) || string.IsNullOrWhiteSpace(certificatePin))
        {
            Console.WriteLine("Skipped: set FILING_CERTIFICATE_PATH and FILING_CERTIFICATE_PIN to an ELSTER test certificate");
            Console.WriteLine("(PKCS#12 file and PIN) to run this step.\n");
            return;
        }

        var pfx = await File.ReadAllBytesAsync(certificatePath);

        // Do not wrap this call in a retry policy. A failure after the native send began has an
        // unknown outcome and must be reconciled, not resent.
        var result = await api.SimulateFilingAsync(RoutePrefix, data, pfx, certificatePin);
        PrintFeedback(result);

        var protocol = result?["transmissionProtocol"];
        Console.WriteLine($"Transmitted: {protocol?["transmitted"]}, simulated: {protocol?["simulated"]}, "
            + $"test marker: {protocol?["testMarker"]}");
        Console.WriteLine($"Return code: {protocol?["ericReturnCode"]}, transfer ticket: {protocol?["transferTicket"]}");
        Console.WriteLine($"Completed at: {protocol?["completedAt"]}");

        if (result?["error"]?.GetValue<bool>() ?? true)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("ELSTER reported findings for the test transmission (see above).");
            Console.ResetColor();
        }

        Console.WriteLine("✔ Step 7 completed.\n");
    }

    private static void PrintFeedback(JsonNode? result)
    {
        if (result?["feedback"] is not JsonArray feedback || feedback.Count == 0)
        {
            Console.WriteLine("No findings.");
            return;
        }

        Console.WriteLine($"Findings ({feedback.Count}):");
        foreach (var item in feedback)
        {
            var field = item?["field"] is { } f ? $" (Field: {f})" : string.Empty;
            Console.WriteLine($"  - [{item?["severity"]}/{item?["origin"]}] {item?["code"]}: {item?["message"]}{field}");
        }
    }

    private static void WriteStep(string text)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(text);
        Console.ResetColor();
    }
}
