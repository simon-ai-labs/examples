using System.Text.Json.Nodes;

namespace FilingService.TestApps;

public sealed record UstVa2026FilingRequest(
    JsonNode Data,
    string CertificateBase64,
    string Pin,
    bool Simulation);

