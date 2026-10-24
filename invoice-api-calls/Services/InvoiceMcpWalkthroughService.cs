using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

namespace InvoiceServices.Samples.Services;

/// <summary>
/// Demonstrates calling the invoice service as an MCP server over Streamable HTTP.
/// The REST walkthrough remains available as the default program mode.
/// </summary>
public sealed class InvoiceMcpWalkthroughService(Uri endpoint, string? apiKey, string? canonicalInvoiceJsonPath)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private const string OfficialTestInvoiceFileName = "01.06_minimal_test_ubl.xml";

    public async Task<int> RunAsync()
    {
        var discoveryTransport = CreateTransport(Guid.NewGuid().ToString("N"));
        await using (var discoveryClient = await McpClient.CreateAsync(discoveryTransport))
        {
            var tools = await discoveryClient.ListToolsAsync();

            Console.WriteLine($"Connected to MCP endpoint: {endpoint}");
            Console.WriteLine("Available invoice tools:");
            foreach (var tool in tools)
            {
                Console.WriteLine($"  - {tool.Name}: {tool.Description}");
            }
        }

        await InvokeAsync("invoice_capabilities", new Dictionary<string, object?>());

        if (!string.IsNullOrWhiteSpace(canonicalInvoiceJsonPath))
        {
            var invoiceJson = await File.ReadAllTextAsync(canonicalInvoiceJsonPath);
            var invoice = JsonNode.Parse(invoiceJson)
                ?? throw new InvalidOperationException("The canonical invoice JSON file is empty.");
            await InvokeAsync("generate_invoice", new Dictionary<string, object?>
            {
                ["request"] = new Dictionary<string, object?>
                {
                    ["invoice"] = invoice,
                    ["target"] = new Dictionary<string, object?>
                    {
                        ["format"] = "xRechnung",
                        ["version"] = "3.0.2",
                        ["profile"] = "xRechnung",
                        ["syntax"] = "ubl"
                    }
                }
            });
        }
        else
        {
            Console.WriteLine("Set INVOICE_CANONICAL_JSON_PATH to also demonstrate generate_invoice.");
        }

        var invoicePath = Path.Combine(AppContext.BaseDirectory, "TestInvoices", OfficialTestInvoiceFileName);
        if (!File.Exists(invoicePath))
        {
            throw new FileNotFoundException("The bundled KoSIT test invoice is missing.", invoicePath);
        }

        var invoiceContent = Convert.ToBase64String(await File.ReadAllBytesAsync(invoicePath));
        var document = new Dictionary<string, object?>
        {
            ["content"] = invoiceContent,
            ["fileName"] = OfficialTestInvoiceFileName,
            ["contentType"] = "application/xml",
            ["expectedRepresentation"] = new Dictionary<string, object?>
            {
                ["format"] = "xRechnung",
                ["version"] = "3.0.2",
                ["profile"] = "xRechnung",
                ["syntax"] = "ubl"
            }
        };

        await InvokeAsync("validate_invoice", new Dictionary<string, object?>
        {
            ["request"] = new Dictionary<string, object?> { ["document"] = document }
        });

        await InvokeAsync("read_invoice", new Dictionary<string, object?>
        {
            ["request"] = new Dictionary<string, object?> { ["document"] = document }
        });

        await InvokeAsync("convert_invoice", new Dictionary<string, object?>
        {
            ["request"] = new Dictionary<string, object?>
            {
                ["document"] = document,
                ["target"] = new Dictionary<string, object?>
                {
                    ["format"] = "zugferd",
                    ["version"] = "2.5.2",
                    ["profile"] = "en16931",
                    ["syntax"] = "cii"
                }
            }
        });

        await InvokeAsync("render_invoice", new Dictionary<string, object?>
        {
            ["request"] = new Dictionary<string, object?>
            {
                ["document"] = document,
                ["outputFormat"] = "html"
            }
        });

        Console.WriteLine("MCP walkthrough completed.");
        return 0;
    }

    private HttpClientTransport CreateTransport(string idempotencyKey)
    {
        var headers = new Dictionary<string, string>
        {
            ["Idempotency-Key"] = idempotencyKey
        };
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            headers["X-API-Key"] = apiKey;
        }

        return new HttpClientTransport(new HttpClientTransportOptions
        {
            Endpoint = endpoint,
            TransportMode = HttpTransportMode.StreamableHttp,
            AdditionalHeaders = headers
        });
    }

    private async Task InvokeAsync(string toolName, Dictionary<string, object?> arguments)
    {
        // The Marketplace quota layer requires a fresh Idempotency-Key for each document tool call.
        var transport = CreateTransport(Guid.NewGuid().ToString("N"));
        await using var client = await McpClient.CreateAsync(transport);
        var tool = (await client.ListToolsAsync())
            .FirstOrDefault(candidate => string.Equals(candidate.Name, toolName, StringComparison.Ordinal))
            ?? throw new InvalidOperationException($"The MCP server does not expose the expected tool '{toolName}'.");

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n▶ Calling MCP tool: {toolName}");
        Console.ResetColor();

        var result = await tool.InvokeAsync(new AIFunctionArguments(arguments));
        Console.WriteLine(JsonSerializer.Serialize(result, JsonOptions));
    }
}
