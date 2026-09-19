using System.Text.Json.Nodes;

namespace FilingService.TestApps;

public sealed record E34a2025FilingRequest(
    JsonNode Data,
    string CertificateBase64,
    string Pin,
    bool Simulation);

