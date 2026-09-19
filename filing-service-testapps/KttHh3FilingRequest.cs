using System.Text.Json.Nodes;

namespace FilingService.TestApps;

public sealed record KttHh3FilingRequest(
    JsonNode Data,
    string CertificateBase64,
    string Pin,
    bool Simulation);

