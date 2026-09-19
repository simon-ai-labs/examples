using System.Text.Json.Nodes;

namespace FilingService.TestApps.Models;
public sealed record LStA2019FilingRequest(
    JsonNode Data,
    string CertificateBase64,
    string Pin,
    bool Simulation);


