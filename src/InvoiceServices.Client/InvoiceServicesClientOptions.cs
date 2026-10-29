using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace InvoiceServices.Client;

/// <summary>Connection settings, bound from the <c>InvoiceServices</c> configuration section.</summary>
public sealed class InvoiceServicesClientOptions
{
    public const string SectionName = "InvoiceServices";

    /// <summary>Service root, for example <c>https://invoice-service-api.simon-ai-enterprise.de</c>.</summary>
    [Required]
    public Uri BaseAddress { get; set; } = new("https://invoice-service-api.simon-ai-enterprise.de");

    /// <summary>
    /// Marketplace API key (<c>imp_…</c>). Keep it in user secrets or an environment variable.
    /// Without one only the public <c>/docs</c> can be read.
    /// </summary>
    public string ApiKey { get; set; } = "";

    /// <summary>Upper bound for one HTTP attempt. Rendering a PDF under load can take tens of seconds.</summary>
    [Range(typeof(TimeSpan), "00:00:05", "00:05:00")]
    public TimeSpan AttemptTimeout { get; set; } = TimeSpan.FromSeconds(90);

    /// <summary>Upper bound for one logical call including retries.</summary>
    [Range(typeof(TimeSpan), "00:00:10", "00:15:00")]
    public TimeSpan TotalTimeout { get; set; } = TimeSpan.FromMinutes(4);

    /// <summary>The MCP endpoint, which the service exposes under <c>/mcp</c>.</summary>
    public Uri McpEndpoint => new(BaseAddress, "/mcp");
}

[OptionsValidator]
internal sealed partial class InvoiceServicesClientOptionsValidator : IValidateOptions<InvoiceServicesClientOptions>;

/// <summary>Rules the attributes cannot express.</summary>
internal sealed class InvoiceServicesClientOptionsRules : IValidateOptions<InvoiceServicesClientOptions>
{
    public ValidateOptionsResult Validate(string? name, InvoiceServicesClientOptions options)
    {
        List<string> failures = [];
        if (!options.BaseAddress.IsAbsoluteUri)
            failures.Add("InvoiceServices:BaseAddress must be an absolute URI.");
        else if (options.BaseAddress.Scheme != Uri.UriSchemeHttps && !options.BaseAddress.IsLoopback)
            failures.Add("InvoiceServices:BaseAddress must use HTTPS unless it points to localhost.");
        // An empty key is allowed: /docs is public, and calls to the API then fail with a clear 401.
        if (options.ApiKey.Length > 0 &&
            (options.ApiKey.Length != 68 || !options.ApiKey.StartsWith("imp_", StringComparison.Ordinal) || !options.ApiKey.Skip(4).All(char.IsAsciiHexDigit)))
            failures.Add("InvoiceServices:ApiKey must be a Marketplace key (imp_ followed by 64 hex characters).");
        if (options.TotalTimeout <= options.AttemptTimeout)
            failures.Add("InvoiceServices:TotalTimeout must be longer than AttemptTimeout.");
        return failures.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(failures);
    }
}
