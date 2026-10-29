# E-Invoicing Service Examples

Sample code for the E-Invoicing Service, which validates, reads, generates, converts and renders XRechnung, ZUGFeRD, Peppol and EN 16931 electronic invoices.

Service URL: [https://invoice-service-api.simon-ai-enterprise.de](https://invoice-service-api.simon-ai-enterprise.de) · [API documentation](https://invoice-service-api.simon-ai-enterprise.de/docs/)

## Projects

- **[invoice-api-calls](invoice-api-calls/)**: .NET console walkthrough for the REST API and MCP interface. It has no request or response classes: requests are built at runtime from the service's own JSON definitions.
  - `InvoiceBusinessCases` walks through every business case of the service (see below), once over each transport.
  - `InvoiceRestService` reads `/docs`, follows it to the OpenAPI description, looks up each operation there and builds the request body from its schema.
  - `InvoiceMcpService` connects to `/mcp` over Streamable HTTP, lists the tools and builds each call's arguments from the tool's input schema.
- **[vibe-coding.md](vibe-coding.md)**: a complete prompt for building your own tool on this API with an AI assistant. Copy the whole file into the AI chat (ChatGPT, Claude, Copilot, Cursor, …) and send it. The AI then guides you stage by stage from the first idea through setup, the business cases, error handling, test automation with code coverage, SAST, SCA and DAST to a shippable tool with a CI pipeline.

### Business cases in the walkthrough

| Business case | Endpoint · MCP tool | What the walkthrough does |
|---|---|---|
| Query capabilities | `GET /v1/capabilities` · `invoice_capabilities` | Lists formats, versions, syntaxes, input/output types and the profiles per operation. The versions used in all following cases are taken from here. |
| Check an incoming invoice | `POST /v1/invoices/validate` · `validate_invoice` | Validates the KoSIT test invoice (XSD, EN 16931 and XRechnung Schematron, code lists, calculation rules), then a variant without BuyerReference (BT-10) and expects rule `BR-DE-15` with its BT path, the reason to reject the invoice. Generated invoices are validated too, which includes PDF/A-3b for hybrid PDFs. |
| Read an incoming invoice | `POST /v1/invoices/read` · `read_invoice` | Turns the XML into the canonical JSON model (`InvoiceDto`) with detected representation and validation result, as for an ERP import. Saved as `invoice.json`. |
| Generate an outgoing invoice | `POST /v1/invoices/generate` · `generate_invoice` | Generates an XRechnung UBL (B2G) and a ZUGFeRD EN 16931 invoice (B2B) from the JSON read before, or from `INVOICE_CANONICAL_JSON_PATH`. Prints warnings and validates the result. |
| Convert the format | `POST /v1/invoices/convert` · `convert_invoice` | Converts XRechnung UBL to XRechnung CII and to ZUGFeRD EN 16931 and prints the warnings for information lost in the conversion. |
| Render the invoice for people | `POST /v1/invoices/render` · `render_invoice` | Renders the converted XRechnung as HTML and PDF for a visual check, an approval workflow or an archive view. |

All results are written to `output/rest` and `output/mcp`.

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

The API key can also be provided with the `INVOICE_API_KEY` environment variable or .NET User Secrets. The base URL can be set with `INVOICE_API_BASE_URL`.

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

The MCP walkthrough uses one session for all tool calls. Both walkthroughs run all
business cases. `generate_invoice` uses the invoice read from the KoSIT sample
unless `INVOICE_CANONICAL_JSON_PATH` points to your own JSON file containing an `InvoiceDto`:

```bash
export INVOICE_CANONICAL_JSON_PATH="./invoice.json"
dotnet run --project invoice-api-calls/invoice-api-calls.csproj -- mcp
```

The canonical invoice schema and supported target combinations are defined by
the service's `/openapi/v1.json` and `/v1/capabilities` endpoints.

Sample code notice: These examples are intentionally minimal and intended to demonstrate API integration. Production applications should add appropriate error handling, secret management, retries, logging and monitoring.

