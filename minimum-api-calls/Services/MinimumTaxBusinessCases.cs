using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static MinimumTaxServices.Samples.Services.WalkthroughOutput;

namespace MinimumTaxServices.Samples.Services;

/// <summary>
/// The business cases of the Minimum Tax API, run identically over REST and MCP, for every country in
/// TestDocuments. Each country has a positive document and two negative ones for the GloBE Information Return
/// (GIR) and for the Country-by-Country Report (CbCR): one the XML Schema rejects (technical) and one the
/// business rules reject. Around the checks the walkthrough reads each GIR into the typed model and into the
/// schema-structured document, generates both again, corrects the filing against its history, reports the processing status back as a GIR status message, and reads each CbCR. The jurisdictions
/// are taken from the capabilities at runtime.
/// </summary>
public sealed partial class MinimumTaxBusinessCases(IMinimumTaxTransport transport, string documentsPath)
{
    private static readonly (string File, string Kind)[] Documents =
    [
        ("positive.xml", "positive"),
        ("negative-xsd.xml", "technical (XSD)"),
        ("negative-business.xml", "business rules")
    ];

    private readonly JsonArray _summary = [];
    private readonly Dictionary<string, JsonNode?> _businessRejections = [];
    private JsonNode? _capabilities;
    private int _failures;

    public async Task<int> RunAsync()
    {
        await QueryCapabilitiesAsync();
        await CheckGirPerCountryAsync();
        await ReadAndGenerateGirPerCountryAsync();
        await ReadAndGenerateDocumentPerCountryAsync();
        await CorrectFilingPerCountryAsync();
        await ReportStatusPerCountryAsync();
        await CheckCbcrPerCountryAsync();
        await ReadCbcrPerCountryAsync();
        await RejectInvalidInputAsync();
        await SaveJsonAsync(transport.Name, "summary.json", _summary);

        if (_failures > 0)
        {
            Write(ConsoleColor.Red, $"\n✘ {_failures} step(s) over {transport.Name.ToUpperInvariant()} did not get the expected result.");
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
                var (validation, error) = await transport.TryCallAsync("gir/validate", GirValidation(xml, jurisdiction, bytes));
                var issues = Issues(validation, "ruleId");
                var passed = error is null && file switch
                {
                    "positive.xml" => IsTrue(validation, "isValid") && IsTrue(validation, "isComplete"),
                    // Schema findings come from the xsd layer; everything else is a rule of the OECD catalog or the national pack.
                    "negative-xsd.xml" => IsFalse(validation, "isValid") && Errors(validation, "ruleId")
                        .Any(issue => issue?["ruleId"]?.ToString() == "SCHEMA-ERR" && issue["source"]?["layer"]?.ToString() == "xsd"),
                    _ => IsFalse(validation, "isValid") && issues.Contains("60020")
                };
                if (file == "negative-business.xml")
                {
                    _businessRejections[jurisdiction] = validation;
                }

                Record("gir", jurisdiction, Path.GetFileNameWithoutExtension(file), kind, passed, Outcome(validation, issues, error), IssueLines(validation, error), results);
            }

            Console.WriteLine($"  GIR  {jurisdiction}  {string.Join("  ", results)}");
        }
    }

    /// <summary>
    /// ERP import and filing from calculated data: the positive GIR becomes the typed JSON model, the model becomes
    /// GIR XML again, and the generated document is validated with its own bytes, as it would be before upload. The
    /// model keeps the ultimate parent entity and the filing entity with their TINs; generating it again must give
    /// a return the national pack still accepts.
    /// </summary>
    private async Task ReadAndGenerateGirPerCountryAsync()
    {
        Case("Read and generate a GIR per country", "POST /v1/gir/read · read_gir → POST /v1/gir/generate · generate_gir → validate_gir");
        foreach (var jurisdiction in Countries("gir"))
        {
            var results = new List<string>();
            var bytes = await File.ReadAllBytesAsync(Path.Combine(documentsPath, "gir", jurisdiction, "positive.xml"));
            var xml = Encoding.UTF8.GetString(bytes);
            var (read, readError) = await transport.TryCallAsync("gir/read", GirValidation(xml, jurisdiction, bytes));
            var report = read?["report"];
            var upeTin = report?["group"]?["ultimateParent"]?["taxIdentifiers"]?[0]?["tin"]?.ToString();
            var readPassed = readError is null && report?["metadata"]?["receivingCountry"]?.ToString() == jurisdiction && !string.IsNullOrEmpty(upeTin);
            Record("gir", jurisdiction, "read", "read into the model", readPassed,
                readError is null ? $"{report?["metadata"]?["messageRefId"]}, UPE TIN {upeTin ?? "missing"}" : "failed", Detail(readError), results);
            if (report is null)
            {
                Console.WriteLine($"  GIR  {jurisdiction}  {string.Join("  ", results)}");
                continue;
            }

            await SaveJsonAsync(transport.Name, Path.Combine(jurisdiction, "gir.json"), report);
            var (generated, generateError) = await transport.TryCallAsync("gir/generate", new Dictionary<string, object?>
            {
                ["report"] = report,
                ["jurisdiction"] = jurisdiction
            });
            var generatedXml = generated?["xmlContent"]?.ToString();
            if (generatedXml is null)
            {
                Record("gir", jurisdiction, "generate", "generated from the model", false, "failed", Detail(generateError), results);
                Console.WriteLine($"  GIR  {jurisdiction}  {string.Join("  ", results)}");
                continue;
            }

            await SaveXmlAsync(transport.Name, Path.Combine(jurisdiction, "generated-gir.xml"), generatedXml);
            var generatedBytes = Encoding.UTF8.GetBytes(generatedXml);
            var (validation, validationError) = await transport.TryCallAsync("gir/validate", GirValidation(generatedXml, jurisdiction, generatedBytes));
            var issues = Issues(validation, "ruleId");
            Record("gir", jurisdiction, "generate", "generated from the model",
                validationError is null && IsTrue(validation, "isValid") && IsTrue(validation, "isComplete"),
                Outcome(validation, issues, validationError), IssueLines(validation, validationError), results);
            Console.WriteLine($"  GIR  {jurisdiction}  {string.Join("  ", results)}");
        }
    }

    /// <summary>
    /// ERP import without a hand-written model: the GIR is read as <c>document</c>, the OECD payload in the schema's
    /// own structure (every element under its schema name, every code as the schema spells it), and posted back to
    /// generate. The result must state exactly what the original stated and still pass the national pack.
    /// </summary>
    private async Task ReadAndGenerateDocumentPerCountryAsync()
    {
        Case("Read and generate the GIR document per country", "POST /v1/gir/read · read_gir (document) → POST /v1/gir/generate · generate_gir → validate_gir");
        foreach (var jurisdiction in Countries("gir"))
        {
            var results = new List<string>();
            var bytes = await File.ReadAllBytesAsync(Path.Combine(documentsPath, "gir", jurisdiction, "positive.xml"));
            var xml = Encoding.UTF8.GetString(bytes);
            var (read, readError) = await transport.TryCallAsync("gir/read", GirValidation(xml, jurisdiction, bytes));
            var document = read?["document"];
            if (document is null)
            {
                Record("gir", jurisdiction, "document", "read as document", false, read?["documentNotOffered"]?.ToString() ?? "failed", Detail(readError), results);
                Console.WriteLine($"  GIR  {jurisdiction}  {string.Join("  ", results)}");
                continue;
            }

            await SaveJsonAsync(transport.Name, Path.Combine(jurisdiction, "gir-document.json"), document);
            var (generated, generateError) = await transport.TryCallAsync("gir/generate", new Dictionary<string, object?>
            {
                ["document"] = document.DeepClone(),
                ["nationalEnvelope"] = read?["nationalEnvelope"]?.DeepClone(),
                ["jurisdiction"] = jurisdiction,
                ["validateOutput"] = false
            });
            var generatedXml = generated?["xmlContent"]?.ToString();
            if (generatedXml is null)
            {
                Record("gir", jurisdiction, "document", "generated from the document", false, "failed", Detail(generateError), results);
                Console.WriteLine($"  GIR  {jurisdiction}  {string.Join("  ", results)}");
                continue;
            }

            await SaveXmlAsync(transport.Name, Path.Combine(jurisdiction, "generated-from-document.xml"), generatedXml);
            var lost = Except(Content(xml), Content(generatedXml));
            var added = Except(Content(generatedXml), Content(xml));
            var generatedBytes = Encoding.UTF8.GetBytes(generatedXml);
            var (validation, validationError) = await transport.TryCallAsync("gir/validate", GirValidation(generatedXml, jurisdiction, generatedBytes));
            var passed = lost.Count == 0 && added.Count == 0 && validationError is null && IsTrue(validation, "isValid") && IsTrue(validation, "isComplete");
            Record("gir", jurisdiction, "document", "read → document → generate", passed,
                lost.Count + added.Count == 0 ? Outcome(validation, Issues(validation, "ruleId"), validationError) : $"{lost.Count} lost, {added.Count} added",
                [.. lost.Select(entry => $"    lost: {entry}"), .. added.Select(entry => $"    added: {entry}"), .. IssueLines(validation, validationError)], results);
            Console.WriteLine($"  GIR  {jurisdiction}  {string.Join("  ", results)}");
        }
    }

    /// <summary>
    /// A correction of the filed return, made by editing the document as a filer would: GIR102, the FilingInfo resent
    /// unchanged as OECD0 under its DocRefId, every other record sent again as OECD2 under a new DocRefId in the
    /// national format and naming the record it corrects. Validated against the history the authority holds (the
    /// first filing, accepted), no pack may find anything wrong with it. Where a pack needs records only the
    /// authority holds, it withholds a named rule instead of deciding; those are listed here. A correction of a
    /// record that was never filed must be refused.
    /// </summary>
    private async Task CorrectFilingPerCountryAsync()
    {
        Case("Correct the filing per country", "read_gir (document) → generate_gir (GIR102) → validate_gir with the filing history");
        foreach (var jurisdiction in Countries("gir"))
        {
            var results = new List<string>();
            foreach (var (step, neverFiled) in new[] { ("correction", false), ("unknown record", true) })
            {
                var (correction, history, error) = await CorrectionAsync(jurisdiction, neverFiled);
                if (correction is null)
                {
                    Record("gir", jurisdiction, step, "correction", false, "failed", Detail(error), results);
                    continue;
                }

                var bytes = Encoding.UTF8.GetBytes(correction);
                var request = GirValidation(correction, jurisdiction, bytes);
                foreach (var (key, value) in history)
                {
                    request[$"validationContext.{key}"] = value;
                }

                var (validation, validationError) = await transport.TryCallAsync("gir/validate", request);
                var errors = Issues(validation, "ruleId");
                var withheld = (validation?["issues"]?.AsArray() ?? [])
                    .Where(issue => issue?["severityAdjustment"]?.ToString() == "verdict-withheld-missing-context")
                    .Select(issue => issue?["ruleId"]?.ToString() ?? "").Order(StringComparer.Ordinal).ToList();
                var expectedWithheld = WithheldOnCorrection.GetValueOrDefault(jurisdiction, []);
                var passed = validationError is null && (neverFiled
                    ? IsFalse(validation, "isValid") && errors.Count > 0
                    : errors.Count == 0 && withheld.SequenceEqual(expectedWithheld)
                      && IsTrue(validation, "isComplete") == (expectedWithheld.Length == 0));
                var outcome = neverFiled
                    ? Outcome(validation, errors, validationError)
                    : errors.Count > 0 ? Outcome(validation, errors, validationError)
                    : withheld.Count == 0 ? "accepted" : $"no finding, withheld {string.Join(",", withheld)}";
                Record("gir", jurisdiction, step, "correction", passed, outcome, IssueLines(validation, validationError), results);
            }

            Console.WriteLine($"  GIR correction {jurisdiction}  {string.Join("  ", results)}");
        }
    }

    /// <summary>Packs that compare a correction with records the validation context cannot carry, and the rules they withhold.</summary>
    private static readonly Dictionary<string, string[]> WithheldOnCorrection = new()
    {
        ["AT"] = ["AT-GIR-AT00010"],
        ["IE"] = ["IE-GIR-9999-3", "IE-GIR-9999-4"],
        ["LU"] = ["LU-GIR-99001", "LU-GIR-99004"],
        ["PL"] = ["PL-GIR-REG.031", "PL-GIR-REG.032"]
    };

    private async Task<(string? Xml, Dictionary<string, object?> History, string? Error)> CorrectionAsync(string jurisdiction, bool neverFiled)
    {
        var bytes = await File.ReadAllBytesAsync(Path.Combine(documentsPath, "gir", jurisdiction, "positive.xml"));
        var original = Encoding.UTF8.GetString(bytes);
        var (read, readError) = await transport.TryCallAsync("gir/read", GirValidation(original, jurisdiction, bytes));
        if (read?["document"]?.DeepClone() is not { } document)
        {
            return (null, [], readError ?? read?["documentNotOffered"]?.ToString());
        }

        var messageSpec = document["messageSpec"]!;
        var filed = messageSpec["messageRefId"]!.ToString();
        var filedAt = DateTime.Parse(messageSpec["timestamp"]!.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        var correctedAt = filedAt.AddDays(30);
        messageSpec["messageRefId"] = Successor(filed, correctedAt);
        messageSpec["messageTypeIndic"] = "GIR102";
        messageSpec["timestamp"] = correctedAt.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
        foreach (var body in document["globeBody"]!.AsArray())
        {
            foreach (var section in (string[])["filingInfo", "generalSection", "summary", "jurisdictionSection", "utprAttribution"])
            {
                IEnumerable<JsonNode> records = body![section] switch
                {
                    JsonArray array => array.Select(record => record!),
                    JsonObject record => [record],
                    _ => []
                };
                foreach (var record in records)
                {
                    var docSpec = record["docSpec"]!;
                    if (section == "filingInfo")
                    {
                        docSpec["docTypeIndic"] = "OECD0";
                        continue;
                    }

                    var docRefId = docSpec["docRefId"]!.ToString();
                    docSpec["docTypeIndic"] = "OECD2";
                    docSpec["docRefId"] = Successor(docRefId, correctedAt);
                    docSpec["corrDocRefId"] = neverFiled ? docRefId + "-NEVER-FILED" : docRefId;
                }
            }
        }

        var (generated, generateError) = await transport.TryCallAsync("gir/generate", new Dictionary<string, object?>
        {
            ["document"] = document,
            ["nationalEnvelope"] = read["nationalEnvelope"]?.DeepClone(),
            ["jurisdiction"] = jurisdiction,
            ["validateOutput"] = false
        });
        var xml = generated?["xmlContent"]?.ToString();
        if (xml is not null)
        {
            await SaveXmlAsync(transport.Name, Path.Combine(jurisdiction, neverFiled ? "correction-unknown-record.xml" : "correction.xml"), xml);
        }

        return (xml, new Dictionary<string, object?>
        {
            ["previousMessageRefIds"] = new JsonArray(filed),
            ["priorDocuments"] = AcceptedRecords(original),
            ["previousSubmissionTimestamp"] = filedAt.ToString("O", CultureInfo.InvariantCulture),
            ["acceptedInitialReportingPeriods"] = new JsonArray(messageSpec["reportingPeriod"]!.ToString())
        }, generateError);
    }

    /// <summary>
    /// A new identifier in the format of the one it follows, as a filer's software would create it: a new UUID
    /// version 4 where the old one had a UUID, the creation time where it states one (BG), the next number where it
    /// ends in digits (PL), otherwise the old one with a suffix.
    /// </summary>
    private static string Successor(string identifier, DateTime createdAt)
    {
        var uuid = Uuid().Match(identifier);
        if (uuid.Success)
        {
            var hash = MD5.HashData(Encoding.UTF8.GetBytes(identifier));
            hash[6] = (byte)(hash[6] & 0x0F | 0x40);
            hash[8] = (byte)(hash[8] & 0x3F | 0x80);
            var next = Convert.ToHexStringLower(hash);
            return identifier[..uuid.Index] + $"{next[..8]}-{next[8..12]}-{next[12..16]}-{next[16..20]}-{next[20..]}" + identifier[(uuid.Index + uuid.Length)..];
        }

        var stamp = CreationTime().Match(identifier);
        if (stamp.Success)
        {
            return identifier[..stamp.Index] + createdAt.ToString("_yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture);
        }

        var digits = TrailingDigits().Match(identifier);
        return digits.Success
            ? identifier[..digits.Index] + (long.Parse(digits.Value, CultureInfo.InvariantCulture) + 5000).ToString(new string('0', digits.Length), CultureInfo.InvariantCulture)
            : identifier + "C";
    }

    /// <summary>
    /// The records of the accepted first filing as the authority holds them, with what the correction rules compare
    /// against: each record's recipient jurisdictions, the UPE's residence and the accepted FilingInfo.
    /// </summary>
    private static JsonArray AcceptedRecords(string xml)
    {
        var root = XDocument.Parse(xml).Root!;
        var messageRefId = Value(root, "MessageRefId");
        var period = Value(root, "ReportingPeriod");
        var records = new JsonArray();
        foreach (var body in root.Descendants().Where(element => element.Name.LocalName == "GLOBEBody"))
        {
            var filingInfoDocRefId = Value(Child(Child(body, "FilingInfo"), "DocSpec"), "DocRefId");
            foreach (var section in body.Elements().Where(element => element.Elements().Any(child => child.Name.LocalName == "DocSpec")))
            {
                var record = new JsonObject
                {
                    ["messageRefId"] = messageRefId,
                    ["docRefId"] = Value(Child(section, "DocSpec"), "DocRefId"),
                    ["section"] = section.Name.LocalName,
                    ["isLatest"] = true,
                    ["acceptedReportingPeriod"] = period,
                    ["acceptedFilingInfoDocRefId"] = filingInfoDocRefId,
                    ["acceptedRecipientCountries"] = Codes(section.Descendants().Where(element => element.Name.LocalName == "RecJurCode"))
                };
                if (section.Name.LocalName == "GeneralSection")
                {
                    record["acceptedUpeResidenceCountries"] = Codes(section.Descendants().Where(element => element.Name.LocalName == "UPE")
                        .SelectMany(upe => upe.Descendants().Where(element => element.Name.LocalName == "ResCountryCode")));
                }

                if (section.Name.LocalName == "FilingInfo")
                {
                    var filingCe = Child(section, "FilingCE");
                    var tin = Child(filingCe, "TIN");
                    record["acceptedFilingInfo"] = new JsonObject
                    {
                        ["nameMne"] = Value(section, "NameMNE"), ["cfsOfUpe"] = Value(section, "CFSofUPE"),
                        ["currency"] = Value(section, "Currency"), ["periodStart"] = Value(section, "Start"),
                        ["periodEnd"] = Value(section, "End"), ["fas"] = Value(section, "FAS"),
                        ["additionalInfo"] = Value(section, "AdditionalInfo")
                    };
                    record["acceptedFilingCe"] = new JsonObject
                    {
                        ["residenceCountry"] = Value(filingCe, "ResCountryCode"), ["name"] = Value(filingCe, "Name"),
                        ["tin"] = tin.Value.Trim(), ["role"] = Value(filingCe, "Role"),
                        ["tinIssuedBy"] = tin.Attribute("issuedBy")?.Value, ["tinType"] = tin.Attribute("TypeOfTIN")?.Value,
                        ["tinUnknown"] = tin.Attribute("unknown")?.Value == "true"
                    };
                }

                records.Add(record);
            }
        }

        return records;

        static XElement Child(XElement parent, string name) => parent.Elements().First(element => element.Name.LocalName == name);

        static string? Value(XElement parent, string name) => parent.Descendants().FirstOrDefault(element => element.Name.LocalName == name)?.Value.Trim();

        static JsonArray Codes(IEnumerable<XElement> elements) =>
            new([.. elements.Select(element => element.Value.Trim()).Distinct().Select(code => (JsonNode)code)]);
    }

    /// <summary>Every leaf value and attribute of a document as path=value, prefixes and formatting removed.</summary>
    private static List<string> Content(string xml)
    {
        var root = XDocument.Parse(xml).Root!;
        static string PathOf(XElement element) => string.Join('/', element.AncestorsAndSelf().Reverse().Select(ancestor => ancestor.Name.LocalName));
        return
        [
            .. root.DescendantsAndSelf().Where(element => !element.HasElements).Select(element => $"{PathOf(element)}={element.Value.Trim()}"),
            .. root.DescendantsAndSelf().SelectMany(element => element.Attributes()
                .Where(attribute => !attribute.IsNamespaceDeclaration && attribute.Name.Namespace == XNamespace.None)
                .Select(attribute => $"{PathOf(element)}/@{attribute.Name.LocalName}={attribute.Value}"))
        ];
    }

    /// <summary>The entries of <paramref name="left"/> that <paramref name="right"/> lacks, counting duplicates.</summary>
    private static List<string> Except(List<string> left, List<string> right)
    {
        var remaining = right.GroupBy(entry => entry).ToDictionary(group => group.Key, group => group.Count());
        return [.. left.Where(entry => !(remaining.TryGetValue(entry, out var count) && count > 0 && (remaining[entry] = count - 1) >= 0))];
    }

    [GeneratedRegex("[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-4[0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}", RegexOptions.CultureInvariant)]
    private static partial Regex Uuid();

    [GeneratedRegex(@"_\d{4}-\d{2}-\d{2}_\d{2}-\d{2}-\d{2}$", RegexOptions.CultureInvariant)]
    private static partial Regex CreationTime();

    [GeneratedRegex(@"\d+$", RegexOptions.CultureInvariant)]
    private static partial Regex TrailingDigits();

    /// <summary>
    /// The receiving authority's answer: a GIR status message that accepts the positive return and one that rejects
    /// the business-rule negative with the errors its validation found. Reading each message back must give the same
    /// status, the same original MessageRefId and, for the rejection, the same rule codes.
    /// </summary>
    private async Task ReportStatusPerCountryAsync()
    {
        Case("Report the processing status per country",
            "POST /v1/gir/status-message/generate · generate_gir_status_message → POST /v1/gir/status-message/read · read_gir_status_message");
        foreach (var jurisdiction in Countries("gir"))
        {
            var results = new List<string>();
            foreach (var (file, accepted) in new[] { ("positive.xml", true), ("negative-business.xml", false) })
            {
                var xml = await File.ReadAllTextAsync(Path.Combine(documentsPath, "gir", jurisdiction, file));
                var messageRefId = MessageRefId().Match(xml).Groups["id"].Value;
                JsonNode?[] errors = accepted ? [] : Errors(_businessRejections.GetValueOrDefault(jurisdiction), "ruleId").Select(issue => issue!.DeepClone()).ToArray();
                var step = accepted ? "accepted" : "rejected";
                var (generated, generateError) = await transport.TryCallAsync("gir/status-message/generate", new Dictionary<string, object?>
                {
                    ["originalMessageRefId"] = messageRefId,
                    ["transmittingCountry"] = jurisdiction,
                    ["receivingCountry"] = jurisdiction,
                    ["isAccepted"] = accepted,
                    ["errors"] = new JsonArray(errors)
                });
                var statusXml = generated?["statusMessageXml"]?.ToString();
                if (statusXml is null)
                {
                    Record("status", jurisdiction, step, "status message", false, "generate failed", Detail(generateError), results);
                    continue;
                }

                var (read, readError) = await transport.TryCallAsync("gir/status-message/read", new Dictionary<string, object?> { ["xmlContent"] = statusXml });
                var expectedCodes = errors.Select(error => error?["ruleId"]?.ToString()).Order().ToList();
                // reportedErrors are the errors the message reports about the return; issues are the message's own schema issues.
                var readCodes = (read?["reportedErrors"]?.AsArray() ?? []).Select(issue => issue?["ruleId"]?.ToString()).Order().ToList();
                var passed = readError is null
                    && IsTrue(generated, "isValidSchema") && IsTrue(read, "isValidSchema")
                    && read?["acceptanceStatus"]?.ToString() == (accepted ? "Accepted" : "Rejected")
                    && read?["originalMessageRefId"]?.ToString() == messageRefId
                    && readCodes.SequenceEqual(expectedCodes);
                var detail = readError is not null
                    ? Detail(readError)
                    : [$"    expected codes [{string.Join(", ", expectedCodes)}], read back [{string.Join(", ", readCodes)}], status {read?["acceptanceStatus"]}"];
                Record("status", jurisdiction, step, "status message", passed,
                    $"{read?["acceptanceStatus"] ?? "unread"} [{string.Join(",", readCodes)}]", detail, results);
            }

            Console.WriteLine($"  GIR status {jurisdiction}  {string.Join("  ", results)}");
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
                var (validation, error) = await transport.TryCallAsync("cbcr/validate", new Dictionary<string, object?> { ["xmlContent"] = xml });
                var issues = Issues(validation, "code");
                var passed = error is null && file switch
                {
                    "positive.xml" => IsTrue(validation, "isValid"),
                    "negative-xsd.xml" => IsFalse(validation, "isValid") && issues.Contains("CBCR-SCHEMA-ERR"),
                    _ => IsFalse(validation, "isValid") && issues.Contains("CBCR-SEM-002")
                };
                Record("cbcr", jurisdiction, Path.GetFileNameWithoutExtension(file), kind, passed, Outcome(validation, issues, error), IssueLines(validation, error), results);
            }

            Console.WriteLine($"  CbCR {jurisdiction}  {string.Join("  ", results)}");
        }
    }

    /// <summary>CbCR import: the XML becomes the typed JSON model, with the schema issues found while reading.</summary>
    private async Task ReadCbcrPerCountryAsync()
    {
        Case("Read a CbCR per country", "POST /v1/cbcr/read · read_cbcr");
        foreach (var jurisdiction in Countries("cbcr"))
        {
            var results = new List<string>();
            var xml = await File.ReadAllTextAsync(Path.Combine(documentsPath, "cbcr", jurisdiction, "positive.xml"));
            var (read, error) = await transport.TryCallAsync("cbcr/read", new Dictionary<string, object?> { ["xmlContent"] = xml });
            var report = read?["report"];
            var passed = error is null && IsTrue(read, "isValid") && report?["metadata"]?["transmittingCountry"]?.ToString() == jurisdiction;
            Record("cbcr", jurisdiction, "read", "read into the model", passed,
                error is null ? $"{report?["metadata"]?["messageRefId"]}, {report?["metadata"]?["transmittingCountry"]} → {report?["metadata"]?["receivingCountry"]}" : "failed",
                IssueLines(read, error), results);
            if (report is not null)
            {
                await SaveJsonAsync(transport.Name, Path.Combine(jurisdiction, "cbcr.json"), report);
            }

            Console.WriteLine($"  CbCR {jurisdiction}  {string.Join("  ", results)}");
        }
    }

    /// <summary>
    /// Input the service must refuse rather than complete on the caller's behalf: an unknown code, a constituent
    /// entity without owners, a reported computation without its figures, a document of the wrong kind and a tool
    /// call without its document. Each is expected with its status code, so a substitute value or a server error fails.
    /// </summary>
    private async Task RejectInvalidInputAsync()
    {
        Case("Reject invalid input", "generate_gir · read_gir · read_cbcr · validate_cbcr with input the service must refuse");
        var bytes = await File.ReadAllBytesAsync(Path.Combine(documentsPath, "gir", "DE", "positive.xml"));
        var (read, readError) = await transport.TryCallAsync("gir/read", GirValidation(Encoding.UTF8.GetString(bytes), "DE", bytes));
        var model = read?["report"] ?? throw new InvalidOperationException($"The DE positive GIR could not be read: {readError}");
        var results = new List<string>();

        foreach (var (name, change, field) in new (string, Action<JsonNode>, string)[]
                 {
                     ("unknown country", report => report["metadata"]!["transmittingCountry"] = "UK", "country"),
                     ("unknown currency", report => report["group"]!["reportingCurrency"] = "EURO", "currency"),
                     ("unknown TIN type", report => report["group"]!["ultimateParent"]!["taxIdentifiers"]![0]!["tinType"] = "GIR9999", "TypeOfTIN"),
                     ("CE without owners", report => report["group"]!["entities"] = new JsonArray(new JsonObject
                     {
                         ["entityId"] = "Sub GmbH", ["legalName"] = "Sub GmbH", ["jurisdictionCode"] = "DE",
                         ["taxIdentifiers"] = new JsonArray(new JsonObject { ["tin"] = "5555555555555", ["issuedBy"] = "DE", ["tinType"] = "GIR3001" })
                     }), "directOwnerships"),
                     ("computation without figures", report => report["group"]!["jurisdictions"] = new JsonArray(new JsonObject
                     {
                         ["jurisdictionCode"] = "DE",
                         ["netGlobeIncomeLoss"] = new JsonObject { ["financialAccountingNetIncome"] = 100, ["netGlobeIncomeOrLoss"] = 100 }
                     }), "effectiveTaxRate.calculatedEtr")
                 })
        {
            var changed = model.DeepClone();
            change(changed);
            var (_, error) = await transport.TryCallAsync("gir/generate", new Dictionary<string, object?> { ["report"] = changed, ["validateOutput"] = false });
            Expect(name, error, 422, field, results);
        }

        var cbcr = await File.ReadAllTextAsync(Path.Combine(documentsPath, "cbcr", "DE", "positive.xml"));
        var gir = Encoding.UTF8.GetString(bytes);
        var (_, girReadError) = await transport.TryCallAsync("gir/read", new Dictionary<string, object?> { ["xmlContent"] = cbcr });
        Expect("gir/read with a CbCR", girReadError, 400, "GloBE Information Return", results);
        var (_, cbcrReadError) = await transport.TryCallAsync("cbcr/read", new Dictionary<string, object?> { ["xmlContent"] = gir });
        Expect("cbcr/read with a GIR", cbcrReadError, 400, "CbC report", results);

        // A reported total of 0 against non-zero parts is the inconsistency CBCR-SEM-004 exists to report.
        var (validation, validationError) = await transport.TryCallAsync("cbcr/validate", new Dictionary<string, object?>
        {
            ["xmlContent"] = cbcr.Replace(">900000000</cbc:Total>", ">0</cbc:Total>", StringComparison.Ordinal)
        });
        var codes = (validation?["issues"]?.AsArray() ?? []).Select(issue => issue?["code"]?.ToString()).ToList();
        Record("input", "DE", "total 0", "rejected input", validationError is null && codes.Contains("CBCR-SEM-004"),
            validationError is null ? $"issues {string.Join(",", codes)}" : "failed", Detail(validationError), results);

        Console.WriteLine($"  Input  {string.Join("  ", results)}");
    }

    /// <summary>A refused call: its error must carry the expected status and name the field.</summary>
    private void Expect(string name, string? error, int status, string field, List<string> results)
    {
        var passed = error is not null
            && (error.Contains($"({status} ", StringComparison.Ordinal) || error.Contains($"\"status\":{status}", StringComparison.Ordinal))
            && error.Contains(field, StringComparison.OrdinalIgnoreCase);
        Record("input", "DE", name, "rejected input", passed, error is null ? "accepted" : $"refused {status}", Detail(error ?? "the service accepted the input"), results);
    }

    /// <summary>
    /// The request for /v1/gir/validate and /v1/gir/read. The validation context states what a filer knows and the
    /// document cannot show: this is a first filing with no earlier messages, the TINs were checked against the
    /// national registers, and these are the original file bytes. Without it the stateful OECD rules and the
    /// encoding checks withhold their verdict and the result is valid only as far as it was checked
    /// (isComplete = false).
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

    private void Record(string family, string jurisdiction, string step, string kind, bool passed, string outcome, IEnumerable<string> detail, List<string> results)
    {
        _summary.Add(new JsonObject
        {
            ["document"] = family,
            ["jurisdiction"] = jurisdiction,
            ["step"] = step,
            ["kind"] = kind,
            ["passed"] = passed,
            ["outcome"] = outcome
        });

        results.Add($"{(passed ? "✔" : "✘")} {step}: {outcome}");
        if (passed)
        {
            return;
        }

        _failures++;
        Write(ConsoleColor.Red, $"  ✘ {family.ToUpperInvariant()} {jurisdiction} {step} ({kind}) did not get the expected result:");
        foreach (var line in detail)
        {
            Console.WriteLine(line);
        }
    }

    private static string Outcome(JsonNode? validation, List<string> issues, string? error) =>
        error is not null ? "failed" : IsTrue(validation, "isValid") ? "valid" : $"invalid {string.Join(",", issues.Distinct().Take(3))}";

    private static IEnumerable<string> IssueLines(JsonNode? result, string? error) =>
        error is not null
            ? Detail(error)
            : ((result?["issues"] ?? result?["validationIssues"])?.AsArray() ?? [])
                .Select(issue => $"    - [{issue?["severity"]}] {issue?["ruleId"] ?? issue?["code"]} at {issue?["path"] ?? issue?["xPath"]}: {issue?["message"]}");

    private static string[] Detail(string? error) => error is null ? [] : [$"    {error}"];

    /// <summary>The rule IDs or codes of the issues that count as errors.</summary>
    private static List<string> Issues(JsonNode? validation, string idProperty) =>
        Errors(validation, idProperty).Select(issue => issue?[idProperty]?.ToString() ?? "").ToList();

    private static IEnumerable<JsonNode?> Errors(JsonNode? validation, string idProperty) =>
        (validation?["issues"]?.AsArray() ?? []).Where(issue => issue?["severity"]?.ToString() == "Error" && issue[idProperty] is not null);

    private static bool IsTrue(JsonNode? node, string property) => node?[property]?.GetValue<bool>() == true;

    private static bool IsFalse(JsonNode? node, string property) => node?[property]?.GetValue<bool>() == false;

    private void Case(string title, string endpoints) =>
        Step($"[{transport.Name.ToUpperInvariant()}] {title}  ({endpoints})");

    private static string List(JsonNode? array) => string.Join(", ", array?.AsArray() ?? []);

    [GeneratedRegex("MessageRefId>(?<id>[^<]+)<", RegexOptions.CultureInvariant)]
    private static partial Regex MessageRefId();
}
