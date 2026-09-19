using System.Text.Json.Nodes;

namespace FilingService.TestApps.Models;
public sealed record UstVa2018FilingRequest(
    JsonNode Data,
    string CertificateBase64,
    string Pin,
    bool Simulation);


