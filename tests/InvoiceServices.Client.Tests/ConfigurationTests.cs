using System.Net;
using InvoiceServices.Client;
using InvoiceServices.Client.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace InvoiceServices.Client.Tests;

public sealed class ConfigurationTests
{
    private const string ValidKey = "imp_0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef";

    private static IServiceProvider Services(Dictionary<string, string?> settings) =>
        new ServiceCollection()
            .AddLogging()
            .AddInvoiceServicesClient(new ConfigurationBuilder().AddInMemoryCollection(settings).Build())
            .BuildServiceProvider();

    [Theory]
    [InlineData("https://invoice-service-api.simon-ai-enterprise.de", ValidKey, null)]
    [InlineData("http://localhost:5000", "", null)]
    [InlineData("http://invoice-service-api.simon-ai-enterprise.de", ValidKey, "must use HTTPS")]
    [InlineData("https://invoice-service-api.simon-ai-enterprise.de", "", null)]
    [InlineData("https://invoice-service-api.simon-ai-enterprise.de", "imp_zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz", "Marketplace key")]
    [InlineData("https://invoice-service-api.simon-ai-enterprise.de", "imp_short", "Marketplace key")]
    public void OptionsAreValidatedAtStartup(string baseAddress, string apiKey, string? expectedFailure)
    {
        var services = Services(new() { ["InvoiceServices:BaseAddress"] = baseAddress, ["InvoiceServices:ApiKey"] = apiKey });
        var resolve = () => services.GetRequiredService<IOptions<InvoiceServicesClientOptions>>().Value;
        if (expectedFailure is null)
        {
            Assert.Equal(new Uri(baseAddress), resolve().BaseAddress);
            return;
        }
        var failure = Assert.Throws<OptionsValidationException>(resolve);
        Assert.Contains(failure.Failures, f => f.Contains(expectedFailure, StringComparison.Ordinal));
    }

    [Fact]
    public void TotalTimeoutMustExceedAttemptTimeout()
    {
        var services = Services(new()
        {
            ["InvoiceServices:ApiKey"] = ValidKey,
            ["InvoiceServices:AttemptTimeout"] = "00:01:00",
            ["InvoiceServices:TotalTimeout"] = "00:01:00",
        });
        var failure = Assert.Throws<OptionsValidationException>(() => services.GetRequiredService<IOptions<InvoiceServicesClientOptions>>().Value);
        Assert.Contains(failure.Failures, f => f.Contains("TotalTimeout", StringComparison.Ordinal));
    }

    [Fact]
    public void McpEndpointIsDerivedFromTheBaseAddress()
    {
        var options = new InvoiceServicesClientOptions { BaseAddress = new Uri("https://example.test/") };
        Assert.Equal(new Uri("https://example.test/mcp"), options.McpEndpoint);
    }

    [Fact]
    public async Task ApiKeyHandlerReplacesAnyCallerSuppliedKeyWithTheConfiguredOne()
    {
        var monitor = new StaticMonitor(new InvoiceServicesClientOptions { ApiKey = ValidKey });
        var recorder = new RecordingHandler();
        using var invoker = new HttpMessageInvoker(new ApiKeyHandler(monitor) { InnerHandler = recorder });
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.test/v1/capabilities");
        request.Headers.Add(ApiKeyHandler.HeaderName, "imp_from_caller");

        using var _ = await invoker.SendAsync(request, TestContext.Current.CancellationToken);

        Assert.Equal([ValidKey], recorder.LastRequest!.Headers.GetValues(ApiKeyHandler.HeaderName));
    }

    private sealed class StaticMonitor(InvoiceServicesClientOptions value) : IOptionsMonitor<InvoiceServicesClientOptions>
    {
        public InvoiceServicesClientOptions CurrentValue => value;
        public InvoiceServicesClientOptions Get(string? name) => value;
        public IDisposable? OnChange(Action<InvoiceServicesClientOptions, string?> listener) => null;
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
