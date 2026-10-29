using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Client;

namespace InvoiceServices.Client.Mcp;

/// <summary>
/// Connects over Streamable HTTP using the factory's <c>InvoiceServices.Mcp</c> client, so MCP traffic gets
/// the same API key handler and resilience pipeline as the REST client.
/// </summary>
internal sealed class HttpMcpClientConnector(
    IHttpClientFactory httpClientFactory,
    IOptionsMonitor<InvoiceServicesClientOptions> options,
    ILoggerFactory loggerFactory) : IMcpClientConnector
{
    public async Task<McpClient> ConnectAsync(McpClientOptions clientOptions, CancellationToken cancellationToken)
    {
        var transport = new HttpClientTransport(
            new HttpClientTransportOptions
            {
                Endpoint = options.CurrentValue.McpEndpoint,
                TransportMode = HttpTransportMode.StreamableHttp,
                Name = "invoice-services",
            },
            httpClientFactory.CreateClient(InvoiceMcpClient.HttpClientName),
            loggerFactory,
            ownsHttpClient: true);
        return await McpClient.CreateAsync(transport, clientOptions, loggerFactory, cancellationToken).ConfigureAwait(false);
    }
}
