using System.Text;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using static MinimumTaxServices.Samples.Services.WalkthroughOutput;

namespace MinimumTaxServices.Samples.Services;

/// <summary>
/// The business cases of the Minimum Tax API, run identically over REST and MCP. Every country in
/// TestDocuments has a positive document and two negative ones for the GloBE Information Return (GIR) and
/// for the Country-by-Country Report (CbCR): one the XML Schema rejects (technical) and one the business
/// rules reject (fachlich). The jurisdictions are taken from the capabilities at runtime.
/// </summary>
public sealed class MinimumTaxBusinessCases(IMinimumTaxTransport transport, string documentsPath)
{
    private const string RoundTripJurisdiction = "DE";

    private static readonly (string File, string Kind)[] Documents =
    [
        ("positive.xml", "positive"),
        ("negative-xsd.xml", "technical (XSD)"),
        ("negative-business.xml", "business rules")
    ];

    private readonly JsonArray _summary = [];
    private JsonNode? _capabilities;
    private int _failures;

    public async Task<int> RunAsync()
    {
        await QueryCapabilitiesAsync();
        await CheckGirPerCountryAsync();
        await CheckCbcrPerCountryAsync();
        var report = await ReadGirAsync();
        await GenerateGirAsync(report);
        await ReadCbcrAsync();
        await SaveJsonAsync(transport.Name, "validation-summary.json", _summary);

        if (_failures > 0)
        {
            Write(ConsoleColor.Red, $"\n✘ {_failures} document(s) over {transport.Name.ToUpperInvariant()} did not get the expected result.");
            return 1;
        }

        Write(ConsoleColor.Green, $"\n✔ All business cases completed over {transport.Name.ToUpperInvariant()}.");
        return 0;
    }

    /// <summary>Integrators: which schema versions, rule sets, overlays and jurisdiction packs the service carries.</summary>
    private async Task QueryCapabilitiesAsync()
    {
        Case("Query capabilities", "GET /v1/capabilities · minimum_tax_capabilities");
        _capabilities = await transport.CallAsync("capabilities", new Dictionary<string, object?>());
        var gir = Gir;
        var cbcr = _capabilities?["cbcr"];
        Console.WriteLine($"API version: {_capabilities?["apiVersion"]}");
        Console.WriteLine($"GIR schema versions: {List(gir?["supportedSchemaVersions"])}  CbCR schema versions: {List(cbcr?["supportedSchemaVersions"])}");
        Console.WriteLine($"Default rule set: {gir?["defaultRuleSet"]}  overlays: {List(gir?["availableOverlays"])}");
        Console.WriteLine($"Jurisdictions: {List(gir?["availableJurisdictions"])}");
        foreach (var relaxation in gir?["activeRuleRelaxations"]?.AsArray() ?? [])
        {
            Console.WriteLine($"  relaxed: rule {relaxation?["ruleId"]} ({relaxation?["effectiveSeverity"]}): {relaxation?["reason"]}");
        }

        foreach (var check in cbcr?["consistencyChecks"]?.AsArray() ?? [])
        {
            Console.WriteLine($"  CbCR check {check?["code"]} ({check?["severity"]}): {check?["description"]}");
        }
    }

    /// <summary>The GIR part of the capabilities: its own block since API 4.0, the whole document before.</summary>
    private JsonNode? Gir => _capabilities?["gir"] ?? _capabilities;

    /// <summary>
    /// Tax department before filing: is the GIR for this country schema-valid and does it pass the OECD rules and
    /// the national pack? Each issue names its rule and where the rule comes from, so a rejected return can be
    /// corrected at the right place.
    /// </summary>
    private async Task CheckGirPerCountryAsync()
    {
        Case("Check a GIR per country", "POST /v1/gir/validate · validate_gir");
        foreach (var jurisdiction in Countries("gir"))
        {
            var results = new List<string>();
            foreach (var (file, kind) in Documents)
            {
                var bytes = await File.ReadAllBytesAsync(Path.Combine(documentsPath, "gir", jurisdiction, file));
                var xml = Encoding.UTF8.GetString(bytes);
                var validation = await transport.CallAsync("gir/validate", GirValidation(xml, jurisdiction, bytes));
                var issues = Issues(validation, "ruleId");
                var passed = file switch
                {
                    "positive.xml" => validation?["isValid"]?.GetValue<bool>() == true && validation["isComplete"]?.GetValue<bool>() == true,
                    // Schema findings come from the xsd layer; everything else is a rule of the OECD catalog or the national pack.
                    "negative-xsd.xml" => validation?["isValid"]?.GetValue<bool>() == false && Errors(validation, "ruleId")
                        .Any(issue => issue?["ruleId"]?.ToString() == "SCHEMA-ERR" && issue["source"]?["layer"]?.ToString() == "xsd"),
                    _ => validation?["isValid"]?.GetValue<bool>() == false && issues.Contains("60020")
                };
                Record("gir", jurisdiction, file, kind, passed, validation, issues, results);
            }

            Console.WriteLine($"  GIR  {jurisdiction}  {string.Join("  ", results)}");
        }
    }

    /// <summary>
    /// Country-by-Country Report: schema check against the OECD CbC XML Schema v2.0 and the service's
    /// consistency checks (CBCR-SEM-*), for the same countries.
    /// </summary>
    private async Task CheckCbcrPerCountryAsync()
    {
        Case("Check a CbCR per country", "POST /v1/cbcr/validate · validate_cbcr");
        foreach (var jurisdiction in Countries("cbcr"))
        {
            var results = new List<string>();
            foreach (var (file, kind) in Documents)
            {
                var xml = await File.ReadAllTextAsync(Path.Combine(documentsPath, "cbcr", jurisdiction, file));
                var validation = await transport.CallAsync("cbcr/validate", new Dictionary<string, object?> { ["xmlContent"] = xml });
                var issues = Issues(validation, "code");
                var passed = file switch
                {
                    "positive.xml" => validation?["isValid"]?.GetValue<bool>() == true,
                    "negative-xsd.xml" => validation?["isValid"]?.GetValue<bool>() == false && issues.Contains("CBCR-SCHEMA-ERR"),
                    _ => validation?["isValid"]?.GetValue<bool>() == false && issues.Contains("CBCR-SEM-002")
                };
                Record("cbcr", jurisdiction, file, kind, passed, validation, issues, results);
            }

            Console.WriteLine($"  CbCR {jurisdiction}  {string.Join("  ", results)}");
        }
    }

    /// <summary>ERP or tax engine import: GIR XML becomes the typed JSON model.</summary>
    private async Task<JsonNode> ReadGirAsync()
    {
        Case("Read a GIR", "POST /v1/gir/read · read_gir");
        var xml = await File.ReadAllTextAsync(Path.Combine(documentsPath, "gir", RoundTripJurisdiction, "positive.xml"));
        var read = await transport.CallAsync("gir/read", new Dictionary<string, object?>
        {
            ["xmlContent"] = xml,
            ["jurisdiction"] = RoundTripJurisdiction
        });
        var report = read?["report"] ?? throw new InvalidOperationException("The read result contains no report.");
        Console.WriteLine($"Detected: {read?["detectedSchemaVersion"]}, rule set {read?["ruleSetApplied"]}");
        Console.WriteLine($"Message {report["metadata"]?["messageRefId"]} ({report["metadata"]?["messageTypeIndic"]}), " +
                          $"{report["metadata"]?["transmittingCountry"]} → {report["metadata"]?["receivingCountry"]}, group {report["group"]?["groupName"]}");
        await SaveJsonAsync(transport.Name, $"gir-{RoundTripJurisdiction}.json", report);
        return report;
    }

    /// <summary>
    /// Filing from calculated data: the typed model becomes GIR XML. The generated document is validated again
    /// with its own bytes, as it would be before it is uploaded.
    /// </summary>
    private async Task GenerateGirAsync(JsonNode report)
    {
        Case("Generate a GIR", "POST /v1/gir/generate · generate_gir");
        // The model carries the ultimate parent entity and the filing entity with their TINs, which become
        // UPE/ID/TIN and FilingCE/TIN. A model without a UPE TIN is answered with 422, never with an invented one.
        var upe = report["group"]?["ultimateParent"];
        Console.WriteLine($"Ultimate parent entity: {upe?["legalName"]}, TIN {upe?["taxIdentifiers"]?[0]?["tin"]}");

        var generated = await transport.CallAsync("gir/generate", new Dictionary<string, object?>
        {
            ["report"] = report,
            ["jurisdiction"] = RoundTripJurisdiction
        });
        var generatedXml = generated?["xmlContent"]?.ToString() ?? throw new InvalidOperationException("The generate result contains no XML.");
        Console.WriteLine($"MessageRefId {generated?["messageRefId"]}, DocRefId {generated?["docRefId"]}");
        await SaveXmlAsync(transport.Name, $"generated-gir-{RoundTripJurisdiction}.xml", generatedXml);

        var bytes = Encoding.UTF8.GetBytes(generatedXml);
        var validation = await transport.CallAsync("gir/validate", GirValidation(generatedXml, RoundTripJurisdiction, bytes));
        var results = new List<string>();
        Record("gir", RoundTripJurisdiction, "generated", "generated",
            validation?["isValid"]?.GetValue<bool>() == true && validation["isComplete"]?.GetValue<bool>() == true,
            validation, Issues(validation, "ruleId"), results);
        Console.WriteLine($"Validating the generated GIR: {results[0]}");
    }

    /// <summary>CbCR import: the XML becomes the typed JSON model, with the schema issues found while reading.</summary>
    private async Task ReadCbcrAsync()
    {
        Case("Read a CbCR", "POST /v1/cbcr/read · read_cbcr");
        var xml = await File.ReadAllTextAsync(Path.Combine(documentsPath, "cbcr", RoundTripJurisdiction, "positive.xml"));
        var read = await transport.CallAsync("cbcr/read", new Dictionary<string, object?> { ["xmlContent"] = xml });
        var report = read?["report"] ?? throw new InvalidOperationException("The read result contains no report.");
        Console.WriteLine($"Detected: CbC XML {read?["detectedSchemaVersion"]}, valid = {read?["isValid"]}");
        Console.WriteLine($"Message {report["metadata"]?["messageRefId"]}, {report["metadata"]?["transmittingCountry"]} → {report["metadata"]?["receivingCountry"]}");
        await SaveJsonAsync(transport.Name, $"cbcr-{RoundTripJurisdiction}.json", report);
    }

    /// <summary>
    /// The request for /v1/gir/validate. The validation context states what a filer knows and the document
    /// cannot show: this is a first filing with no earlier messages, the TINs were checked against the national
    /// registers, and these are the original file bytes. Without it the stateful OECD rules and the encoding
    /// checks withhold their verdict and the result is valid only as far as it was checked (isComplete = false).
    /// </summary>
    private static Dictionary<string, object?> GirValidation(string xml, string jurisdiction, byte[] bytes) => new()
    {
        ["xmlContent"] = xml,
        ["jurisdiction"] = jurisdiction,
        ["validateSchema"] = true,
        ["validationContext.historyIsComplete"] = true,
        ["validationContext.previousMessageRefIds"] = new JsonArray(),
        ["validationContext.priorDocuments"] = new JsonArray(),
        ["validationContext.acceptedInitialReportingPeriods"] = new JsonArray(),
        ["validationContext.tinValidationResults"] = CheckedTins(xml),
        ["validationContext.originalXmlBytes"] = bytes
    };

    /// <summary>Every TIN of the document with its issuing country, stated as checked and valid.</summary>
    private static JsonArray CheckedTins(string xml) =>
    [
        .. XDocument.Parse(xml).Descendants()
            .Where(element => element.Name.LocalName == "TIN" && element.Attribute("issuedBy") is not null)
            .Select(element => (IssuedBy: element.Attribute("issuedBy")!.Value, Tin: element.Value.Trim()))
            .Distinct()
            .Select(tin => (JsonNode)new JsonObject { ["issuedBy"] = tin.IssuedBy, ["tin"] = tin.Tin, ["isValid"] = true })
    ];

    /// <summary>
    /// The countries with test documents. A country the service does not list is a failure, not a skip: a run
    /// that checked nothing must not end as if everything had passed.
    /// </summary>
    private IEnumerable<string> Countries(string family)
    {
        var available = Gir?["availableJurisdictions"]?.AsArray().Select(code => code?.ToString()).ToHashSet() ?? [];
        foreach (var directory in Directory.GetDirectories(Path.Combine(documentsPath, family)).Order())
        {
            var jurisdiction = Path.GetFileName(directory);
            if (available.Contains(jurisdiction))
            {
                yield return jurisdiction;
            }
            else
            {
                _failures++;
                Write(ConsoleColor.Red, $"  ✘ {family.ToUpperInvariant()} {jurisdiction}: the service does not list this jurisdiction in its capabilities.");
            }
        }
    }

    private void Record(string family, string jurisdiction, string file, string kind, bool passed, JsonNode? validation, List<string> issues, List<string> results)
    {
        _summary.Add(new JsonObject
        {
            ["document"] = family,
            ["jurisdiction"] = jurisdiction,
            ["file"] = file,
            ["kind"] = kind,
            ["passed"] = passed,
            ["isValid"] = validation?["isValid"]?.DeepClone(),
            ["isComplete"] = validation?["isComplete"]?.DeepClone(),
            ["issues"] = new JsonArray([.. issues.Select(issue => (JsonNode)issue)])
        });

        var outcome = validation?["isValid"]?.GetValue<bool>() == true ? "valid" : $"invalid {string.Join(",", issues.Distinct().Take(3))}";
        results.Add($"{(passed ? "✔" : "✘")} {Path.GetFileNameWithoutExtension(file)}: {outcome}");
        if (passed)
        {
            return;
        }

        _failures++;
        Write(ConsoleColor.Red, $"  ✘ {family.ToUpperInvariant()} {jurisdiction} {file} ({kind}) did not get the expected result:");
        foreach (var issue in validation?["issues"]?.AsArray() ?? [])
        {
            Console.WriteLine($"    - [{issue?["severity"]}] {issue?["ruleId"] ?? issue?["code"]} at {issue?["path"] ?? issue?["xPath"]}: {issue?["message"]}");
        }
    }

    /// <summary>The rule IDs or codes of the issues that count as errors.</summary>
    private static List<string> Issues(JsonNode? validation, string idProperty) =>
        Errors(validation, idProperty).Select(issue => issue?[idProperty]?.ToString() ?? "").ToList();

    private static IEnumerable<JsonNode?> Errors(JsonNode? validation, string idProperty) =>
        (validation?["issues"]?.AsArray() ?? []).Where(issue => issue?["severity"]?.ToString() == "Error" && issue[idProperty] is not null);

    private void Case(string title, string endpoints) =>
        Step($"[{transport.Name.ToUpperInvariant()}] {title}  ({endpoints})");

    private static string List(JsonNode? array) => string.Join(", ", array?.AsArray() ?? []);
}
