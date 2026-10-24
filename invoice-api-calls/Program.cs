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
            var baseUrl = args.Length > 1
                ? args[1]
                : Environment.GetEnvironmentVariable("INVOICE_API_BASE_URL") ?? "http://localhost:5000";
            var apiKey = args.Length > 2
                ? args[2]
                : Environment.GetEnvironmentVariable("INVOICE_API_KEY");
            var invoiceJsonPath = args.Length > 3
                ? args[3]
                : Environment.GetEnvironmentVariable("INVOICE_CANONICAL_JSON_PATH");
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

        Console.WriteLine("==================================================================");
        Console.WriteLine("    RapidAPI E-Invoicing Service — Step-by-Step Walkthrough      ");
        Console.WriteLine("    https://rapidapi.com/schickmaster/api/european-german-e-invoicing-service");
        Console.WriteLine("==================================================================\n");

        // 1. Resolve configuration (CLI args -> .env -> User Secrets -> Environment Variables)
        var (baseUrl, host, apiKey, isLocal) = ConfigurationHelper.ResolveConnectionSettings(args);

        // 2. Setup HTTP Client
        using var client = new HttpClient { BaseAddress = new Uri(baseUrl) };

        if (!isLocal && !string.IsNullOrWhiteSpace(apiKey))
        {
            client.DefaultRequestHeaders.Add("x-rapidapi-key", apiKey);
            client.DefaultRequestHeaders.Add("x-rapidapi-host", host);
        }

        // 3. Initialize services
        var apiClient = new EInvoicingApiClient(client, JsonOptions);
        var walkthrough = new EInvoicingWalkthroughService(apiClient);

        // 4. Execute the step-by-step walkthrough
        try
        {
            return await walkthrough.RunAllStepsAsync();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[ERROR] Walkthrough failed: {ex.Message}");
            Console.ResetColor();
            return 1;
        }
    }

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
