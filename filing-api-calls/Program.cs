using FilingServices.Samples.Configuration;
using FilingServices.Samples.Services;

// Usage: [rest|mcp|perf] [baseUrl] [apiKey]. Without a mode the REST and MCP walkthroughs run.
var mode = args.FirstOrDefault()?.ToLowerInvariant() is "rest" or "mcp" or "perf" ? args[0].ToLowerInvariant() : null;
var (baseUrl, apiKey) = ConfigurationHelper.ResolveConnectionSettings(mode is null ? args : args[1..]);
var baseUri = new Uri(baseUrl.TrimEnd('/') + "/");
var referenceFilingPath = Path.Combine(AppContext.BaseDirectory, "TestFilings", "ustva-2026.json");

// FILING_SIMULATE=true sends every Datenart once to ELSTER as a test case (Testmerker), signed with the
// ERiC test certificate the service brings along. It is off by default: it is a real transmission.
var testCertificate = ConfigurationHelper.GetSetting("FILING_SIMULATE") is "true" or "1"
    ? ConfigurationHelper.GetSetting("FILING_TEST_CERTIFICATE") ?? "organisation"
    : null;

Console.WriteLine($"Filing Service: {baseUri} (docs: {new Uri(baseUri, "docs/")})");
if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine("Notice: No FILING_API_KEY supplied. A Marketplace deployment rejects unauthenticated requests.");
}

try
{
    // ERiC checks and renders the large forms in seconds; a cold instance takes longer.
    using var http = new HttpClient { BaseAddress = baseUri, Timeout = TimeSpan.FromMinutes(4) };
    var exitCode = 0;

    if (mode is null or "rest")
    {
        Title("REST walkthrough");
        exitCode |= await new FilingBusinessCases(new FilingRestService(http, apiKey), referenceFilingPath, testCertificate).RunAsync();
    }

    if (mode is null or "mcp")
    {
        Title("MCP walkthrough");
        await using var client = await FilingMcpService.ConnectAsync(new Uri(baseUri, "mcp"), apiKey);
        exitCode |= await new FilingBusinessCases(new FilingMcpService(client), referenceFilingPath, testCertificate).RunAsync();
    }

    if (mode is "perf")
    {
        var concurrencies = (ConfigurationHelper.GetSetting("FILING_PERF_CONCURRENCY") ?? "1,5,10,25,50")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(int.Parse)
            .ToList();
        var requestsPerWorker = int.Parse(ConfigurationHelper.GetSetting("FILING_PERF_REQUESTS_PER_WORKER") ?? "10");
        var p95 = TimeSpan.FromMilliseconds(int.Parse(ConfigurationHelper.GetSetting("FILING_PERF_P95_MS") ?? "2000"));
        exitCode |= await new FilingPerformanceService(http, apiKey, referenceFilingPath, concurrencies, requestsPerWorker, p95).RunAsync();
    }

    return exitCode;
}
catch (Exception ex)
{
    // The one place where failures are reported; the services let every exception through.
    WalkthroughOutput.Write(ConsoleColor.Red, $"\n[ERROR] {ex.Message}");
    return 1;
}

static void Title(string title) =>
    WalkthroughOutput.Write(ConsoleColor.White, $"\n==================== {title} ====================");
