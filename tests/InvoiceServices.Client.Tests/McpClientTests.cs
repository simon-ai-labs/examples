using System.IO.Pipelines;
using System.Text.Json;
using System.Text.Json.Nodes;
using InvoiceServices.Client.Mcp;
using InvoiceServices.Client.OpenApi.Generated.Models;
using Microsoft.Extensions.Logging.Abstractions;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace InvoiceServices.Client.Tests;

/// <summary>Runs the client against a real MCP server over in-memory pipes, so the protocol is exercised end to end.</summary>
public sealed class McpClientTests : IAsyncLifetime
{
    private readonly Pipe _toServer = new();
    private readonly Pipe _toClient = new();
    private readonly CancellationTokenSource _stop = new();
    private readonly List<JsonElement> _receivedRequests = [];
    private McpServer? _server;
    private Task? _serverRun;
    private int _listToolsCalls;

    public ValueTask InitializeAsync()
    {
        var options = new McpServerOptions
        {
            ServerInfo = new Implementation { Name = "fake-invoice-service", Version = "1.0.0" },
            ToolCollection =
            [
                McpServerTool.Create((JsonElement request) =>
                {
                    lock (_receivedRequests) _receivedRequests.Add(request.Clone());
                    return new JsonObject
                    {
                        ["valid"] = false,
                        ["issues"] = new JsonArray(new JsonObject { ["ruleId"] = "BR-DE-15", ["severity"] = "error", ["message"] = "BT-10 missing" }),
                    };
                }, new() { Name = "validate_invoice", UseStructuredContent = true }),
                McpServerTool.Create(CallToolResult () => new()
                {
                    IsError = true,
                    Content = [new TextContentBlock { Text = """{"code":"unsupported_invoice_format","title":"Unsupported invoice format.","detail":"Not an invoice."}""" }],
                }, new() { Name = "read_invoice" }),
            ],
        };
        options.Filters.Request.ListToolsFilters.Add(next => (context, ct) =>
        {
            Interlocked.Increment(ref _listToolsCalls);
            return next(context, ct);
        });
        _server = McpServer.Create(
            new StreamServerTransport(_toServer.Reader.AsStream(), _toClient.Writer.AsStream(), "fake", NullLoggerFactory.Instance),
            options, NullLoggerFactory.Instance);
        _serverRun = _server.RunAsync(_stop.Token);
        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await _stop.CancelAsync();
        if (_server is not null) await _server.DisposeAsync();
        _stop.Dispose();
    }

    private InvoiceMcpClient Client() => new(new PipeConnector(_toServer, _toClient), NullLogger<InvoiceMcpClient>.Instance);

    [Fact]
    public async Task ToolCallWrapsTheRequestAndReturnsTheTypedStructuredResult()
    {
        await using var client = Client();
        var result = await client.ValidateAsync(new ValidateInvoiceRequestDto
        {
            Document = new DocumentInputDto { Content = [1, 2, 3], FileName = "a.xml", ContentType = "application/xml" },
        }, TestContext.Current.CancellationToken);

        Assert.False(result.Valid);
        Assert.Equal("BR-DE-15", Assert.Single(result.Issues!).RuleId);
        Assert.Equal(ApiValidationSeverity.Error, result.Issues![0].Severity);
        var document = Assert.Single(_receivedRequests).GetProperty("document");
        Assert.Equal("AQID", document.GetProperty("content").GetString());
        Assert.Equal("a.xml", document.GetProperty("fileName").GetString());
    }

    [Fact]
    public async Task ToolErrorsSurfaceTheServiceProblem()
    {
        await using var client = Client();
        var failure = await Assert.ThrowsAsync<InvoiceMcpToolException>(() =>
            client.ReadAsync(new ReadInvoiceRequestDto { Document = new DocumentInputDto { Content = [1] } }, TestContext.Current.CancellationToken));

        Assert.Equal("read_invoice", failure.Tool);
        Assert.Equal("unsupported_invoice_format", failure.Code);
        Assert.Equal("Not an invoice.", failure.Detail);
    }

    [Fact]
    public async Task UnknownToolsAreRejectedBeforeCallingTheServer()
    {
        await using var client = Client();
        var failure = await Assert.ThrowsAsync<InvalidOperationException>(() => client.RenderAsync(new RenderInvoiceRequestDto(), TestContext.Current.CancellationToken));
        Assert.Contains("render_invoice", failure.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ToolListIsCachedAndExposedAsAIFunctions()
    {
        await using var client = Client();
        var ct = TestContext.Current.CancellationToken;
        var tools = await client.GetAIToolsAsync(ct);
        await client.ListToolsAsync(ct);
        await client.ValidateAsync(new ValidateInvoiceRequestDto { Document = new DocumentInputDto { Content = [1] } }, ct);

        Assert.Equal(["read_invoice", "validate_invoice"], tools.Select(t => t.Name).Order(StringComparer.Ordinal));
        Assert.Equal(1, Volatile.Read(ref _listToolsCalls));
    }

    [Fact]
    public async Task DisposingTwiceIsSafeAndBlocksFurtherCalls()
    {
        var client = Client();
        await client.ListToolsAsync(TestContext.Current.CancellationToken);
        await client.DisposeAsync();
        await client.DisposeAsync();
        await Assert.ThrowsAsync<ObjectDisposedException>(() => client.GetCapabilitiesAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public void ErrorWithoutProblemJsonKeepsTheText()
    {
        var failure = InvoiceMcpClient.ToException("x", new CallToolResult { IsError = true, Content = [new TextContentBlock { Text = "boom" }] });
        Assert.Equal("boom", failure.Title);
        Assert.Null(failure.Code);
    }

    private sealed class PipeConnector(Pipe toServer, Pipe toClient) : IMcpClientConnector
    {
        public Task<McpClient> ConnectAsync(McpClientOptions options, CancellationToken cancellationToken) =>
            McpClient.CreateAsync(new StreamClientTransport(toServer.Writer.AsStream(), toClient.Reader.AsStream(), NullLoggerFactory.Instance),
                options, NullLoggerFactory.Instance, cancellationToken);
    }
}
