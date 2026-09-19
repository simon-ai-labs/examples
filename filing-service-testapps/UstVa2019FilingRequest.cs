using System.Text.Json.Nodes;

namespace FilingService.TestApps;

public sealed record UstVa2019FilingRequest(
    JsonNode Data,
    string CertificateBase64,
    string Pin,
    bool Simulation);

