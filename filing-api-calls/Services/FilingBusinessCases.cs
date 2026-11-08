using System.Text.Json.Nodes;
using static FilingServices.Samples.Services.WalkthroughOutput;

namespace FilingServices.Samples.Services;

/// <summary>One checked call, as it is written to <c>summary.json</c>.</summary>
public sealed record FilingStep(string Datenart, string Route, string Operation, string Case, string Expected, int Status, bool Passed, string Detail, long Milliseconds);

/// <summary>
/// The business cases, run for the latest Stand of every Datenart over one transport:
/// <list type="bullet">
/// <item><c>validate</c> with every request example of the contract — expected: valid (<c>error: false</c>).</item>
/// <item><c>validate</c> with an empty body — expected: <c>400</c>, the service refuses a request without data.</item>
/// <item><c>preview</c> with the first example — expected: valid with a PDF, saved under <c>output/&lt;transport&gt;/pdf</c>.</item>
/// <item><c>filings</c>, <c>decode</c> and the Abfrage operations without a certificate — expected: <c>400</c>.
/// Nothing reaches ELSTER without credentials.</item>
/// <item>Only with <c>FILING_SIMULATE=true</c>: <c>filings</c> as a simulation, signed with the ERiC test certificate
/// the service brings along (<c>FILING_TEST_CERTIFICATE</c>, default <c>organisation</c>).</item>
/// </list>
/// Before them, the bundled UStVA 2026 reference filing is validated as a known-good case and once
/// with a four-digit PLZ as a known-bad one.
/// </summary>
public sealed class FilingBusinessCases(IFilingTransport transport, string referenceFilingPath, string? testCertificate)
{
    private const string ReferenceRoute = "/api/ustva/2026";
    private readonly List<FilingStep> _steps = [];

    public async Task<int> RunAsync()
    {
        var stands = await transport.LoadStandsAsync();
        Console.WriteLine($"{stands.Count} Datenarten, each in its latest Stand:");
        foreach (var stand in stands)
        {
            Console.WriteLine($"  {stand.Route,-48} {string.Join(", ", stand.Operations.Select(operation => operation.Name))}");
        }

        await RunReferenceAsync(stands);
        foreach (var stand in stands)
        {
            Step($"{stand.Route} ({stand.Document})");
            await RunStandAsync(stand);
        }

        return await SummarizeAsync(stands.Count);
    }

    private async Task RunReferenceAsync(IReadOnlyList<FilingStand> stands)
    {
        var stand = stands.FirstOrDefault(candidate => candidate.Route == ReferenceRoute);
        if (stand?.Find("validate") is not { } validate)
        {
            Record(new FilingStep("/api/ustva", ReferenceRoute, "validate", "reference", "listed", 0, false, $"{ReferenceRoute} is not in the catalog", 0));
            return;
        }

        Step($"Reference: the bundled UStVA 2026 filing on {ReferenceRoute}");
        var data = JsonNode.Parse(await File.ReadAllTextAsync(referenceFilingPath))!;
        await ExpectValidAsync(stand, validate, "reference", new JsonObject { ["data"] = data.DeepClone() });

        var invalid = data.DeepClone();
        invalid["steuerfall"]!["unternehmer"]!["plz"] = "0815";
        await ExpectAsync(stand, validate, "reference-four-digit-plz", new JsonObject { ["data"] = invalid }, "error: true",
            response => response.Status == 200 && response.Body?["error"]?.GetValue<bool>() == true);
    }

    private async Task RunStandAsync(FilingStand stand)
    {
        foreach (var operation in stand.Operations.Where(operation => transport.Supports(operation.Name)))
        {
            switch (operation.Name)
            {
                case "validate":
                    foreach (var example in operation.Examples)
                    {
                        await ExpectValidAsync(stand, operation, example.Name, example.Body.DeepClone());
                    }

                    await ExpectAsync(stand, operation, "empty-body", new JsonObject(), "400", response => response.Status == 400);
                    break;

                case "preview" when operation.Examples.Count > 0:
                    var preview = await ExpectValidAsync(stand, operation, operation.Examples[0].Name, operation.Examples[0].Body.DeepClone());
                    if (preview?["pdf"]?["content"]?.GetValue<string>() is { Length: > 0 } pdf)
                    {
                        await SaveAsync(Path.Combine(transport.Name, "pdf"), FileName(stand) + ".pdf", pdf);
                    }

                    break;

                default:
                    if (operation.Examples.Count == 0)
                    {
                        break;
                    }

                    // The contract leaves the certificate out of every example (it is the caller's own),
                    // so the example as published must be refused before anything is sent.
                    var body = operation.Examples[0].Body.DeepClone();
                    body.AsObject().Remove("certificate");
                    await ExpectAsync(stand, operation, "without-certificate", body, "400", response => response.Status == 400);

                    if (operation.Name == "filings" && testCertificate is not null)
                    {
                        var simulation = operation.Examples[0].Body.DeepClone().AsObject();
                        simulation["simulation"] = true;
                        simulation["certificate"] = new JsonObject { ["testCertificate"] = testCertificate };
                        await ExpectValidAsync(stand, operation, $"simulation-{testCertificate}", simulation);
                    }

                    break;
            }
        }
    }

    private Task<JsonNode?> ExpectValidAsync(FilingStand stand, FilingOperation operation, string name, JsonNode body) =>
        ExpectAsync(stand, operation, name, body, "200, error: false",
            response => response.Status == 200 && response.Body?["error"]?.GetValue<bool>() == false);

    private async Task<JsonNode?> ExpectAsync(FilingStand stand, FilingOperation operation, string name, JsonNode body, string expected, Func<FilingResponse, bool> passes)
    {
        var started = TimeProvider.System.GetTimestamp();
        var response = await transport.CallAsync(stand, operation, body);
        var elapsed = (long)TimeProvider.System.GetElapsedTime(started).TotalMilliseconds;
        var passed = passes(response);
        Record(new FilingStep(stand.Datenart, stand.Route, operation.Name, name, expected, response.Status, passed, Describe(response), elapsed));
        return response.Body;
    }

    private void Record(FilingStep step)
    {
        _steps.Add(step);
        Write(step.Passed ? ConsoleColor.Green : ConsoleColor.Red,
            $"  {(step.Passed ? "✔" : "✘")} {step.Operation,-17} {step.Case,-30} {step.Status} {step.Milliseconds,6} ms  {(step.Passed ? string.Empty : $"expected {step.Expected}: {step.Detail}")}");
    }

    private async Task<int> SummarizeAsync(int datenarten)
    {
        var failed = _steps.Where(step => !step.Passed).ToList();
        await SaveJsonAsync(transport.Name, "summary.json", System.Text.Json.JsonSerializer.SerializeToNode(_steps)!);
        Write(ConsoleColor.White, $"\n{transport.Name}: {datenarten} Datenarten, {_steps.Count} steps, {_steps.Count - failed.Count} passed, {failed.Count} failed.");
        foreach (var group in failed.GroupBy(step => step.Detail.Split(':')[0]).OrderByDescending(group => group.Count()))
        {
            Write(ConsoleColor.Red, $"  {group.Count(),4} × {group.Key}");
        }

        return failed.Count == 0 ? 0 : 1;
    }

    /// <summary>The error codes ERiC reported, or the problem the service answered with.</summary>
    private static string Describe(FilingResponse response)
    {
        if (response.Status != 200)
        {
            var errors = response.Body?["errors"]?.AsObject().Select(error => error.Key);
            return $"{response.Body?["type"] ?? response.Status.ToString()}: {response.Body?["detail"] ?? string.Join(", ", errors ?? [])}";
        }

        var codes = response.Body?["feedback"]?.AsArray()
            .Where(item => item?["severity"]?.GetValue<string>() == "error")
            .Select(item => item!["code"]?.ToString())
            .Distinct()
            .ToList() ?? [];
        return codes.Count == 0 ? "no error feedback" : "error codes " + string.Join(", ", codes);
    }

    private static string FileName(FilingStand stand) => stand.Route.Replace("/api/", string.Empty).Replace('/', '-');
}
