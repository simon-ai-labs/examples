using System.Text.Json;
using FilingServices.Samples.Configuration;
using FilingServices.Samples.Services;

namespace FilingServices.Samples;

public static class Program
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public static async Task<int> Main(string[] args)
    {
        var (baseUrl, apiKey) = ConfigurationHelper.ResolveConnectionSettings(args);
        var certificatePath = ConfigurationHelper.GetSetting("FILING_CERTIFICATE_PATH");
        var certificatePin = ConfigurationHelper.GetSetting("FILING_CERTIFICATE_PIN");

        Console.WriteLine("==================================================================");
        Console.WriteLine("    Filing Service — REST API Walkthrough (UStVA 2026)");
        Console.WriteLine("==================================================================\n");

        // Checks and especially transmissions can take noticeably longer than a typical REST call.
        using var client = new HttpClient
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = TimeSpan.FromMinutes(3)
        };
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            client.DefaultRequestHeaders.Add("X-API-Key", apiKey);
        }

        var walkthrough = new FilingWalkthroughService(
            new FilingApiClient(client, JsonOptions),
            certificatePath,
            certificatePin);

        try
        {
            return await walkthrough.RunAllStepsAsync();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[ERROR] Filing sample failed: {ex.Message}");
            Console.ResetColor();
            return 1;
        }
    }
}
