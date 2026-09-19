using FilingService.TestApps.Infrastructure;
using FilingService.TestApps.Services;
namespace FilingService.TestApps.Models;
public sealed record FilingTestAppRegistration(
    string Name,
    FilingTestServiceBase Service,
    FilingApiClientBase Client,
    string RoutePrefix);

