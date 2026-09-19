using System.Text.Json.Nodes;

namespace FilingService.TestApps;

public sealed record LStA2016FilingRequest(
    JsonNode Data,
    string CertificateBase64,
    string Pin,
    bool Simulation);

