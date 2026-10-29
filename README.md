# E-Invoicing Service Examples

Sample code for the E-Invoicing Service, which validates, reads, generates, converts and renders XRechnung, ZUGFeRD, Peppol and EN 16931 electronic invoices.

Service URL: [https://invoice-service-api.simon-ai-enterprise.de](https://invoice-service-api.simon-ai-enterprise.de) · [API documentation](https://invoice-service-api.simon-ai-enterprise.de/docs/)

## Projects

- **[invoice-api-calls](invoice-api-calls/)**: .NET console walkthrough for the REST API and MCP interface. It has no request or response classes: requests are built at runtime from the service's own JSON definitions.
  - `InvoiceRestService` reads `/docs`, follows it to the OpenAPI description, looks up each operation there and builds the request body from its schema. It demonstrates capabilities, validation, reading, conversion and rendering.
  - `InvoiceMcpService` connects to `/mcp` over Streamable HTTP, lists the tools and builds each call's arguments from the tool's input schema.
  - Optional canonical invoice JSON demonstrates the `generate_invoice` tool.

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

### Run both walkthroughs

```bash
dotnet run --project invoice-api-calls/invoice-api-calls.csproj -- https://invoice-service-api.simon-ai-enterprise.de <YOUR_API_KEY>
```

Without `rest` or `mcp` as the first argument, the REST walkthrough runs first and the MCP walkthrough after it. Converted and rendered documents are written to `output/rest` and `output/mcp`.

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

