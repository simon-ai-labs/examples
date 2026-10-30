using System.Text.Json.Nodes;

namespace MinimumTaxServices.Samples.Services;

/// <summary>One way to reach the minimum tax operations: REST or MCP.</summary>
public interface IMinimumTaxTransport
{
    /// <summary>Short name used in console output and as the output folder.</summary>
    string Name { get; }

    /// <summary>
    /// Calls an operation (<c>capabilities</c>, <c>gir/validate</c>, <c>gir/read</c>, <c>gir/generate</c>,
    /// <c>cbcr/validate</c>, <c>cbcr/read</c>) with values addressed by dotted paths in the request body, for
    /// example <c>validationContext.historyIsComplete</c>. The transport builds the request from its own schema.
    /// </summary>
    Task<JsonNode?> CallAsync(string operation, IReadOnlyDictionary<string, object?> values);
}
