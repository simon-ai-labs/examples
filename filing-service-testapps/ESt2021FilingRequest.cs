using System.Text.Json.Nodes;

namespace FilingService.TestApps;

public sealed record ESt2021FilingRequest(
    JsonNode Data,
    string CertificateBase64,
    string Pin,
    bool Simulation);

