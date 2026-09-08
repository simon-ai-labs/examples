using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace InvoiceServices.Samples.Services;

public sealed class EInvoicingApiClient(HttpClient client, JsonSerializerOptions jsonOptions)
{
    public async Task<JsonNode?> GetCapabilitiesAsync()
    {
        using var response = await client.GetAsync("/v1/capabilities");
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"GetCapabilities failed ({response.StatusCode}): {error}");
        }

        return await response.Content.ReadFromJsonAsync<JsonNode>(jsonOptions);
    }

    public async Task<JsonNode?> ValidateInvoiceAsync(string base64Content, string fileName, JsonObject? expectedRepresentation)
    {
        var request = new JsonObject
        {
            ["document"] = new JsonObject
            {
                ["content"] = base64Content,
                ["fileName"] = fileName,
                ["contentType"] = "application/xml",
                ["expectedRepresentation"] = expectedRepresentation
            }
        };

        using var response = await client.PostAsJsonAsync("/v1/invoices/validate", request, jsonOptions);
        return await response.Content.ReadFromJsonAsync<JsonNode>(jsonOptions);
    }

    public async Task<JsonNode?> ReadInvoiceAsync(string base64Content, string fileName)
    {
        var request = new JsonObject
        {
            ["document"] = new JsonObject
            {
                ["content"] = base64Content,
                ["fileName"] = fileName,
                ["contentType"] = "application/xml"
            }
        };

        using var response = await client.PostAsJsonAsync("/v1/invoices/read", request, jsonOptions);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"ReadInvoice failed ({response.StatusCode}): {error}");
        }

        return await response.Content.ReadFromJsonAsync<JsonNode>(jsonOptions);
    }

    public async Task<(string Base64Content, string FileName)> ConvertInvoiceAsync(
        string base64Content,
        string fileName,
        JsonObject target)
    {
        var request = new JsonObject
        {
            ["document"] = new JsonObject
            {
                ["content"] = base64Content,
                ["fileName"] = fileName,
                ["contentType"] = "application/xml"
            },
            ["target"] = target
        };

        using var response = await client.PostAsJsonAsync("/v1/invoices/convert", request, jsonOptions);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"ConvertInvoice failed ({response.StatusCode}): {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<JsonNode>(jsonOptions);
        var base64 = result?["document"]?["content"]?.GetValue<string>()
            ?? throw new InvalidOperationException("Converted document content is missing in API response.");
        var convertedFileName = result?["document"]?["fileName"]?.GetValue<string>() ?? "converted_invoice.xml";

        return (base64, convertedFileName);
    }

    public async Task<string> RenderInvoiceAsync(string base64Content, string fileName, string outputFormat)
    {
        var request = new JsonObject
        {
            ["document"] = new JsonObject
            {
                ["content"] = base64Content,
                ["fileName"] = fileName,
                ["contentType"] = "application/xml"
            },
            ["outputFormat"] = outputFormat
        };

        using var response = await client.PostAsJsonAsync("/v1/invoices/render", request, jsonOptions);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"RenderInvoice failed ({response.StatusCode}): {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<JsonNode>(jsonOptions);
        return result?["content"]?.GetValue<string>()
            ?? throw new InvalidOperationException($"Rendered {outputFormat} content is missing in API response.");
    }
}
