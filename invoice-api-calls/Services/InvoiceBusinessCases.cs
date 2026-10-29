using System.Text;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using static InvoiceServices.Samples.Services.WalkthroughOutput;

namespace InvoiceServices.Samples.Services;

/// <summary>
/// The business cases of the E-Invoicing Service, run identically over REST and MCP. Standard versions
/// are taken from the capabilities at runtime, so the walkthrough follows the service's pinned versions.
/// </summary>
public sealed class InvoiceBusinessCases(IInvoiceTransport transport, string invoicePath, string? canonicalInvoiceJsonPath)
{
    private JsonNode? _capabilities;

    public async Task<int> RunAsync()
    {
        await QueryCapabilitiesAsync();

        var invoice = await File.ReadAllBytesAsync(invoicePath);
        var fileName = Path.GetFileName(invoicePath);
        Console.WriteLine($"\nIncoming invoice: {fileName} ({invoice.Length:N0} bytes)");

        if (!await CheckIncomingInvoiceAsync(invoice, fileName))
        {
            return 1;
        }

        var canonicalInvoice = await ReadIncomingInvoiceAsync(invoice, fileName);
        await GenerateOutgoingInvoicesAsync(canonicalInvoice);
        var converted = await ConvertFormatAsync(invoice, fileName);
        await RenderForPeopleAsync(converted);

        Write(ConsoleColor.Green, $"\n✔ All business cases completed over {transport.Name.ToUpperInvariant()}.");
        return 0;
    }

    /// <summary>Integrators: which formats, profiles and combinations each operation supports.</summary>
    private async Task QueryCapabilitiesAsync()
    {
        Case("Query capabilities", "GET /v1/capabilities · invoice_capabilities");
        _capabilities = await transport.CallAsync("capabilities", new Dictionary<string, object?>());
        Console.WriteLine($"API version: {_capabilities?["apiVersion"]}");
        foreach (var standard in _capabilities?["standards"]?.AsArray() ?? [])
        {
            Console.WriteLine($"  {standard?["format"]} {standard?["version"]}  syntaxes: {List(standard?["syntaxes"])}  " +
                              $"in: {List(standard?["inputTypes"])}  out: {List(standard?["outputTypes"])}");
            foreach (var operation in standard?["operationProfiles"]?.AsArray() ?? [])
            {
                Console.WriteLine($"    {operation?["operation"],-9} {List(operation?["profiles"])}");
            }
        }
    }

    /// <summary>
    /// Accounts payable: is the delivered invoice formally and semantically correct? The result names rule
    /// ID and BT path, so a rejected invoice can be sent back with a reason.
    /// </summary>
    private async Task<bool> CheckIncomingInvoiceAsync(byte[] invoice, string fileName)
    {
        Case("Check an incoming invoice", "POST /v1/invoices/validate · validate_invoice");
        PrintValidation(await transport.CallAsync("validate", Document(invoice, fileName, "application/xml", Representation("xRechnung", "ubl"))));

        // BR-DE-15 requires BuyerReference (BT-10). Without it the UBL stays schema-valid, so the
        // rejection comes from the XRechnung Schematron.
        Console.WriteLine("\nThe same invoice without BuyerReference (BT-10):");
        var rejected = await transport.CallAsync("validate",
            Document(WithoutBuyerReference(invoice), "missing_buyer_reference.xml", "application/xml", Representation("xRechnung", "ubl")));
        PrintValidation(rejected);
        var rules = rejected?["issues"]?.AsArray().Select(issue => issue?["ruleId"]?.ToString()).ToList() ?? [];
        if (rejected?["valid"]?.GetValue<bool>() != false || !rules.Contains("BR-DE-15"))
        {
            Write(ConsoleColor.Red, $"✘ Expected BR-DE-15, received: {string.Join(", ", rules)}");
            return false;
        }

        Write(ConsoleColor.Green, "✔ Rejected with rule BR-DE-15, which can be quoted to the supplier.");
        return true;
    }

    /// <summary>ERP import: XML or PDF in any supported format becomes one JSON model, with the validation result.</summary>
    private async Task<JsonNode> ReadIncomingInvoiceAsync(byte[] invoice, string fileName)
    {
        Case("Read an incoming invoice", "POST /v1/invoices/read · read_invoice");
        var read = await transport.CallAsync("read", Document(invoice, fileName, "application/xml"));
        var parsed = read?["invoice"] ?? throw new InvalidOperationException("The read result contains no invoice.");
        var detected = read?["detectedRepresentation"];
        Console.WriteLine($"Detected: {detected?["format"]} {detected?["version"]} {detected?["profile"]} {detected?["syntax"]}");
        Console.WriteLine($"Invoice {parsed["invoiceId"]} from {parsed["issueDate"]}: {parsed["seller"]?["name"]} → {parsed["buyer"]?["name"]}, " +
                          $"{parsed["lines"]?.AsArray().Count ?? 0} line(s), payable {parsed["totals"]?["payableAmount"]} {parsed["invoiceCurrency"]}");
        Console.WriteLine($"Validation included: valid = {read?["validation"]?["valid"]}");
        await SaveJsonAsync(transport.Name, "invoice.json", parsed);
        return parsed;
    }

    /// <summary>
    /// Billing from an ERP or shop: JSON invoice data becomes an XRechnung for public buyers (B2G) and a
    /// ZUGFeRD / Factur-X invoice for businesses (B2B). Without INVOICE_CANONICAL_JSON_PATH the invoice
    /// read in the previous case is used.
    /// </summary>
    private async Task GenerateOutgoingInvoicesAsync(JsonNode readInvoice)
    {
        Case("Generate an outgoing invoice", "POST /v1/invoices/generate · generate_invoice");
        var invoice = readInvoice;
        if (!string.IsNullOrWhiteSpace(canonicalInvoiceJsonPath))
        {
            invoice = JsonNode.Parse(await File.ReadAllTextAsync(canonicalInvoiceJsonPath))
                ?? throw new InvalidOperationException("The canonical invoice JSON file is empty.");
            Console.WriteLine($"Invoice data: {canonicalInvoiceJsonPath}");
        }
        else
        {
            Console.WriteLine("Invoice data: the JSON read above (set INVOICE_CANONICAL_JSON_PATH to use your own).");
        }

        foreach (var (audience, target) in new[]
                 {
                     ("B2G", Representation("xRechnung", "ubl")),
                     ("B2B", Representation("zugferd", "cii", "en16931"))
                 })
        {
            Console.WriteLine($"\n{audience}: {target["format"]} {target["version"]} {target["profile"]} {target["syntax"]}");
            var values = new Dictionary<string, object?> { ["invoice"] = invoice };
            foreach (var (name, value) in target)
            {
                values[$"target.{name}"] = value?.ToString();
            }

            var generated = await transport.CallAsync("generate", values);
            var document = generated?["document"];
            PrintWarnings(generated);
            var generatedName = document?["fileName"]?.ToString() ?? $"generated_{target["format"]}.xml";
            await SaveAsync(transport.Name, generatedName, document?["content"]?.ToString());

            // A ZUGFeRD hybrid PDF is also checked for PDF/A-3b by validate.
            Console.WriteLine($"Validating the generated {document?["contentType"]} document:");
            PrintValidation(await transport.CallAsync("validate", new Dictionary<string, object?>
            {
                ["document.content"] = document?["content"]?.ToString(),
                ["document.fileName"] = generatedName,
                ["document.contentType"] = document?["contentType"]?.ToString()
            }));
        }
    }

    /// <summary>A recipient requires another format. Information lost in the conversion is reported as a warning.</summary>
    private async Task<JsonNode> ConvertFormatAsync(byte[] invoice, string fileName)
    {
        Case("Convert the format", "POST /v1/invoices/convert · convert_invoice");
        JsonNode? cii = null;
        foreach (var target in new[] { Representation("xRechnung", "cii"), Representation("zugferd", "cii", "en16931") })
        {
            Console.WriteLine($"\nXRechnung UBL → {target["format"]} {target["version"]} {target["profile"]} {target["syntax"]}");
            var values = Document(invoice, fileName, "application/xml");
            foreach (var (name, value) in target)
            {
                values[$"target.{name}"] = value?.ToString();
            }

            var converted = await transport.CallAsync("convert", values);
            PrintWarnings(converted);
            var document = converted?["document"] ?? throw new InvalidOperationException("The convert result contains no document.");
            await SaveAsync(transport.Name, document["fileName"]?.ToString() ?? $"converted_{target["format"]}.xml", document["content"]?.ToString());
            cii ??= document;
        }

        return cii!;
    }

    /// <summary>Visual check, approval workflow, archive view: an XML invoice as HTML or PDF.</summary>
    private async Task RenderForPeopleAsync(JsonNode document)
    {
        Case("Render the invoice for people", "POST /v1/invoices/render · render_invoice");
        foreach (var outputFormat in (string[])["html", "pdf"])
        {
            var rendered = await transport.CallAsync("render", new Dictionary<string, object?>
            {
                ["document.content"] = document["content"]?.ToString(),
                ["document.fileName"] = document["fileName"]?.ToString(),
                ["document.contentType"] = "application/xml",
                ["outputFormat"] = outputFormat
            });
            await SaveAsync(transport.Name, rendered?["fileName"]?.ToString() ?? $"rendered_invoice.{outputFormat}", rendered?["content"]?.ToString());
        }
    }

    private void Case(string title, string endpoints) =>
        Step($"[{transport.Name.ToUpperInvariant()}] {title}  ({endpoints})");

    /// <summary>A target or expected representation with the version the service reports for the format.</summary>
    private JsonObject Representation(string format, string syntax, string? profile = null)
    {
        var version = _capabilities?["standards"]?.AsArray()
            .FirstOrDefault(standard => string.Equals(standard?["format"]?.ToString(), format, StringComparison.OrdinalIgnoreCase))?["version"]?.ToString()
            ?? throw new InvalidOperationException($"The service reports no version for {format}.");
        return new JsonObject
        {
            ["format"] = format,
            ["version"] = version,
            ["profile"] = profile ?? format,
            ["syntax"] = syntax
        };
    }

    private static Dictionary<string, object?> Document(byte[] content, string fileName, string contentType, JsonObject? expected = null)
    {
        var values = new Dictionary<string, object?>
        {
            ["document.content"] = content,
            ["document.fileName"] = fileName,
            ["document.contentType"] = contentType
        };
        foreach (var (name, value) in expected ?? [])
        {
            values[$"document.expectedRepresentation.{name}"] = value?.ToString();
        }

        return values;
    }

    private static void PrintValidation(JsonNode? validation)
    {
        if (validation?["valid"]?.GetValue<bool>() == true)
        {
            Write(ConsoleColor.Green, "✔ Valid.");
            return;
        }

        Write(ConsoleColor.Yellow, "Invalid:");
        foreach (var issue in validation?["issues"]?.AsArray() ?? [])
        {
            Console.WriteLine($"  - [{issue?["severity"]}] {issue?["ruleId"]} at {issue?["businessPath"]}: {issue?["message"]}");
        }
    }

    private static void PrintWarnings(JsonNode? result)
    {
        var warnings = result?["warnings"]?.AsArray() ?? [];
        Console.WriteLine(warnings.Count == 0 ? "No information lost." : "Warnings:");
        foreach (var warning in warnings)
        {
            Console.WriteLine($"  - {warning?["code"]} at {warning?["businessPath"]}: {warning?["message"]}");
        }
    }

    private static string List(JsonNode? array) => string.Join(", ", array?.AsArray() ?? []);

    private static byte[] WithoutBuyerReference(byte[] invoice)
    {
        var document = XDocument.Parse(Encoding.UTF8.GetString(invoice));
        var cbc = (XNamespace)"urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
        (document.Root?.Element(cbc + "BuyerReference")
            ?? throw new InvalidOperationException("The test invoice has no BuyerReference to remove.")).Remove();
        return Encoding.UTF8.GetBytes((document.Declaration is null ? "" : document.Declaration + Environment.NewLine) + document);
    }
}
