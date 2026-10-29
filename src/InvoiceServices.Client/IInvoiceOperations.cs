using InvoiceServices.Client.OpenApi.Generated.Models;

namespace InvoiceServices.Client;

/// <summary>
/// The service's business operations, independent of the transport. Resolve the REST or the MCP
/// implementation with <see cref="InvoiceTransports"/> as the service key.
/// </summary>
public interface IInvoiceOperations
{
    string Transport { get; }
    Task<CapabilitiesResponse> GetCapabilitiesAsync(CancellationToken cancellationToken = default);
    Task<ValidateInvoiceResponseDto> ValidateAsync(ValidateInvoiceRequestDto request, CancellationToken cancellationToken = default);
    Task<ReadInvoiceResponseDto> ReadAsync(ReadInvoiceRequestDto request, CancellationToken cancellationToken = default);
    Task<ConvertInvoiceResponseDto> ConvertAsync(ConvertInvoiceRequestDto request, CancellationToken cancellationToken = default);
    Task<RenderInvoiceResponseDto> RenderAsync(RenderInvoiceRequestDto request, CancellationToken cancellationToken = default);
    Task<GenerateInvoiceResponseDto> GenerateAsync(GenerateInvoiceRequestDto request, CancellationToken cancellationToken = default);
}

/// <summary>Service keys for <see cref="IInvoiceOperations"/>.</summary>
public static class InvoiceTransports
{
    public const string Rest = "rest";
    public const string Mcp = "mcp";
}
