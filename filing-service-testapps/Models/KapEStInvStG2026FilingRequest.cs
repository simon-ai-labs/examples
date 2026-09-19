using System.Text.Json.Nodes;

namespace FilingService.TestApps.Models;
public sealed record KapEStInvStG2026FilingRequest(
    JsonNode Data,
    string CertificateBase64,
    string Pin,
    bool Simulation);


