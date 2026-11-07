using InvoiceServices.Samples.Configuration;
using InvoiceServices.Samples.Services;

// Usage: [rest|mcp] [baseUrl] [apiKey]. Without a mode both walkthroughs run.
var mode = args.FirstOrDefault()?.ToLowerInvariant() is "rest" or "mcp" ? args[0].ToLowerInvariant() : null;
var (baseUrl, apiKey) = ConfigurationHelper.ResolveConnectionSettings(mode is null ? args : args[1..]);
var invoicePath = Path.Combine(AppContext.BaseDirectory, "TestInvoices", "01.06_minimal_test_ubl.xml");
var baseUri = new Uri(baseUrl.TrimEnd('/') + "/");
var canonicalInvoiceJsonPath = ConfigurationHelper.GetSetting("INVOICE_CANONICAL_JSON_PATH");

Console.WriteLine($"Invoice Services: {baseUri} (docs: {new Uri(baseUri, "docs/")})");
if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine("Notice: No INVOICE_API_KEY supplied. A Marketplace deployment rejects unauthenticated requests.");
}

try
{
    var exitCode = 0;
    if (mode is null or "rest")
    {
        // PDF rendering under load can take longer than HttpClient's default 100 seconds.
        using var http = new HttpClient { BaseAddress = baseUri, Timeout = TimeSpan.FromMinutes(4) };
        exitCode |= await new InvoiceRestService(http, apiKey, invoicePath, canonicalInvoiceJsonPath).RunAsync();
    }

    if (mode is null or "mcp")
    {
        exitCode |= await new InvoiceMcpService(new Uri(baseUri, "mcp"), apiKey, invoicePath, canonicalInvoiceJsonPath).RunAsync();
    }

    return exitCode;
}
catch (Exception ex)
{
    // The one place where failures are reported; the services let every exception through.
    WalkthroughOutput.Write(ConsoleColor.Red, $"\n[ERROR] {ex.Message}");
    return 1;
}
