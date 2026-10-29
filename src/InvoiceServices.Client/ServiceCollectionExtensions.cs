using InvoiceServices.Client;
using InvoiceServices.Client.Http;
using InvoiceServices.Client.Mcp;
using InvoiceServices.Client.OpenApi;
using InvoiceServices.Client.OpenApi.Generated;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Bundle;

namespace Microsoft.Extensions.DependencyInjection;

public static class InvoiceServicesClientServiceCollectionExtensions
{
    internal const string RestHttpClientName = "InvoiceServices.Rest";

    /// <summary>
    /// Registers the generated REST client (<see cref="InvoiceApiClient"/>), the docs reader
    /// (<see cref="InvoiceApiDocsReader"/>) and the MCP client (<see cref="IInvoiceMcpClient"/>).
    /// </summary>
    public static IServiceCollection AddInvoiceServicesClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<InvoiceServicesClientOptions>()
            .Bind(configuration.GetSection(InvoiceServicesClientOptions.SectionName))
            .ValidateOnStart();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<InvoiceServicesClientOptions>, InvoiceServicesClientOptionsValidator>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<InvoiceServicesClientOptions>, InvoiceServicesClientOptionsRules>());
        services.AddTransient<ApiKeyHandler>();

        // The docs are public: no key is sent to them.
        services.AddHttpClient<InvoiceApiDocsReader>(InvoiceApiDocsReader.HttpClientName, ConfigureClient)
            .AddStandardResilienceHandler().Configure(ConfigureResilience);

        services.AddHttpClient(RestHttpClientName, ConfigureClient)
            .AddStandardResilienceHandler().Configure(ConfigureResilience);
        services.AddHttpClient(InvoiceMcpClient.HttpClientName, ConfigureClient)
            .AddStandardResilienceHandler().Configure(ConfigureResilience);
        foreach (var name in (string[])[RestHttpClientName, InvoiceMcpClient.HttpClientName])
            services.AddHttpClient(name).AddHttpMessageHandler<ApiKeyHandler>();

        // Kiota's own retry and redirect handlers are left out: the resilience pipeline above owns retries.
        services.AddTransient(sp =>
        {
            var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient(RestHttpClientName);
            var adapter = new DefaultRequestAdapter(new AnonymousAuthenticationProvider(), httpClient: http)
            {
                BaseUrl = sp.GetRequiredService<IOptionsMonitor<InvoiceServicesClientOptions>>().CurrentValue.BaseAddress.ToString().TrimEnd('/'),
            };
            return new InvoiceApiClient(adapter);
        });

        services.TryAddSingleton<IMcpClientConnector, HttpMcpClientConnector>();
        services.TryAddSingleton<IInvoiceMcpClient, InvoiceMcpClient>();
        services.AddKeyedTransient<IInvoiceOperations, RestInvoiceOperations>(InvoiceTransports.Rest);
        services.AddKeyedSingleton<IInvoiceOperations>(InvoiceTransports.Mcp, (sp, _) => sp.GetRequiredService<IInvoiceMcpClient>());
        return services;
    }

    private static void ConfigureClient(IServiceProvider services, HttpClient client)
    {
        client.BaseAddress = services.GetRequiredService<IOptionsMonitor<InvoiceServicesClientOptions>>().CurrentValue.BaseAddress;
        // Timeouts belong to the resilience pipeline; HttpClient's own 100 s limit would cut it short.
        client.Timeout = Timeout.InfiniteTimeSpan;
    }

    /// <summary>
    /// The invoice operations are pure functions of their input and the service stores no documents,
    /// so retrying a POST after a transient 429/5xx or timeout cannot apply an operation twice.
    /// </summary>
    private static void ConfigureResilience(HttpStandardResilienceOptions resilience, IServiceProvider services)
    {
        var options = services.GetRequiredService<IOptionsMonitor<InvoiceServicesClientOptions>>().CurrentValue;
        resilience.AttemptTimeout.Timeout = options.AttemptTimeout;
        resilience.TotalRequestTimeout.Timeout = options.TotalTimeout;
        resilience.CircuitBreaker.SamplingDuration = options.AttemptTimeout * 2;
    }
}
