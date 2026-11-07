using FilingServices.Samples.Services;

namespace FilingServices.Samples;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var baseUrl = args.ElementAtOrDefault(0)
            ?? Environment.GetEnvironmentVariable("FILING_API_BASE_URL")
            ?? "http://localhost:8080";
        var apiKey = args.ElementAtOrDefault(1) ?? Environment.GetEnvironmentVariable("FILING_API_KEY");

        using var client = new HttpClient { BaseAddress = new Uri(baseUrl), Timeout = TimeSpan.FromMinutes(3) };
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            client.DefaultRequestHeaders.Add("X-API-Key", apiKey);
        }

        var walkthrough = new FilingWalkthroughService(
            new FilingApiClient(client),
            Environment.GetEnvironmentVariable("FILING_CERTIFICATE_PATH"),
            Environment.GetEnvironmentVariable("FILING_CERTIFICATE_PIN"));

        try
        {
            return await walkthrough.RunAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n[ERROR] {ex.Message}");
            return 1;
        }
    }
}
