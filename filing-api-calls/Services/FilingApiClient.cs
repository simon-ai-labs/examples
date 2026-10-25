using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace FilingServices.Samples.Services;

public sealed class FilingApiClient(HttpClient client)
{
    public async Task<JsonNode?> PostAsync(string path, JsonObject request)
    {
        using var response = await client.PostAsJsonAsync(path, request);
        if (!response.IsSuccessStatusCode)
        {
            var problem = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"POST {path} failed ({(int)response.StatusCode}): {problem}");
        }

        return await response.Content.ReadFromJsonAsync<JsonNode>();
    }
}
