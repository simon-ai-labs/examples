using System.Text.Json;
using InvoiceServices.Client.OpenApi.Generated.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Serialization.Json;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace InvoiceServices.Client.Mcp;

/// <summary>
/// Calls the invoice operations through the service's MCP endpoint. Requests and results use the same
/// models as the generated REST client, so both transports are interchangeable for callers.
/// </summary>
public interface IInvoiceMcpClient : IInvoiceOperations, IAsyncDisposable
{
    /// <summary>The tools the server currently offers; cached until the server reports a change.</summary>
    Task<IReadOnlyList<McpClientTool>> ListToolsAsync(CancellationToken cancellationToken = default);

    /// <summary>The tools as Microsoft.Extensions.AI functions, ready to hand to any <see cref="IChatClient"/>.</summary>
    Task<IReadOnlyList<AITool>> GetAIToolsAsync(CancellationToken cancellationToken = default);
}

/// <summary>A tool call the server answered with <c>isError: true</c>, carrying the service's problem details.</summary>
public sealed class InvoiceMcpToolException(string tool, string? code, string? title, string? detail)
    : Exception($"MCP tool '{tool}' failed: {title ?? code ?? "unknown error"}{(detail is null ? "" : $" {detail}")}")
{
    public string Tool { get; } = tool;
    public string? Code { get; } = code;
    public string? Title { get; } = title;
    public string? Detail { get; } = detail;
}

/// <summary>Opens MCP sessions. Separate from <see cref="InvoiceMcpClient"/> so tests can connect in memory.</summary>
public interface IMcpClientConnector
{
    Task<McpClient> ConnectAsync(McpClientOptions options, CancellationToken cancellationToken);
}

internal sealed partial class InvoiceMcpClient(IMcpClientConnector connector, ILogger<InvoiceMcpClient> logger) : IInvoiceMcpClient
{
    internal const string HttpClientName = "InvoiceServices.Mcp";

    private readonly SemaphoreSlim _gate = new(1, 1);
    private McpClient? _client;

    public string Transport => InvoiceTransports.Mcp;
    private IReadOnlyList<McpClientTool>? _tools;
    private int _disposed;

    public async Task<IReadOnlyList<McpClientTool>> ListToolsAsync(CancellationToken cancellationToken = default)
    {
        if (Volatile.Read(ref _tools) is { } cached) return cached;
        var client = await GetClientAsync(cancellationToken).ConfigureAwait(false);
        var tools = (await client.ListToolsAsync(cancellationToken: cancellationToken).ConfigureAwait(false)).ToArray();
        Volatile.Write(ref _tools, tools);
        return tools;
    }

    public async Task<IReadOnlyList<AITool>> GetAIToolsAsync(CancellationToken cancellationToken = default) =>
        [.. await ListToolsAsync(cancellationToken).ConfigureAwait(false)];

    public Task<CapabilitiesResponse> GetCapabilitiesAsync(CancellationToken cancellationToken = default) =>
        CallAsync("invoice_capabilities", null, CapabilitiesResponse.CreateFromDiscriminatorValue, cancellationToken);

    public Task<ValidateInvoiceResponseDto> ValidateAsync(ValidateInvoiceRequestDto request, CancellationToken cancellationToken = default) =>
        CallAsync("validate_invoice", request, ValidateInvoiceResponseDto.CreateFromDiscriminatorValue, cancellationToken);

    public Task<ReadInvoiceResponseDto> ReadAsync(ReadInvoiceRequestDto request, CancellationToken cancellationToken = default) =>
        CallAsync("read_invoice", request, ReadInvoiceResponseDto.CreateFromDiscriminatorValue, cancellationToken);

    public Task<ConvertInvoiceResponseDto> ConvertAsync(ConvertInvoiceRequestDto request, CancellationToken cancellationToken = default) =>
        CallAsync("convert_invoice", request, ConvertInvoiceResponseDto.CreateFromDiscriminatorValue, cancellationToken);

    public Task<RenderInvoiceResponseDto> RenderAsync(RenderInvoiceRequestDto request, CancellationToken cancellationToken = default) =>
        CallAsync("render_invoice", request, RenderInvoiceResponseDto.CreateFromDiscriminatorValue, cancellationToken);

    public Task<GenerateInvoiceResponseDto> GenerateAsync(GenerateInvoiceRequestDto request, CancellationToken cancellationToken = default) =>
        CallAsync("generate_invoice", request, GenerateInvoiceResponseDto.CreateFromDiscriminatorValue, cancellationToken);

    /// <summary>
    /// The service's tools take one <c>request</c> argument shaped like the REST request body and return
    /// the REST response body as structured content.
    /// </summary>
    internal async Task<T> CallAsync<T>(string tool, IParsable? request, ParsableFactory<T> factory, CancellationToken cancellationToken)
        where T : IParsable
    {
        var tools = await ListToolsAsync(cancellationToken).ConfigureAwait(false);
        if (!tools.Any(t => t.Name == tool))
            throw new InvalidOperationException($"The MCP server does not offer the tool '{tool}'.");

        Dictionary<string, object?> arguments = [];
        if (request is not null)
            arguments["request"] = Serialize(request);

        var client = await GetClientAsync(cancellationToken).ConfigureAwait(false);
        CallToolResult result;
        try
        {
            result = await client.CallToolAsync(tool, arguments, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is HttpRequestException or IOException)
        {
            // The HTTP pipeline already retried. Drop the session so the next call starts a fresh one.
            LogConnectionLost(tool, exception);
            await ResetAsync().ConfigureAwait(false);
            throw;
        }

        if (result.IsError == true) throw ToException(tool, result);
        var json = result.StructuredContent is { } structured
            ? structured.GetRawText()
            : result.Content.OfType<TextContentBlock>().FirstOrDefault()?.Text
              ?? throw new InvalidOperationException($"MCP tool '{tool}' returned no content.");
        return await DeserializeAsync(json, factory, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"MCP tool '{tool}' returned an empty result.");
    }

    // Kiota's JSON writer and parser are used directly: the static KiotaJsonSerializer depends on the
    // global registry, which is only populated once an InvoiceApiClient has been constructed.
    internal static JsonElement Serialize(IParsable value)
    {
        using var writer = new JsonSerializationWriter();
        writer.WriteObjectValue(null, value);
        using var content = writer.GetSerializedContent();
        using var document = JsonDocument.Parse(content);
        return document.RootElement.Clone();
    }

    internal static async Task<T?> DeserializeAsync<T>(string json, ParsableFactory<T> factory, CancellationToken cancellationToken)
        where T : IParsable
    {
        using var content = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
        var node = await new JsonParseNodeFactory().GetRootParseNodeAsync("application/json", content, cancellationToken).ConfigureAwait(false);
        return node.GetObjectValue(factory);
    }

    internal static InvoiceMcpToolException ToException(string tool, CallToolResult result)
    {
        var text = result.Content.OfType<TextContentBlock>().FirstOrDefault()?.Text;
        if (text is null) return new InvoiceMcpToolException(tool, null, null, null);
        try
        {
            using var problem = JsonDocument.Parse(text);
            string? Get(string name) => problem.RootElement.ValueKind == JsonValueKind.Object &&
                problem.RootElement.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;
            return new InvoiceMcpToolException(tool, Get("code"), Get("title"), Get("detail"));
        }
        catch (JsonException)
        {
            return new InvoiceMcpToolException(tool, null, text, null);
        }
    }

    private async Task<McpClient> GetClientAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) == 1, this);
        if (Volatile.Read(ref _client) is { } existing) return existing;
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_client is not null) return _client;
            var options = new McpClientOptions
            {
                ClientInfo = new Implementation { Name = "InvoiceServices.Client", Version = typeof(InvoiceMcpClient).Assembly.GetName().Version?.ToString() ?? "1.0.0" },
                Handlers = new McpClientHandlers
                {
                    // Servers announce tool changes; the cached list is refreshed on the next call.
                    NotificationHandlers =
                    [
                        new(NotificationMethods.ToolListChangedNotification, (_, _) =>
                        {
                            Volatile.Write(ref _tools, null);
                            return ValueTask.CompletedTask;
                        }),
                    ],
                },
            };
            var client = await connector.ConnectAsync(options, cancellationToken).ConfigureAwait(false);
            LogConnected(client.ServerInfo?.Name ?? "unknown", client.ServerInfo?.Version ?? "unknown");
            Volatile.Write(ref _client, client);
            return client;
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task ResetAsync()
    {
        await _gate.WaitAsync().ConfigureAwait(false);
        try
        {
            if (_client is not null) await _client.DisposeAsync().ConfigureAwait(false);
            _client = null;
            _tools = null;
        }
        finally
        {
            _gate.Release();
        }
    }

    // The same instance is also resolved as a keyed IInvoiceOperations, so the container may dispose it twice.
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 1) return;
        await ResetAsync().ConfigureAwait(false);
        _gate.Dispose();
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Connected to MCP server {ServerName} {ServerVersion}")]
    private partial void LogConnected(string serverName, string serverVersion);

    [LoggerMessage(Level = LogLevel.Warning, Message = "MCP connection lost during {Tool}; the next call reconnects")]
    private partial void LogConnectionLost(string tool, Exception exception);
}
