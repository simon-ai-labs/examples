using System.Text.Json.Nodes;

namespace FilingService.TestApps.Models;
public sealed record StAbS50a2025FilingRequest(
    JsonNode Data,
    string CertificateBase64,
    string Pin,
    bool Simulation);


