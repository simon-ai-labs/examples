using System.Text.Json.Nodes;

namespace FilingService.TestApps;

public sealed record StAbS50a2026FilingRequest(
    JsonNode Data,
    string CertificateBase64,
    string Pin,
    bool Simulation);

