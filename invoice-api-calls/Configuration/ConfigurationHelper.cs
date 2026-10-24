using Microsoft.Extensions.Configuration;

namespace InvoiceServices.Samples.Configuration;

public static class ConfigurationHelper
{
    private const string DefaultBaseUrl = "http://localhost:5000";

    /// <summary>
    /// Resolves the Invoice API base URL and API key.
    /// CLI arguments may be either [baseUrl, apiKey] or [apiKey].
    /// </summary>
    public static (string BaseUrl, string? ApiKey) ResolveConnectionSettings(string[] args)
    {
        var baseUrl = GetSetting("INVOICE_API_BASE_URL") ?? DefaultBaseUrl;
        var apiKey = GetSetting("INVOICE_API_KEY");

        if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
        {
            if (Uri.TryCreate(args[0], UriKind.Absolute, out var suppliedUrl)
                && suppliedUrl.Scheme is "http" or "https")
            {
                baseUrl = args[0].TrimEnd('/');
                if (args.Length > 1 && !string.IsNullOrWhiteSpace(args[1]))
                {
                    apiKey = args[1];
                }
            }
            else
            {
                apiKey = args[0];
            }
        }

        return (baseUrl, apiKey);
    }

    public static string? GetSetting(string name)
    {
        var environmentValue = Environment.GetEnvironmentVariable(name);
        if (!string.IsNullOrWhiteSpace(environmentValue))
        {
            return environmentValue;
        }

        try
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets(typeof(ConfigurationHelper).Assembly, optional: true)
                .Build();
            var secretValue = configuration[name];
            if (!string.IsNullOrWhiteSpace(secretValue))
            {
                return secretValue;
            }
        }
        catch
        {
            // User Secrets are optional for this sample.
        }

        return TryLoadSettingFromEnvFiles(name);
    }

    private static string? TryLoadSettingFromEnvFiles(string name)
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
                    var value = trimmed[(separatorIndex + 1)..].Trim().Trim('"', '\'');

                    if (string.Equals(key, name, StringComparison.OrdinalIgnoreCase)
                        && !string.IsNullOrWhiteSpace(value))
                    {
                        return value;
                    }
                }
            }
            catch
            {
                // Ignore optional .env files that cannot be read.
            }
        }

        return null;
    }
}
