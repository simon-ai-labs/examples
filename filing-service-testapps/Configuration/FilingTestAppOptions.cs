namespace FilingService.TestApps.Configuration;
public sealed class FilingTestAppOptions
{
    public Uri BaseUrl { get; init; } = new("http://localhost:8080/");
    public string AppName { get; init; } = "ustva-2026";
    public FilingTestOperation Operation { get; init; } = FilingTestOperation.Contract;
    public string? PayloadPath { get; init; }
    public string? CertificatePath { get; init; }
    public string? Pin { get; init; }
    public string ApiKeyHeader { get; init; } = "X-API-Key";
    public string? ApiKey { get; init; }
    public int TimeoutSeconds { get; init; } = 120;
    public bool List { get; init; }
    public bool All { get; init; }
    public bool VerifyCatalog { get; init; }

    public static FilingTestAppOptions Parse(string[] args)
    {
        var values = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        var flags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            if (!arg.StartsWith("--", StringComparison.Ordinal))
            {
                values["app"] = arg;
                continue;
            }

            var option = arg[2..];
            if (option is "list" or "all" or "verify-catalog")
            {
                flags.Add(option);
                continue;
            }

            if (i + 1 >= args.Length)
            {
                throw new ArgumentException($"Missing value for --{option}.");
            }

            values[option] = args[++i];
        }

        var rawBaseUrl = values.TryGetValue("base-url", out var explicitBaseUrl)
            ? explicitBaseUrl
            : Environment.GetEnvironmentVariable("FILING_SERVICE_BASE_URL");
        var baseUrlText = string.IsNullOrWhiteSpace(rawBaseUrl) ? "http://localhost:8080/" : rawBaseUrl;
        if (!Uri.TryCreate(baseUrlText, UriKind.Absolute, out var parsedBaseUrl))
        {
            throw new ArgumentException("FILING_SERVICE_BASE_URL/--base-url must be an absolute URL.");
        }

        if (!parsedBaseUrl.AbsolutePath.EndsWith("/", StringComparison.Ordinal))
        {
            parsedBaseUrl = new Uri(parsedBaseUrl + "/");
        }

        var operationText = values.TryGetValue("operation", out var rawOperation) ? rawOperation : "contract";
        if (!Enum.TryParse<FilingTestOperation>(operationText, true, out var operation))
        {
            throw new ArgumentException("Operation must be contract, validate, preview, simulate or submit.");
        }

        var apiKey = values.TryGetValue("api-key", out var rawApiKey)
            ? rawApiKey
            : Environment.GetEnvironmentVariable("FILING_SERVICE_API_KEY");
        var apiKeyHeader = values.TryGetValue("api-key-header", out var rawApiKeyHeader)
            ? rawApiKeyHeader ?? "X-API-Key"
            : Environment.GetEnvironmentVariable("FILING_SERVICE_API_KEY_HEADER") ?? "X-API-Key";
        var pin = values.TryGetValue("pin", out var rawPin)
            ? rawPin
            : Environment.GetEnvironmentVariable("FILING_SERVICE_PIN");

        return new FilingTestAppOptions
        {
            BaseUrl = parsedBaseUrl,
            AppName = values.TryGetValue("app", out var app) ? app ?? "ustva-2026" : "ustva-2026",
            Operation = operation,
            PayloadPath = values.GetValueOrDefault("payload"),
            CertificatePath = values.GetValueOrDefault("certificate"),
            Pin = pin,
            ApiKey = apiKey,
            ApiKeyHeader = apiKeyHeader,
            TimeoutSeconds = int.TryParse(values.GetValueOrDefault("timeout"), out var timeout) && timeout > 0 ? timeout : 120,
            List = flags.Contains("list"),
            All = flags.Contains("all"),
            VerifyCatalog = flags.Contains("verify-catalog")
        };
    }
}

