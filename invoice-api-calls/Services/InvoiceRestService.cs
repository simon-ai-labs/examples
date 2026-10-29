using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static InvoiceServices.Samples.Services.WalkthroughOutput;

namespace InvoiceServices.Samples.Services;

/// <summary>
/// REST walkthrough without request or response classes. It reads the OpenAPI description that the
/// Scalar page at /docs/ renders, looks up each operation there and builds the request body from the
/// operation's schema at runtime. Responses are read as JSON.
/// </summary>
public sealed partial class InvoiceRestService(HttpClient http, string? apiKey, string invoicePath)
{
    private const string Transport = "rest";

    private JsonNode _description = new JsonObject();
    private SchemaRequestBuilder _builder = new(new JsonObject());

    public async Task<int> RunAsync()
    {
        Write(ConsoleColor.White, "\n==================== REST walkthrough ====================");
        await LoadDescriptionAsync();

        Step("GET /v1/capabilities");
        var capabilities = await CallAsync(HttpMethod.Get, "/v1/capabilities");
        Console.WriteLine($"API version: {capabilities?["apiVersion"]}");
        foreach (var standard in capabilities?["standards"]?.AsArray() ?? [])
        {
            Console.WriteLine($"  - {standard?["format"]} {standard?["version"]} ({string.Join(", ", standard?["syntaxes"]?.AsArray() ?? [])})");
        }

        var invoice = await File.ReadAllBytesAsync(invoicePath);
        var fileName = Path.GetFileName(invoicePath);
        Console.WriteLine($"\nInvoice: {fileName} ({invoice.Length:N0} bytes)");

        Step("POST /v1/invoices/validate");
        var validation = await CallAsync(HttpMethod.Post, "/v1/invoices/validate", Document(invoice, fileName, expectXRechnungUbl: true));
        PrintValidation(validation);

        Step("POST /v1/invoices/validate without BuyerReference (BT-10), expecting BR-DE-15");
        var expectedFailure = await CallAsync(HttpMethod.Post, "/v1/invoices/validate",
            Document(WithoutBuyerReference(invoice), "missing_buyer_reference.xml", expectXRechnungUbl: true));
        var rules = expectedFailure?["issues"]?.AsArray().Select(issue => issue?["ruleId"]?.ToString()).ToList() ?? [];
        if (expectedFailure?["valid"]?.GetValue<bool>() != false || !rules.Contains("BR-DE-15"))
        {
            Write(ConsoleColor.Red, $"✘ Expected BR-DE-15, received: {string.Join(", ", rules)}");
            return 1;
        }

        Write(ConsoleColor.Green, "✔ Schematron rule BR-DE-15 reported as expected.");

        Step("POST /v1/invoices/read");
        var read = await CallAsync(HttpMethod.Post, "/v1/invoices/read", Document(invoice, fileName));
        var parsed = read?["invoice"];
        Console.WriteLine($"Invoice {parsed?["invoiceId"]} from {parsed?["issueDate"]}: {parsed?["seller"]?["name"]} → {parsed?["buyer"]?["name"]}, " +
                          $"payable {parsed?["totals"]?["payableAmount"]} {parsed?["invoiceCurrency"]}");

        Step("POST /v1/invoices/convert to XRechnung CII");
        var converted = await CallAsync(HttpMethod.Post, "/v1/invoices/convert", new Dictionary<string, object?>(Document(invoice, fileName))
        {
            ["target.format"] = "xRechnung",
            ["target.version"] = "3.0.2",
            ["target.profile"] = "xRechnung",
            ["target.syntax"] = "cii"
        });
        var convertedName = converted?["document"]?["fileName"]?.ToString() ?? "converted_xrechnung_cii.xml";
        var convertedContent = converted?["document"]?["content"]?.ToString();
        await SaveAsync(Transport, convertedName, convertedContent);

        foreach (var outputFormat in (string[])["html", "pdf"])
        {
            Step($"POST /v1/invoices/render as {outputFormat}");
            var rendered = await CallAsync(HttpMethod.Post, "/v1/invoices/render", new Dictionary<string, object?>
            {
                ["document.content"] = convertedContent,
                ["document.fileName"] = convertedName,
                ["document.contentType"] = "application/xml",
                ["outputFormat"] = outputFormat
            });
            await SaveAsync(Transport, rendered?["fileName"]?.ToString() ?? $"rendered_invoice.{outputFormat}", rendered?["content"]?.ToString());
        }

        Write(ConsoleColor.Green, "\n✔ REST walkthrough completed.");
        return 0;
    }

    /// <summary>Reads /docs/, follows it to the OpenAPI description and lists the documented operations.</summary>
    private async Task LoadDescriptionAsync()
    {
        var docsUrl = new Uri(http.BaseAddress!, "/docs/");
        Step($"Reading the API description from {docsUrl}");
        var page = await http.GetStringAsync(docsUrl);
        var source = ScalarSource().Match(page);
        if (!source.Success)
        {
            throw new InvalidOperationException($"No OpenAPI description is referenced on {docsUrl}.");
        }

        // Scalar resolves a relative source URL against the site root.
        var url = Regex.Unescape(source.Groups["url"].Value);
        var specificationUrl = Uri.TryCreate(url, UriKind.Absolute, out var absolute) && absolute.Scheme is "http" or "https"
            ? absolute
            : new Uri(http.BaseAddress!, "/" + url.TrimStart('/'));
        _description = JsonNode.Parse(await http.GetStringAsync(specificationUrl))
            ?? throw new InvalidOperationException($"{specificationUrl} is empty.");
        _builder = new SchemaRequestBuilder(_description);

        Console.WriteLine($"{_description["info"]?["title"]} {_description["info"]?["version"]} ({specificationUrl})");
        foreach (var (path, operations) in _description["paths"]!.AsObject())
        {
            foreach (var (method, _) in operations!.AsObject())
            {
                Console.WriteLine($"  {method.ToUpperInvariant(),-6} {path}");
            }
        }
    }

    private async Task<JsonNode?> CallAsync(HttpMethod method, string path, IReadOnlyDictionary<string, object?>? values = null)
    {
        var operation = _description["paths"]?[path]?[method.Method.ToLowerInvariant()]
            ?? throw new InvalidOperationException($"{method} {path} is not documented in the API description.");

        using var request = new HttpRequestMessage(method, path);
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            request.Headers.Add("X-API-Key", apiKey);
        }

        if (values is not null)
        {
            var schema = operation["requestBody"]?["content"]?["application/json"]?["schema"]
                ?? throw new InvalidOperationException($"{method} {path} documents no JSON request body.");
            request.Content = JsonContent.Create(_builder.Build(schema, values));
            // The Marketplace quota layer counts each document operation by its Idempotency-Key.
            request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString("N"));
        }

        using var response = await http.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"{method} {path} failed ({(int)response.StatusCode} {response.StatusCode}): {body}", null, response.StatusCode);
        }

        return JsonNode.Parse(body);
    }

    private static Dictionary<string, object?> Document(byte[] content, string fileName, bool expectXRechnungUbl = false)
    {
        var values = new Dictionary<string, object?>
        {
            ["document.content"] = content,
            ["document.fileName"] = fileName,
            ["document.contentType"] = "application/xml"
        };
        if (expectXRechnungUbl)
        {
            values["document.expectedRepresentation.format"] = "xRechnung";
            values["document.expectedRepresentation.version"] = "3.0.2";
            values["document.expectedRepresentation.profile"] = "xRechnung";
            values["document.expectedRepresentation.syntax"] = "ubl";
        }

        return values;
    }

    private static void PrintValidation(JsonNode? validation)
    {
        if (validation?["valid"]?.GetValue<bool>() == true)
        {
            Write(ConsoleColor.Green, "✔ Valid according to EN 16931 and XRechnung.");
            return;
        }

        Write(ConsoleColor.Yellow, "Validation issues:");
        foreach (var issue in validation?["issues"]?.AsArray() ?? [])
        {
            Console.WriteLine($"  - [{issue?["severity"]}] {issue?["ruleId"]}: {issue?["message"]}");
        }
    }

    /// <summary>
    /// BR-DE-15 requires BuyerReference (BT-10). Without it the UBL stays schema-valid, so the failure
    /// comes from the XRechnung Schematron.
    /// </summary>
    private static byte[] WithoutBuyerReference(byte[] invoice)
    {
        var document = XDocument.Parse(Encoding.UTF8.GetString(invoice));
        var cbc = (XNamespace)"urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
        (document.Root?.Element(cbc + "BuyerReference")
            ?? throw new InvalidOperationException("The test invoice has no BuyerReference to remove.")).Remove();
        return Encoding.UTF8.GetBytes((document.Declaration is null ? "" : document.Declaration + Environment.NewLine) + document);
    }

    [GeneratedRegex("""
        "sources"\s*:\s*\[\s*\{[^}]*"url"\s*:\s*"(?<url>[^"]+)"
        """, RegexOptions.CultureInvariant)]
    private static partial Regex ScalarSource();
}
