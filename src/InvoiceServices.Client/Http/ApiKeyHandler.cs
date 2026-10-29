using Microsoft.Extensions.Options;

namespace InvoiceServices.Client.Http;

/// <summary>
/// Adds the Marketplace key to every request of the REST, docs and MCP clients. The key is read per
/// request, so a rotated key from reloadable configuration applies without a restart.
/// </summary>
internal sealed class ApiKeyHandler(IOptionsMonitor<InvoiceServicesClientOptions> options) : DelegatingHandler
{
    public const string HeaderName = "X-API-Key";

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Remove(HeaderName);
        var key = options.CurrentValue.ApiKey;
        if (!string.IsNullOrEmpty(key)) request.Headers.TryAddWithoutValidation(HeaderName, key);
        return base.SendAsync(request, cancellationToken);
    }
}
