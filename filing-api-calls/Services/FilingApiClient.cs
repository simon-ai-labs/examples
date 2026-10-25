using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FilingServices.Samples.Services;

/// <summary>
/// Thin HTTP client for one Datenart/Stand of the Filing Service, for example
/// <c>/api/ustva/2026</c>. Every Datenart/Stand exposes the same three routes:
/// <c>validate</c>, <c>preview</c> and <c>filings</c>.
/// </summary>
public sealed class FilingApiClient(HttpClient client, JsonSerializerOptions jsonOptions)
{
    public async Task<(bool Ready, string Body)> GetReadinessAsync()
    {
        using var response = await client.GetAsync("/health/ready");
        return (response.IsSuccessStatusCode, await response.Content.ReadAsStringAsync());
    }

    public async Task<JsonNode?> GetOpenApiIndexAsync()
    {
        using var response = await client.GetAsync("/openapi/index.json");
        await EnsureSuccessAsync(response, "GetOpenApiIndex");
        return await response.Content.ReadFromJsonAsync<JsonNode>(jsonOptions);
    }

    /// <summary>ELSTER schema and plausibility check. Nothing is printed or transmitted.</summary>
    public async Task<JsonNode?> ValidateAsync(string routePrefix, JsonNode data)
    {
        var request = new JsonObject { ["data"] = data.DeepClone() };
        using var response = await client.PostAsJsonAsync($"{routePrefix}/validate", request, jsonOptions);
        await EnsureSuccessAsync(response, "Validate");
        return await response.Content.ReadFromJsonAsync<JsonNode>(jsonOptions);
    }

    /// <summary>Same checks as <see cref="ValidateAsync"/>, plus the PDF. Nothing is transmitted.</summary>
    public async Task<JsonNode?> PreviewAsync(string routePrefix, JsonNode data)
    {
        var request = new JsonObject { ["data"] = data.DeepClone() };
        using var response = await client.PostAsJsonAsync($"{routePrefix}/preview", request, jsonOptions);
        await EnsureSuccessAsync(response, "Preview");
        return await response.Content.ReadFromJsonAsync<JsonNode>(jsonOptions);
    }

    /// <summary>
    /// Transmits the filing to ELSTER marked as a test case (<c>simulation: true</c>).
    /// ELSTER processes and then discards it. This sample never sends <c>simulation: false</c>.
    /// </summary>
    /// <remarks>
    /// Unlike validate and preview, a failed filing call must not be retried blindly: a 502, 500 or
    /// 499 after the native send began means the delivery outcome is unknown.
    /// </remarks>
    public async Task<JsonNode?> SimulateFilingAsync(string routePrefix, JsonNode data, byte[] pfx, string pin)
    {
        var request = new JsonObject
        {
            ["data"] = data.DeepClone(),
            ["certificate"] = new JsonObject
            {
                ["pfx"] = Convert.ToBase64String(pfx),
                ["pin"] = pin
            },
            ["simulation"] = true
        };

        using var response = await client.PostAsJsonAsync($"{routePrefix}/filings", request, jsonOptions);
        await EnsureSuccessAsync(response, "SimulateFiling");
        return await response.Content.ReadFromJsonAsync<JsonNode>(jsonOptions);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, string operation)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        // Errors are application/problem+json with type, title, status, detail and traceId.
        var problem = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"{operation} failed ({(int)response.StatusCode} {response.StatusCode}): {problem}",
            inner: null,
            statusCode: response.StatusCode);
    }
}
