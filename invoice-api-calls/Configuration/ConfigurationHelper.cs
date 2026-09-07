using Microsoft.Extensions.Configuration;

namespace InvoiceServices.Samples.Configuration;

public static class ConfigurationHelper
{
    private const string DefaultRapidApiHost = "e-invoicing-service.p.rapidapi.com";
    private const string DefaultRapidApiBaseUrl = "https://e-invoicing-service.p.rapidapi.com";

    public static (string BaseUrl, string Host, string? ApiKey, bool IsLocal) ResolveConnectionSettings(string[] args)
    {
        string? apiKey = null;
        string baseUrl = DefaultRapidApiBaseUrl;
        string host = DefaultRapidApiHost;
        bool isLocal = false;

        // 1. Check CLI arguments
        if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
        {
            if (args[0].StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                args[0].StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                baseUrl = args[0].TrimEnd('/');
                isLocal = true;
                if (args.Length > 1)
                {
                    apiKey = args[1];
                }
            }
            else
            {
                apiKey = args[0];
            }
        }

        // 2. Check local .env file
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            apiKey = TryLoadKeyFromEnvFiles();
        }

        // 3. Check .NET User Secrets
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets(typeof(ConfigurationHelper).Assembly, optional: true)
                .Build();
            apiKey = config["RapidApiKey"] ?? config["RAPIDAPI_KEY"];
        }

        // 4. Check Environment variable
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            apiKey = Environment.GetEnvironmentVariable("RAPIDAPI_KEY");
        }

        // 5. Interactive prompt fallback
        if (!isLocal && string.IsNullOrWhiteSpace(apiKey))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Notice: No RAPIDAPI_KEY supplied!");
            Console.WriteLine("Usage: dotnet run --project invoice-api-calls/invoice-api-calls.csproj -- <YOUR_RAPIDAPI_KEY>");
            Console.WriteLine("Or set in .env, User Secrets, or env var RAPIDAPI_KEY.\n");
            Console.ResetColor();
            Console.Write("Enter your RapidAPI Key (or press ENTER to test against http://localhost:5000): ");
            apiKey = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                Console.WriteLine("\nFalling back to local development URL: http://localhost:5000");
                baseUrl = "http://localhost:5000";
                isLocal = true;
            }
        }

        return (baseUrl, host, apiKey, isLocal);
    }

    private static string? TryLoadKeyFromEnvFiles()
    {
        string[] searchPaths =
        [
            Path.Combine(AppContext.BaseDirectory, ".env"),
            Path.Combine(Directory.GetCurrentDirectory(), ".env"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", ".env"),
            Path.Combine(Directory.GetCurrentDirectory(), "invoice-api-calls", ".env"),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".env"),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".env")
        ];

        foreach (var path in searchPaths)
        {
            try
            {
                var fullPath = Path.GetFullPath(path);
                if (!File.Exists(fullPath))
                {
                    continue;
                }

                foreach (var line in File.ReadAllLines(fullPath))
                {
                    var trimmed = line.Trim();
                    if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith('#'))
                    {
                        continue;
                    }

                    var separatorIndex = trimmed.IndexOf('=');
                    if (separatorIndex <= 0)
                    {
                        continue;
                    }

                    var key = trimmed[..separatorIndex].Trim();
                    var val = trimmed[(separatorIndex + 1)..].Trim().Trim('"', '\'');

                    if (string.Equals(key, "RAPIDAPI_KEY", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(val))
                    {
                        return val;
                    }
                }
            }
            catch
            {
                // Ignore IO errors when scanning optional env paths
            }
        }

        return null;
    }
}
