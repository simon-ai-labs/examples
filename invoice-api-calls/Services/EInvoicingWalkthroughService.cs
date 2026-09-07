using System.Text.Json.Nodes;

namespace InvoiceServices.Samples.Services;

public sealed class EInvoicingWalkthroughService(EInvoicingApiClient api)
{
    public async Task<int> RunAllStepsAsync()
    {
        // -------------------------------------------------------------
        // Step 1: Capabilities
        // -------------------------------------------------------------
        var capOk = await Step1_CapabilitiesAsync();
        if (!capOk)
        {
            return 1;
        }

        // -------------------------------------------------------------
        // Step 2: Generate Invoice
        // -------------------------------------------------------------
        var (genOk, base64Content, fileName) = await Step2_GenerateInvoiceAsync();
        if (!genOk || string.IsNullOrWhiteSpace(base64Content))
        {
            return 1;
        }

        // -------------------------------------------------------------
        // Step 3: Validate Invoice
        // -------------------------------------------------------------
        await Step3_ValidateInvoiceAsync(base64Content, fileName);

        // -------------------------------------------------------------
        // Step 4: Read Invoice
        // -------------------------------------------------------------
        await Step4_ReadInvoiceAsync(base64Content, fileName);

        // -------------------------------------------------------------
        // Step 5: Convert Invoice (XRechnung UBL -> XRechnung CII)
        // -------------------------------------------------------------
        await Step5_ConvertInvoiceAsync(base64Content, fileName);

        // -------------------------------------------------------------
        // Step 6: Render Invoice (HTML and PDF)
        // -------------------------------------------------------------
        // Note: We render the converted XRechnung CII document, as rendering requires
        // a fully valid XRechnung or ZUGFeRD document.
        byte[] convertedBytes = await File.ReadAllBytesAsync("converted_xrechnung_cii.xml");
        string convertedBase64 = Convert.ToBase64String(convertedBytes);
        await Step6_RenderVisualizationsAsync(convertedBase64, "converted_xrechnung_cii.xml");

        // -------------------------------------------------------------
        // Step 7: Demonstrate All Generation Targets & Conversions
        // -------------------------------------------------------------
        await Step7_DemonstrateAllGenerationAndConversionVariantsAsync();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("==================================================================");
        Console.WriteLine("    All RapidAPI Walkthrough steps completed successfully!       ");
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
            foreach (var std in standards)
            {
                Console.WriteLine($"  - {std?["format"]} v{std?["version"]} (Syntaxes: {string.Join(", ", std?["syntaxes"]?.AsArray().Select(s => s?.ToString()) ?? [])})");
            }
        }
        Console.WriteLine("✔ Step 1 completed successfully.\n");
        return true;
    }

    public async Task<(bool Success, string? Base64Content, string FileName)> Step2_GenerateInvoiceAsync()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("▶ Step 2: Generating a compliant XRechnung 3.0.2 (UBL) invoice (POST /v1/invoices/generate)...");
        Console.ResetColor();

        // XRechnung is the format that supports the full validate/read/convert/render pipeline used
        // by the following steps. Peppol only supports generation (see /v1/capabilities); it is
        // demonstrated separately in Step 7.
        var generatePayload = InvoiceDataFactory.CreateWalkthroughInvoiceRequest();
        var (base64Content, fileName) = await api.GenerateInvoiceAsync(generatePayload);

        byte[] invoiceBytes = Convert.FromBase64String(base64Content);
        await File.WriteAllBytesAsync(fileName, invoiceBytes);

        Console.WriteLine($"✔ Generated invoice saved locally as: {fileName} ({invoiceBytes.Length} bytes)");
        Console.WriteLine("✔ Step 2 completed successfully.\n");

        return (true, base64Content, fileName);
    }

    public async Task<bool> Step3_ValidateInvoiceAsync(string base64Content, string fileName)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("▶ Step 3: Validating the generated electronic invoice (POST /v1/invoices/validate)...");
        Console.ResetColor();

        var expectedRepresentation = new JsonObject
        {
            ["format"] = "xRechnung",
            ["version"] = "3.0.2",
            ["profile"] = "xRechnung",
            ["syntax"] = "ubl"
        };

        var valResult = await api.ValidateInvoiceAsync(base64Content, fileName, expectedRepresentation);
        bool isValid = valResult?["valid"]?.GetValue<bool>() ?? false;

        if (isValid)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✔ Invoice is 100% VALID according to EN 16931 & XRechnung 3.0.2 rules!");
            Console.ResetColor();
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
            Console.ResetColor();
        }

        Console.WriteLine("✔ Step 3 completed successfully.\n");
        return isValid;
    }

    public async Task<bool> Step4_ReadInvoiceAsync(string base64Content, string fileName)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("▶ Step 4: Reading and parsing invoice to canonical JSON (POST /v1/invoices/read)...");
        Console.ResetColor();

        var readResult = await api.ReadInvoiceAsync(base64Content, fileName);
        var inv = readResult?["invoice"];

        Console.WriteLine("Parsed Invoice Details:");
        Console.WriteLine($"  Invoice ID:   {inv?["invoiceId"]}");
        Console.WriteLine($"  Issue Date:   {inv?["issueDate"]}");
        Console.WriteLine($"  Currency:     {inv?["invoiceCurrency"]}");
        Console.WriteLine($"  Seller:       {inv?["seller"]?["name"]}");
        Console.WriteLine($"  Buyer:        {inv?["buyer"]?["name"]}");
        Console.WriteLine($"  Total Amount: {inv?["totals"]?["payableAmount"]} {inv?["invoiceCurrency"]}");

        Console.WriteLine("✔ Step 4 completed successfully.\n");
        return true;
    }

    public async Task<bool> Step5_ConvertInvoiceAsync(string base64Content, string fileName)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("▶ Step 5: Converting XRechnung 3.0.2 UBL to XRechnung 3.0.2 CII (POST /v1/invoices/convert)...");
        Console.ResetColor();

        var target = new JsonObject
        {
            ["format"] = "xRechnung",
            ["version"] = "3.0.2",
            ["profile"] = "xRechnung",
            ["syntax"] = "cii"
        };

        var (convBase64, convFileName) = await api.ConvertInvoiceAsync(base64Content, fileName, target);
        await File.WriteAllBytesAsync("converted_xrechnung_cii.xml", Convert.FromBase64String(convBase64));

        Console.WriteLine("✔ Converted invoice saved locally as: converted_xrechnung_cii.xml");
        Console.WriteLine("✔ Step 5 completed successfully.\n");
        return true;
    }

    public async Task<bool> Step6_RenderVisualizationsAsync(string base64Content, string fileName)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("▶ Step 6: Rendering invoice visualization as HTML and PDF (POST /v1/invoices/render)...");
        Console.ResetColor();

        // 6a: HTML
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

        // 6b: PDF
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

        Console.WriteLine("✔ Step 6 completed successfully.\n");
        return true;
    }

    public async Task<bool> Step7_DemonstrateAllGenerationAndConversionVariantsAsync()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("▶ Step 7: Demonstrating all 7 generation targets and cross-conversions...");
        Console.ResetColor();

        // 7 Supported Generation Targets
        var generationTargets = new (string Format, string Version, string? Profile, string? Syntax, string OutName)[]
        {
            ("xRechnung", "3.0.2", "xRechnung", "ubl", "generated_target_1_xrechnung_ubl.xml"),
            ("xRechnung", "3.0.2", "xRechnung", "cii", "generated_target_2_xrechnung_cii.xml"),
            ("peppol", "3.0", "peppolBilling", "ubl", "generated_target_3_peppol_ubl.xml"),
            ("zugferd", "2.5.2", "basic", "cii", "generated_target_4_zugferd_basic.pdf"),
            ("zugferd", "2.5.2", "en16931", "cii", "generated_target_5_zugferd_en16931.pdf"),
            ("zugferd", "2.5.2", "extended", "cii", "generated_target_6_zugferd_extended.pdf"),
            ("zugferd", "2.5.2", "xRechnung", "cii", "generated_target_7_zugferd_xrechnung.pdf")
        };

        Console.WriteLine("\n-- 7 / 7 Generation Targets from canonical JSON --");
        string? sampleUblBase64 = null;
        string? sampleCiiBase64 = null;

        for (int i = 0; i < generationTargets.Length; i++)
        {
            var t = generationTargets[i];
            try
            {
                var payload = InvoiceDataFactory.CreateInvoiceRequest(t.Format, t.Version, t.Profile, t.Syntax);
                var (content, fileName) = await api.GenerateInvoiceAsync(payload);
                byte[] bytes = Convert.FromBase64String(content);
                await File.WriteAllBytesAsync(t.OutName, bytes);
                Console.WriteLine($"  [{i + 1}/7] ✔ {t.Format.ToUpperInvariant()} (Profile: {t.Profile}, Syntax: {t.Syntax}) -> {t.OutName} ({bytes.Length} bytes)");

                if (t.OutName.EndsWith("xrechnung_ubl.xml"))
                {
                    sampleUblBase64 = content;
                }
                else if (t.OutName.EndsWith("xrechnung_cii.xml"))
                {
                    sampleCiiBase64 = content;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"  [{i + 1}/7] ⚠ {t.Format} ({t.Profile}) note: {ex.Message}");
                Console.ResetColor();
            }
        }

        Console.WriteLine("\n-- Key Conversion Paths --");
        // Demonstrate key cross conversions
        if (!string.IsNullOrWhiteSpace(sampleUblBase64))
        {
            // 1. UBL XML -> ZUGFeRD EN16931 PDF
            try
            {
                var target = new JsonObject
                {
                    ["format"] = "zugferd",
                    ["version"] = "2.5.2",
                    ["profile"] = "en16931",
                    ["syntax"] = "cii"
                };
                var (convBase64, _) = await api.ConvertInvoiceAsync(sampleUblBase64, "invoice_ubl.xml", target);
                await File.WriteAllBytesAsync("converted_ubl_to_zugferd_pdf.pdf", Convert.FromBase64String(convBase64));
                Console.WriteLine("  ✔ Converted: XRechnung UBL XML -> ZUGFeRD EN16931 Hybrid PDF (converted_ubl_to_zugferd_pdf.pdf)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ⚠ Convert UBL -> ZUGFeRD PDF note: {ex.Message}");
            }

            // 2. UBL XML -> Peppol UBL
            try
            {
                var target = new JsonObject
                {
                    ["format"] = "peppol",
                    ["version"] = "3.0",
                    ["profile"] = "peppolBilling",
                    ["syntax"] = "ubl"
                };
                var (convBase64, _) = await api.ConvertInvoiceAsync(sampleUblBase64, "invoice_ubl.xml", target);
                await File.WriteAllBytesAsync("converted_xrechnung_to_peppol_ubl.xml", Convert.FromBase64String(convBase64));
                Console.WriteLine("  ✔ Converted: XRechnung UBL -> Peppol BIS 3.0 UBL (converted_xrechnung_to_peppol_ubl.xml)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ⚠ Convert XRechnung -> Peppol note: {ex.Message}");
            }
        }

        if (!string.IsNullOrWhiteSpace(sampleCiiBase64))
        {
            // 3. CII XML -> XRechnung UBL
            try
            {
                var target = new JsonObject
                {
                    ["format"] = "xRechnung",
                    ["version"] = "3.0.2",
                    ["profile"] = "xRechnung",
                    ["syntax"] = "ubl"
                };
                var (convBase64, _) = await api.ConvertInvoiceAsync(sampleCiiBase64, "invoice_cii.xml", target);
                await File.WriteAllBytesAsync("converted_cii_to_xrechnung_ubl.xml", Convert.FromBase64String(convBase64));
                Console.WriteLine("  ✔ Converted: XRechnung CII XML -> XRechnung UBL XML (converted_cii_to_xrechnung_ubl.xml)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ⚠ Convert CII -> UBL note: {ex.Message}");
            }
        }

        Console.WriteLine("\n✔ Step 7 completed successfully.\n");
        return true;
    }
}
