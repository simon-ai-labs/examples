using System.Text.Json.Nodes;

namespace FilingService.TestApps.Models;
public sealed record Aufz146a1FilingRequest(
    JsonNode Data,
    string CertificateBase64,
    string Pin,
    bool Simulation);


