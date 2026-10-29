using InvoiceServices.Client.OpenApi.Generated;
using InvoiceServices.Client.OpenApi.Generated.Models;

namespace InvoiceServices.Client.OpenApi;

/// <summary>The business operations over REST, through the Kiota client generated from <c>/docs</c>.</summary>
internal sealed class RestInvoiceOperations(InvoiceApiClient api) : IInvoiceOperations
{
    public string Transport => InvoiceTransports.Rest;

    public async Task<CapabilitiesResponse> GetCapabilitiesAsync(CancellationToken cancellationToken = default) =>
        await api.V1.Capabilities.GetAsync(cancellationToken: cancellationToken).ConfigureAwait(false) ?? throw Empty("capabilities");

    public async Task<ValidateInvoiceResponseDto> ValidateAsync(ValidateInvoiceRequestDto request, CancellationToken cancellationToken = default) =>
        await api.V1.Invoices.Validate.PostAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false) ?? throw Empty("validate");

    public async Task<ReadInvoiceResponseDto> ReadAsync(ReadInvoiceRequestDto request, CancellationToken cancellationToken = default) =>
        await api.V1.Invoices.Read.PostAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false) ?? throw Empty("read");

    public async Task<ConvertInvoiceResponseDto> ConvertAsync(ConvertInvoiceRequestDto request, CancellationToken cancellationToken = default) =>
        await api.V1.Invoices.Convert.PostAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false) ?? throw Empty("convert");

    public async Task<RenderInvoiceResponseDto> RenderAsync(RenderInvoiceRequestDto request, CancellationToken cancellationToken = default) =>
        await api.V1.Invoices.Render.PostAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false) ?? throw Empty("render");

    public async Task<GenerateInvoiceResponseDto> GenerateAsync(GenerateInvoiceRequestDto request, CancellationToken cancellationToken = default) =>
        await api.V1.Invoices.Generate.PostAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false) ?? throw Empty("generate");

    private static InvalidOperationException Empty(string operation) => new($"The service returned an empty {operation} response.");
}
