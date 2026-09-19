namespace FilingService.TestApps;

public sealed record FilingTestAppRegistration(
    string Name,
    FilingTestServiceBase Service,
    FilingApiClientBase Client,
    string RoutePrefix);
