# simon-ai-labs Service Examples

Sample code for the simon-ai-labs APIs. Each folder is a self-contained .NET
console project with its own README.

| Project | Service | What it shows |
|---|---|---|
| [invoice-api-calls](invoice-api-calls/) | E-Invoicing Service | Validating, reading, generating, converting and rendering XRechnung, ZUGFeRD, Peppol and EN 16931 invoices via REST and MCP |
| [filing-api-calls](filing-api-calls/) | Filing Service | Validating, previewing and test-transmitting ELSTER tax filings (UStVA 2026) via REST |

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- Access to a deployment of the service the sample talks to

## Build

```bash
dotnet build examples.sln
```

Copy `.env.example` to `.env` to configure both samples locally. `.env` is
ignored by git; never commit real API keys, certificates or PINs.

Sample code notice: These examples are intentionally minimal and intended to demonstrate API integration. Production applications should add appropriate error handling, secret management, retries, logging and monitoring.
