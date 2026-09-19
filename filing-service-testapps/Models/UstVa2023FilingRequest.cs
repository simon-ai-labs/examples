using System.Text.Json.Nodes;

namespace FilingService.TestApps.Models;
public sealed record UstVa2023FilingRequest(
    JsonNode Data,
    string CertificateBase64,
    string Pin,
    bool Simulation);


