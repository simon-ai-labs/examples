using MinimumTaxServices.Samples.Configuration;
using MinimumTaxServices.Samples.Services;

// Usage: [rest|mcp] [baseUrl] [apiKey]. Without a mode both walkthroughs run.
var mode = args.FirstOrDefault()?.ToLowerInvariant() is "rest" or "mcp" ? args[0].ToLowerInvariant() : null;
var (baseUrl, apiKey) = ConfigurationHelper.ResolveConnectionSettings(mode is null ? args : args[1..]);
var documentsPath = Path.Combine(AppContext.BaseDirectory, "TestDocuments");
var baseUri = new Uri(baseUrl.TrimEnd('/') + "/");

Console.WriteLine($"Minimum Tax API: {baseUri} (docs: {new Uri(baseUri, "docs/")})");
if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine("Notice: No MINIMUM_TAX_API_KEY supplied. A Marketplace deployment rejects unauthenticated requests.");
}

try
{
    var exitCode = 0;
    if (mode is null or "rest")
    {
        using var http = new HttpClient { BaseAddress = baseUri, Timeout = TimeSpan.FromMinutes(4) };
        exitCode |= await new MinimumTaxRestService(http, apiKey, documentsPath).RunAsync();
    }

    if (mode is null or "mcp")
    {
        exitCode |= await new MinimumTaxMcpService(new Uri(baseUri, "mcp"), apiKey, documentsPath).RunAsync();
    }

    return exitCode;
}
catch (Exception ex)
{
    // The one place where failures are reported; the services let every exception through.
    WalkthroughOutput.Write(ConsoleColor.Red, $"\n[ERROR] {ex.Message}");
    return 1;
}
