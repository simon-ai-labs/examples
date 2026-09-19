using FilingService.TestApps.Configuration;
using FilingService.TestApps.Infrastructure;
using FilingService.TestApps.Models;
using FilingService.TestApps.Services;
using System.Text.Json;

public static class FilingTestAppHost
{
    public static async Task<int> RunAsync(string[] args)
    {
        var options = FilingTestAppOptions.Parse(args);

        using var httpClient = new HttpClient
        {
            BaseAddress = options.BaseUrl,
            Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds)
        };

        if (!string.IsNullOrWhiteSpace(options.ApiKey))
        {
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation(options.ApiKeyHeader, options.ApiKey);
        }

        var catalog = FilingTestAppCatalog.Create(httpClient);

        if (options.List)
        {
            foreach (var app in catalog)
            {
                Console.WriteLine($"{app.Name,-40} {app.RoutePrefix}");
            }

            Console.WriteLine();
            Console.WriteLine($"Listed {catalog.Count} Filing Service testapps.");
            return 0;
        }

        if (options.VerifyCatalog)
        {
            return await VerifyCatalogAsync(httpClient, catalog);
        }

        if (options.All)
        {
            var failures = 0;
            foreach (var app in catalog)
            {
                try
                {
                    await app.Service.RunAsync(FilingTestOperation.Contract, null, null, null);
                    Console.WriteLine($"PASS {app.Name}");
                }
                catch (Exception ex)
                {
                    failures++;
                    Console.Error.WriteLine($"FAIL {app.Name}: {ex.Message}");
                }
            }

            Console.WriteLine($"Contract smoke finished: {catalog.Count - failures}/{catalog.Count} passed.");
            return failures == 0 ? 0 : 1;
        }

        var selected = catalog.FirstOrDefault(app =>
            string.Equals(app.Name, options.AppName, StringComparison.OrdinalIgnoreCase));

        if (selected is null)
        {
            Console.Error.WriteLine($"Unknown testapp '{options.AppName}'. Use --list to see all names.");
            return 2;
        }

        try
        {
            var result = await selected.Service.RunAsync(
                options.Operation,
                options.PayloadPath,
                options.CertificatePath,
                options.Pin);
            Console.WriteLine($"PASS {selected.Name} ({options.Operation.ToString().ToLowerInvariant()})");
            return result;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"FAIL {selected.Name}: {ex.Message}");
            return 1;
        }
    }

    private static async Task<int> VerifyCatalogAsync(
        HttpClient httpClient,
        IReadOnlyList<FilingTestAppRegistration> registrations)
    {
        using var response = await httpClient.GetAsync("openapi/index.json");
        var body = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            Console.Error.WriteLine($"OpenAPI catalog request failed ({(int)response.StatusCode}): {body}");
            return 1;
        }

        using var document = JsonDocument.Parse(body);
        if (!document.RootElement.TryGetProperty("documents", out var documents)
            || documents.ValueKind != JsonValueKind.Array)
        {
            Console.Error.WriteLine("The Filing Service catalog has no 'documents' array.");
            return 1;
        }

        var clients = registrations
            .Select(registration => registration.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missing = documents.EnumerateArray()
            .Select(entry => entry.GetProperty("name").GetString())
            .Where(name => name is not null && !clients.Contains(name))
            .ToArray();

        Console.WriteLine($"OpenAPI catalog: {documents.GetArrayLength()} documents; registered clients: {clients.Count}.");
        if (missing.Length > 0)
        {
            Console.Error.WriteLine("Missing clients:");
            foreach (var name in missing)
            {
                Console.Error.WriteLine($"  - {name}");
            }

            return 1;
        }

        return 0;
    }
}
