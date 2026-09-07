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
}
