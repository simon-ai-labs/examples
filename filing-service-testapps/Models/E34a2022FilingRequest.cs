using System.Text.Json.Nodes;

namespace FilingService.TestApps.Models;
public sealed record E34a2022FilingRequest(
    JsonNode Data,
    string CertificateBase64,
    string Pin,
    bool Simulation);


