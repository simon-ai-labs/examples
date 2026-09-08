# E-Invoicing Service Examples

Sample code demonstrating how to interact with the [E-Invoicing Service on RapidAPI](https://rapidapi.com/schickmaster/api/european-german-e-invoicing-service).

> ⚠️ **Disclaimer: Demonstration Purposes Only**  
> This code is provided **for demo and educational purposes only**. It is **not intended for production use**. Before using any code in production environments, ensure proper error handling, secret management, logging, resiliency, and compliance with your organization's security standards.

---

## Projects

- **[invoice-api-calls](invoice-api-calls/)**: Console walkthrough covering all core operations of the E-Invoicing API:
  - Checking capabilities (`GET /v1/capabilities`)
  - Loading an official KoSIT/XRechnung test invoice (no invoice is generated)
  - Invoice validation (`POST /v1/invoices/validate`)
  - Reading structured data (`POST /v1/invoices/read`)
  - Syntax conversion (`POST /v1/invoices/convert`)
  - Rendering HTML / PDF (`POST /v1/invoices/render`)

The bundled `01.06_minimal_test_ubl.xml` is an unmodified positive reference
invoice from the official [KoSIT XRechnung Test Suite](https://github.com/itplr-kosit/xrechnung-testsuite).
Its pinned upstream source and license are recorded in
[invoice-api-calls/TestInvoices/NOTICE.md](invoice-api-calls/TestInvoices/NOTICE.md).

---

## Quick Start

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- RapidAPI Key for [E-Invoicing Service](https://rapidapi.com/schickmaster/api/european-german-e-invoicing-service)

### Build Solution
```bash
dotnet build examples.sln
```

### Run Walkthrough
```bash
# Pass key as argument:
dotnet run --project invoice-api-calls/invoice-api-calls.csproj -- <YOUR_RAPIDAPI_KEY>

# Or set environment variable:
export RAPIDAPI_KEY="your-rapidapi-key"   # Linux / macOS
$env:RAPIDAPI_KEY="your-rapidapi-key"     # Windows PowerShell
dotnet run --project invoice-api-calls/invoice-api-calls.csproj
```
