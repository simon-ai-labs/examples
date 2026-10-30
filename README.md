# Simon AI Service Examples

Sample code for the E-Invoicing Service, which validates, reads, generates, converts and renders XRechnung, ZUGFeRD, Peppol and EN 16931 electronic invoices, and for the Minimum Tax API, which validates, reads and generates OECD Pillar Two GloBE Information Returns (GIR) and validates and reads Country-by-Country Reports (CbCR).

E-Invoicing Service: [https://invoice-service-api.simon-ai-enterprise.de](https://invoice-service-api.simon-ai-enterprise.de) · [API documentation](https://invoice-service-api.simon-ai-enterprise.de/docs/)

Minimum Tax API (development): [https://minimum-tax-api-dev.simon-ai-enterprise.de](https://minimum-tax-api-dev.simon-ai-enterprise.de) · [API documentation](https://minimum-tax-api-dev.simon-ai-enterprise.de/docs/)

## Projects

- **[invoice-api-calls](invoice-api-calls/)**: .NET console walkthrough for the REST API and MCP interface. It has no request or response classes: requests are built at runtime from the service's own JSON definitions.
  - `InvoiceBusinessCases` walks through every business case of the service (see below), once over each transport.
  - `InvoiceRestService` reads `/docs`, follows it to the OpenAPI description, looks up each operation there and builds the request body from its schema.
  - `InvoiceMcpService` connects to `/mcp` over Streamable HTTP, lists the tools and builds each call's arguments from the tool's input schema.
- **[minimum-api-calls](minimum-api-calls/)**: .NET console walkthrough for the Minimum Tax API over REST and MCP, built the same way as `invoice-api-calls`: requests come from the OpenAPI description behind `/docs` and from the MCP tools' input schemas, without request or response classes.
  - `MinimumTaxBusinessCases` runs every business endpoint for every country: it validates a positive, a technical (XSD) negative and a business-rule negative document for both GIR and CbCR, reads each GIR into the typed model and generates it again, answers each GIR with an accepting and a rejecting status message and reads them back, and reads each CbCR.
  - `MinimumTaxRestService` and `MinimumTaxMcpService` are the two transports; the test documents and their expected results are described in [minimum-api-calls/TestDocuments/README.md](minimum-api-calls/TestDocuments/README.md).
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

### Minimum Tax API test matrix

For each of the 27 EU jurisdictions the service lists in `/v1/capabilities`, over REST and over MCP:

| Business case | Endpoint · MCP tool | Expected result |
|---|---|---|
| Check a GIR | `POST /v1/gir/validate` · `validate_gir` | positive: valid and complete under `<CC>-GIR-2025-08`; XSD negative: `SCHEMA-ERR` (`xsd` layer); business negative: OECD rule `60020` |
| Read and generate a GIR | `gir/read` · `read_gir` → `gir/generate` · `generate_gir` → `validate_gir` | the model names the UPE with its TIN; the generated GIR is valid and complete under the same national pack |
| Report the processing status | `gir/status-message/generate` · `generate_gir_status_message` → `gir/status-message/read` · `read_gir_status_message` | the positive is answered `Accepted`, the business negative `Rejected` with exactly the rule codes its validation found, and both read back unchanged |
| Check a CbCR | `POST /v1/cbcr/validate` · `validate_cbcr` | positive: valid; XSD negative: `CBCR-SCHEMA-ERR`; business negative: `CBCR-SEM-002` |
| Read a CbCR | `POST /v1/cbcr/read` · `read_cbcr` | valid, with the country as transmitting country |

Before the business cases, the REST walkthrough checks the operational endpoints `GET /healthz` and `GET /api/status`, which have no MCP tool. That makes 297 business steps per transport (81 GIR and 81 CbCR checks, 27 GIR reads, 27 regenerations, 54 status messages, 27 CbCR reads), plus the two operational endpoints over REST. A step that does not get its expected result is printed with its issues or the service's error, the run continues with the next country and exits with 1 at the end. Results go to `output/rest` and `output/mcp`: per country the read GIR model, the generated GIR and the read CbCR model, and `summary.json` with every step.

**Known service defects (dev, 2026-10-30).** 34 steps per transport fail because of the service, not the samples, and stay red on purpose until it is fixed:

- `gir/read` answers the Austrian `GLOBE_NATIONAL` document with HTTP 500.
- `gir/generate` loses national formats the read document met: BG (`BG-GIR-DOCREF`), ES (`ES-GIR-5.3a`), IT (`IT-GIR-E6011`), NL (`NL-GIR-5.0`, `version` 1.1), PL (`PL-GIR-XSD.1`, numeric DocRefIds) and SI (`SI-GIR-60011`).
- `gir/status-message/read` returns no issues for a rejecting status message whose XML carries the record errors, so all 27 rejections fail. `gir/status-message/generate` also cuts national rule codes to ten characters (`DE-GIR-99011` becomes `DE-GIR-990`).
- `GET /healthz` is answered by Cloud Run's front end with 404, because Cloud Run reserves paths ending in `z` (REST only).

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

### Run the Minimum Tax walkthroughs

```bash
dotnet run --project minimum-api-calls/minimum-api-calls.csproj -- https://minimum-tax-api-dev.simon-ai-enterprise.de
```

As for the invoices, `rest` or `mcp` as the first argument runs one transport only, an API key can follow the URL, and the settings can come from `MINIMUM_TAX_API_BASE_URL` and `MINIMUM_TAX_API_KEY`:

```bash
dotnet run --project minimum-api-calls/minimum-api-calls.csproj -- mcp https://minimum-tax-api-dev.simon-ai-enterprise.de
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

