using System.Text.Json.Nodes;

namespace InvoiceServices.Samples.Services;

/// <summary>One way to reach the invoice operations: REST or MCP.</summary>
public interface IInvoiceTransport
{
    /// <summary>Short name used in console output and as the output folder.</summary>
    string Name { get; }

    /// <summary>
    /// Calls an operation (<c>capabilities</c>, <c>validate</c>, <c>read</c>, <c>generate</c>, <c>convert</c>,
    /// <c>render</c>) with values addressed by dotted paths in the request body, for example
    /// <c>document.fileName</c>. The transport builds the request from its own schema.
    /// </summary>
    Task<JsonNode?> CallAsync(string operation, IReadOnlyDictionary<string, object?> values);
}
