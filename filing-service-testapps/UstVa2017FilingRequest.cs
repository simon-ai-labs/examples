using System.Text.Json.Nodes;

namespace FilingService.TestApps;

public sealed record UstVa2017FilingRequest(
    JsonNode Data,
    string CertificateBase64,
    string Pin,
    bool Simulation);

