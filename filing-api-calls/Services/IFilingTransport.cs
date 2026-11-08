using System.Text.Json.Nodes;

namespace FilingServices.Samples.Services;

/// <summary>The answer of one call: the HTTP status the service chose and its JSON body.</summary>
public sealed record FilingResponse(int Status, JsonNode? Body);

/// <summary>One way to reach the filing operations: REST or MCP.</summary>
public interface IFilingTransport
{
    /// <summary>Short name used in console output and as the output folder.</summary>
    string Name { get; }

    /// <summary>The latest Stand of every Datenart, read from the service's own catalog and contracts.</summary>
    Task<IReadOnlyList<FilingStand>> LoadStandsAsync();

    /// <summary>Whether the transport offers the operation at all (MCP has no tool for the Abfrage routes).</summary>
    bool Supports(string operation);

    /// <summary>
    /// Sends one request body to one operation. A refusal (4xx/5xx) is returned, not thrown, so the
    /// business cases can check that the service refuses what it must refuse.
    /// </summary>
    Task<FilingResponse> CallAsync(FilingStand stand, FilingOperation operation, JsonNode body);
}
