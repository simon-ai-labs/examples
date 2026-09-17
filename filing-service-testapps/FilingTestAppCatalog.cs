using System.Reflection;

namespace FilingService.TestApps;

public static class FilingTestAppCatalog
{
    public static IReadOnlyList<FilingTestAppRegistration> Create(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        var assembly = typeof(FilingTestAppCatalog).Assembly;
        var clients = assembly.GetTypes()
            .Where(type => !type.IsAbstract && typeof(FilingApiClientBase).IsAssignableFrom(type))
            .Select(type => (Type: type, Attribute: type.GetCustomAttribute<FilingTestAppAttribute>()))
            .Where(item => item.Attribute is not null)
            .ToDictionary(item => item.Attribute!.Name, StringComparer.OrdinalIgnoreCase);
        var services = assembly.GetTypes()
            .Where(type => !type.IsAbstract && typeof(FilingTestServiceBase).IsAssignableFrom(type))
            .Select(type => (Type: type, Attribute: type.GetCustomAttribute<FilingTestAppAttribute>()))
            .Where(item => item.Attribute is not null)
            .ToDictionary(item => item.Attribute!.Name, StringComparer.OrdinalIgnoreCase);

        var registrations = new List<FilingTestAppRegistration>(clients.Count);
        foreach (var (name, clientEntry) in clients.OrderBy(entry => entry.Key, StringComparer.OrdinalIgnoreCase))
        {
            if (!services.TryGetValue(name, out var serviceEntry))
            {
                throw new InvalidOperationException($"No matching service registered for client '{name}'.");
            }

            var client = (FilingApiClientBase)Activator.CreateInstance(clientEntry.Type, httpClient)!;
            var service = (FilingTestServiceBase)Activator.CreateInstance(serviceEntry.Type, client)!;
            registrations.Add(new FilingTestAppRegistration(
                name,
                service,
                client,
                clientEntry.Attribute!.RoutePrefix));
        }

        return registrations;
    }
}
