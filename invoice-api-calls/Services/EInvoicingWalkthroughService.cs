using System.Text;
using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace InvoiceServices.Samples.Services;

public sealed class EInvoicingWalkthroughService(EInvoicingApiClient api)
{
    private const string OfficialTestInvoiceFileName = "01.06_minimal_test_ubl.xml";

    public async Task<int> RunAllStepsAsync()
    {
        var capOk = await Step1_CapabilitiesAsync();
        if (!capOk)
        {
            return 1;
        }

        var (base64Content, fileName) = await Step2_LoadOfficialTestInvoiceAsync();
        await Step3_ValidateInvoiceAsync(base64Content, fileName);
        var schematronTestPassed = await Step4_ValidateExpectedSchematronFailureAsync(base64Content);
        if (!schematronTestPassed)
        {
            return 1;
        }

        await Step5_ReadInvoiceAsync(base64Content, fileName);

        var convertedBase64 = await Step6_ConvertInvoiceAsync(base64Content, fileName);
        await Step7_RenderVisualizationsAsync(convertedBase64, "converted_xrechnung_cii.xml");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("==================================================================");
        Console.WriteLine("  Walkthrough completed with the official XRechnung test invoice! ");
        Console.WriteLine("==================================================================");
        Console.ResetColor();
        return 0;
    }

    public async Task<bool> Step1_CapabilitiesAsync()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("▶ Step 1: Checking API capabilities (GET /v1/capabilities)...");
        Console.ResetColor();

        var capJson = await api.GetCapabilitiesAsync();
        Console.WriteLine($"API Version: {capJson?["apiVersion"]}");
        if (capJson?["standards"] is JsonArray standards)
        {
            Console.WriteLine("Supported Standards:");
            foreach (var standard in standards)
            {
                Console.WriteLine($"  - {standard?["format"]} v{standard?["version"]} (Syntaxes: {string.Join(", ", standard?["syntaxes"]?.AsArray().Select(s => s?.ToString()) ?? [])})");
            }
        }

        Console.WriteLine("✔ Step 1 completed successfully.\n");
        return true;
    }

    public async Task<(string Base64Content, string FileName)> Step2_LoadOfficialTestInvoiceAsync()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("▶ Step 2: Loading the official KoSIT/XRechnung UBL test invoice...");
        Console.ResetColor();

        var path = Path.Combine(AppContext.BaseDirectory, "TestInvoices", OfficialTestInvoiceFileName);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("The bundled official XRechnung test invoice is missing.", path);
        }

        var invoiceBytes = await File.ReadAllBytesAsync(path);
        Console.WriteLine($"✔ Loaded official test invoice: {OfficialTestInvoiceFileName} ({invoiceBytes.Length} bytes)");
        Console.WriteLine("✔ Step 2 completed successfully.\n");
        return (Convert.ToBase64String(invoiceBytes), OfficialTestInvoiceFileName);
    }

    public async Task<bool> Step3_ValidateInvoiceAsync(string base64Content, string fileName)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("▶ Step 3: Validating the official test invoice (POST /v1/invoices/validate)...");
        Console.ResetColor();

        var expectedRepresentation = new JsonObject
        {
            ["format"] = "xRechnung",
            ["version"] = "3.0.2",
            ["profile"] = "xRechnung",
            ["syntax"] = "ubl"
        };

        var valResult = await api.ValidateInvoiceAsync(base64Content, fileName, expectedRepresentation);
        var isValid = valResult?["valid"]?.GetValue<bool>() ?? false;
        if (isValid)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✔ Invoice is valid according to EN 16931 and XRechnung rules.");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Validation issues detected:");
            if (valResult?["issues"] is JsonArray issues)
            {
                foreach (var issue in issues)
                {
                    Console.WriteLine($"  - [{issue?["severity"]}] {issue?["ruleId"]}: {issue?["message"]} (Path: {issue?["businessPath"]})");
                }
            }
        }

        Console.ResetColor();
        Console.WriteLine("✔ Step 3 completed successfully.\n");
        return isValid;
    }

    public async Task<bool> Step4_ValidateExpectedSchematronFailureAsync(string validInvoiceBase64)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("▶ Step 4: Checking the expected Schematron failure BR-DE-15 (BT-10)...");
        Console.ResetColor();

        // BR-DE-15 requires BuyerReference (BT-10). Removing it leaves the UBL document
        // schema-valid, so the validation error is specifically produced by Schematron.
        var document = XDocument.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(validInvoiceBase64)));
        var cbc = (XNamespace)"urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
        var buyerReference = document.Root?.Element(cbc + "BuyerReference")
            ?? throw new InvalidOperationException("The reference test invoice has no BuyerReference to remove.");
        buyerReference.Remove();

        var invalidBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(document.Declaration is null
            ? document.ToString()
            : document.Declaration + Environment.NewLine + document));
        var expectedRepresentation = new JsonObject
        {
            ["format"] = "xRechnung",
            ["version"] = "3.0.2",
            ["profile"] = "xRechnung",
            ["syntax"] = "ubl"
        };

        var validationResult = await api.ValidateInvoiceAsync(
            invalidBase64,
            "01.06_missing_buyer_reference_ubl.xml",
            expectedRepresentation);
        var failedRuleIds = validationResult?["issues"]?.AsArray()
            .Select(issue => issue?["ruleId"]?.GetValue<string>())
            .Where(ruleId => !string.IsNullOrWhiteSpace(ruleId))
            .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? [];

        var isExpectedFailure = !(validationResult?["valid"]?.GetValue<bool>() ?? false)
            && failedRuleIds.Contains("BR-DE-15");
        if (isExpectedFailure)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✔ Expected Schematron failure confirmed: BR-DE-15 requires BuyerReference (BT-10).");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✘ Expected BR-DE-15, received: {string.Join(", ", failedRuleIds)}");
        }

        Console.ResetColor();
        Console.WriteLine("✔ Step 4 completed successfully.\n");
        return isExpectedFailure;
    }

    public async Task<bool> Step5_ReadInvoiceAsync(string base64Content, string fileName)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("▶ Step 5: Reading the official test invoice (POST /v1/invoices/read)...");
        Console.ResetColor();

        var readResult = await api.ReadInvoiceAsync(base64Content, fileName);
        var invoice = readResult?["invoice"];
        Console.WriteLine("Parsed Invoice Details:");
        Console.WriteLine($"  Invoice ID:   {invoice?["invoiceId"]}");
        Console.WriteLine($"  Issue Date:   {invoice?["issueDate"]}");
        Console.WriteLine($"  Currency:     {invoice?["invoiceCurrency"]}");
        Console.WriteLine($"  Seller:       {invoice?["seller"]?["name"]}");
        Console.WriteLine($"  Buyer:        {invoice?["buyer"]?["name"]}");
        Console.WriteLine($"  Total Amount: {invoice?["totals"]?["payableAmount"]} {invoice?["invoiceCurrency"]}");
        Console.WriteLine("✔ Step 5 completed successfully.\n");
        return true;
    }

    public async Task<string> Step6_ConvertInvoiceAsync(string base64Content, string fileName)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("▶ Step 6: Converting the official XRechnung UBL test invoice to CII (POST /v1/invoices/convert)...");
        Console.ResetColor();

        var target = new JsonObject
        {
            ["format"] = "xRechnung",
            ["version"] = "3.0.2",
            ["profile"] = "xRechnung",
            ["syntax"] = "cii"
        };

        var (convertedBase64, _) = await api.ConvertInvoiceAsync(base64Content, fileName, target);
        await File.WriteAllBytesAsync("converted_xrechnung_cii.xml", Convert.FromBase64String(convertedBase64));
        Console.WriteLine("✔ Converted document saved locally as: converted_xrechnung_cii.xml");
        Console.WriteLine("✔ Step 6 completed successfully.\n");
        return convertedBase64;
    }

    public async Task<bool> Step7_RenderVisualizationsAsync(string base64Content, string fileName)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("▶ Step 7: Rendering the converted test invoice as HTML and PDF (POST /v1/invoices/render)...");
        Console.ResetColor();

        try
        {
            var htmlBase64 = await api.RenderInvoiceAsync(base64Content, fileName, "html");
            await File.WriteAllBytesAsync("rendered_invoice.html", Convert.FromBase64String(htmlBase64));
            Console.WriteLine("✔ HTML visualization saved as: rendered_invoice.html");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"HTML render note: {ex.Message}");
        }

        try
        {
            var pdfBase64 = await api.RenderInvoiceAsync(base64Content, fileName, "pdf");
            await File.WriteAllBytesAsync("rendered_invoice.pdf", Convert.FromBase64String(pdfBase64));
            Console.WriteLine("✔ PDF visualization saved as: rendered_invoice.pdf");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PDF render note: {ex.Message}");
        }

        Console.WriteLine("✔ Step 7 completed successfully.\n");
        return true;
    }
}
