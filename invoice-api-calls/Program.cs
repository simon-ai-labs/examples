using System.Text.Json;
using InvoiceServices.Samples.Configuration;
using InvoiceServices.Samples.Services;

namespace InvoiceServices.Samples;

public static class Program
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public static async Task<int> Main(string[] args)
    {
        if (args.Length > 0 && string.Equals(args[0], "mcp", StringComparison.OrdinalIgnoreCase))
        {
            var (baseUrl, apiKey) = ConfigurationHelper.ResolveConnectionSettings(args.Skip(1).Take(2).ToArray());
            var invoiceJsonPath = args.Length > 3
                ? args[3]
                : ConfigurationHelper.GetSetting("INVOICE_CANONICAL_JSON_PATH");
            var endpoint = CreateMcpEndpoint(baseUrl);

            Console.WriteLine("==============================================================");
            Console.WriteLine("    Invoice Services — MCP Walkthrough");
            Console.WriteLine("==============================================================\n");

            try
            {
                return await new InvoiceMcpWalkthroughService(endpoint, apiKey, invoiceJsonPath).RunAsync();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[ERROR] MCP walkthrough failed: {ex.Message}");
                Console.ResetColor();
                return 1;
            }
        }

        var restArgs = args.Length > 0 && string.Equals(args[0], "rest", StringComparison.OrdinalIgnoreCase)
            ? args.Skip(1).ToArray()
            : args;
        var (restBaseUrl, restApiKey) = ConfigurationHelper.ResolveConnectionSettings(restArgs);

        Console.WriteLine("==================================================================");
        Console.WriteLine("    Invoice Services — REST API Walkthrough");
        Console.WriteLine("    API:  https://invoice-service-api.simon-ai-enterprise.de");
        Console.WriteLine("    Docs: https://invoice-service-api.simon-ai-enterprise.de/docs/");
        Console.WriteLine("==================================================================\n");

        using var client = new HttpClient { BaseAddress = new Uri(restBaseUrl) };
        if (!string.IsNullOrWhiteSpace(restApiKey))
        {
            client.DefaultRequestHeaders.Add("X-API-Key", restApiKey);
        }
        else if (!IsLocalUrl(restBaseUrl))
        {
            Console.WriteLine("Notice: No INVOICE_API_KEY supplied. A Marketplace deployment will reject unauthenticated requests.\n");
        }

        var apiClient = new EInvoicingApiClient(client, JsonOptions);
        var walkthrough = new EInvoicingWalkthroughService(apiClient);

        try
        {
            return await walkthrough.RunAllStepsAsync();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[ERROR] REST walkthrough failed: {ex.Message}");
            Console.ResetColor();
            return 1;
        }
    }

    private static bool IsLocalUrl(string baseUrl) =>
        Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri)
        && (uri.IsLoopback || string.Equals(uri.Host, "localhost", StringComparison.OrdinalIgnoreCase));

    private static Uri CreateMcpEndpoint(string baseUrl)
    {
        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri))
        {
            throw new ArgumentException("The MCP base URL must be an absolute HTTP or HTTPS URL.", nameof(baseUrl));
        }

        if (uri.Scheme is not ("http" or "https"))
        {
            throw new ArgumentException("The MCP base URL must use HTTP or HTTPS.", nameof(baseUrl));
        }

        var endpoint = uri.AbsolutePath.TrimEnd('/');
        if (!endpoint.EndsWith("/mcp", StringComparison.OrdinalIgnoreCase))
        {
            endpoint += "/mcp";
        }

        return new UriBuilder(uri) { Path = endpoint }.Uri;
    }
}
