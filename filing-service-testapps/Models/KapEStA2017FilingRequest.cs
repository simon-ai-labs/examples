using System.Text.Json.Nodes;

namespace FilingService.TestApps.Models;
public sealed record KapEStA2017FilingRequest(
    JsonNode Data,
    string CertificateBase64,
    string Pin,
    bool Simulation);


