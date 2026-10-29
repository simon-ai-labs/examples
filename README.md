# E-Invoicing Service Examples

Sample code for the E-Invoicing Service, which validates, reads, generates, converts and renders XRechnung, ZUGFeRD, Peppol and EN 16931 electronic invoices.

Service URL: [https://invoice-service-api.simon-ai-enterprise.de](https://invoice-service-api.simon-ai-enterprise.de) · [API documentation](https://invoice-service-api.simon-ai-enterprise.de/docs/)

## Projects

- **[invoice-api-calls](invoice-api-calls/)**: .NET console walkthrough for the REST API and MCP interface.
  - REST mode demonstrates capabilities, invoice validation, reading, conversion and rendering.
  - MCP mode connects to the service over Streamable HTTP, lists available tools and invokes the invoice operations.
  - Optional canonical invoice JSON demonstrates the `generate_invoice` tool.
- **[InvoiceServices.Client](src/InvoiceServices.Client/)**: reusable .NET 10 client library.
  - REST: a typed [Kiota](https://learn.microsoft.com/openapi/kiota/) client generated from the OpenAPI description that `/docs` renders.
  - Docs reader: reads `/docs`, finds and parses the OpenAPI description, and reports drift against the generated client.
  - MCP: a client for `/mcp` on the official [ModelContextProtocol](https://github.com/modelcontextprotocol/csharp-sdk) SDK. It uses the same request and response models as REST and exposes the tools as Microsoft.Extensions.AI functions.
  - Both transports implement `IInvoiceOperations` and are registered as keyed services (`"rest"`, `"mcp"`).
- **[invoice-client](src/InvoiceServices.Client.Cli/)**: command line on top of the library (`docs`, `rest`, `mcp`, `mcp tools`, `smoke`).
- **[InvoiceServices.Client.Tests](tests/InvoiceServices.Client.Tests/)**: xUnit v3 tests. MCP is tested against a real in-memory MCP server.

The bundled `01.06_minimal_test_ubl.xml` is an unmodified positive reference
invoice from the official [KoSIT XRechnung Test Suite](https://github.com/itplr-kosit/xrechnung-testsuite).
Its pinned upstream source and license are recorded in
[invoice-api-calls/TestInvoices/NOTICE.md](invoice-api-calls/TestInvoices/NOTICE.md).
The walkthrough also validates an in-memory negative variant without BuyerReference
(BT-10) and expects the XRechnung Schematron rule `BR-DE-15` to fail.

## Quick Start

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- Access to an Invoice Services API deployment
- An `X-API-Key` for Marketplace deployments

### Build

```bash
dotnet build examples.sln
```

### Run the REST walkthrough

For a local service:

```bash
dotnet run --project invoice-api-calls/invoice-api-calls.csproj -- rest http://localhost:5000
```

For a Marketplace deployment:

```bash
dotnet run --project invoice-api-calls/invoice-api-calls.csproj -- rest https://invoice-service-api.simon-ai-enterprise.de <YOUR_API_KEY>
```

The REST walkthrough sends an `Idempotency-Key` with each document operation. The API key can also be provided with the `INVOICE_API_KEY` environment variable or .NET User Secrets. The base URL can be set with `INVOICE_API_BASE_URL`.

### Run the MCP walkthrough

The MCP endpoint is `/mcp`. Pass the service base URL and optional Marketplace API key:

```bash
dotnet run --project invoice-api-calls/invoice-api-calls.csproj -- mcp https://invoice-service-api.simon-ai-enterprise.de <YOUR_API_KEY>
```

For local development:

```bash
dotnet run --project invoice-api-calls/invoice-api-calls.csproj -- mcp http://localhost:5000
```

MCP settings can also be supplied through environment variables:

```bash
export INVOICE_API_BASE_URL="https://invoice-service-api.simon-ai-enterprise.de"
export INVOICE_API_KEY="your-api-key"
dotnet run --project invoice-api-calls/invoice-api-calls.csproj -- mcp
```

Each MCP tool call sends its own `Idempotency-Key`. The walkthrough calls
`invoice_capabilities`, `validate_invoice`, `read_invoice`,
`convert_invoice` and `render_invoice` using the bundled KoSIT sample
invoice. To also call `generate_invoice`, point
`INVOICE_CANONICAL_JSON_PATH` to a JSON file containing an `InvoiceDto`:

```bash
export INVOICE_CANONICAL_JSON_PATH="./invoice.json"
dotnet run --project invoice-api-calls/invoice-api-calls.csproj -- mcp
```

The canonical invoice schema and supported target combinations are defined by
the service's `/openapi/v1.json` and `/v1/capabilities` endpoints.

Sample code notice: These examples are intentionally minimal and intended to demonstrate API integration. Production applications should add appropriate error handling, secret management, retries, logging and monitoring.

## Typed REST and MCP client (`InvoiceServices.Client`)

### Use it in your application

```csharp
builder.Services.AddInvoiceServicesClient(builder.Configuration);

// Pick a transport, or inject InvoiceApiClient / IInvoiceMcpClient directly.
var invoices = app.Services.GetRequiredKeyedService<IInvoiceOperations>(InvoiceTransports.Mcp);
var result = await invoices.ValidateAsync(new() { Document = new() { Content = xmlBytes, FileName = "invoice.xml" } });
```

Configuration section `InvoiceServices`:

| Key | Default | Notes |
|---|---|---|
| `BaseAddress` | `https://invoice-service-api.simon-ai-enterprise.de` | HTTPS required except for localhost |
| `ApiKey` | empty | Marketplace key `imp_…`. Use user secrets or `InvoiceServices__ApiKey`, never source control or the command line |
| `AttemptTimeout` | `00:01:30` | One HTTP attempt; PDF rendering under load can take tens of seconds |
| `TotalTimeout` | `00:04:00` | One call including retries |

Every HTTP client (REST, MCP, docs) goes through `IHttpClientFactory` with the standard resilience pipeline: retries on 408, 429 and 5xx with `Retry-After`, timeouts and a circuit breaker. The service stores no documents and every operation is a pure function of its input, so retrying a POST is safe. The API key is added by a delegating handler and never sent to `/docs`.

### Command line

```bash
dotnet user-secrets set InvoiceServices:ApiKey <YOUR_API_KEY> --project src/InvoiceServices.Client.Cli
dotnet run --project src/InvoiceServices.Client.Cli -- docs        # public, no key needed
dotnet run --project src/InvoiceServices.Client.Cli -- rest
dotnet run --project src/InvoiceServices.Client.Cli -- mcp tools
dotnet run --project src/InvoiceServices.Client.Cli -- smoke       # docs drift + REST and MCP give identical results
```

### When the API changes

`invoice-client docs`, and the daily `openapi-drift` CI job, compare the live description with the snapshot the client was generated from. To regenerate:

```bash
pwsh Build/generate-openapi-client.ps1
```

### Known issue in the API description

Decimal amounts such as `payableAmount` or `quantity` are described as `type: string` with a number pattern, while the service sends JSON numbers. Generated clients therefore type these fields as `string`, and Kiota reads the numbers as `null`. The fix belongs in the service's OpenAPI generation (`type: number`, or `["number", "string"]`). After that, regenerating the client is enough.
