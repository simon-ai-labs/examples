# filing-api-calls

.NET console walkthrough for the Filing Service REST API. It uses the bundled
UStVA 2026 test filing and calls a running Filing Service container:

1. `GET /health/ready` — checks that the service is ready.
2. `GET /openapi/index.json` — lists the published contracts (one per Datenart and year).
3. `POST /api/ustva/2026/validate` — validates the test filing.
4. `POST /api/ustva/2026/validate` — validates an invalid variant and expects findings.
5. `POST /api/ustva/2026/preview` — renders the PDF and saves `ustva-2026-preview.pdf`.
6. `POST /api/ustva/2026/filings` with `simulation: true` — optional test transmission to ELSTER,
   only when a certificate is configured.

Steps 1–5 transmit nothing. Every other Datenart works the same way under
`/api/{datenart}/{year}/validate|preview|filings`; the request models are in
`/openapi/{datenart}-{year}.json` and on `/docs`.

## Run

All commands are run from the repository root.

```bash
dotnet run --project filing-api-calls/filing-api-calls.csproj -- http://localhost:8080
```

With an API key for a gateway in front of the service:

```bash
dotnet run --project filing-api-calls/filing-api-calls.csproj -- https://your-filing-service.example.com <YOUR_API_KEY>
```

Settings can also be supplied through environment variables, .NET User Secrets or `.env`:

```bash
export FILING_API_BASE_URL="http://localhost:8080"
export FILING_API_KEY="your-api-key"                      # optional
export FILING_CERTIFICATE_PATH="./elster-test-cert.pfx"   # optional, enables step 6
export FILING_CERTIFICATE_PIN="123456"
dotnet run --project filing-api-calls/filing-api-calls.csproj
```

A filing with errors is not an HTTP error: the service answers `200` with
`"error": true` and the findings in `feedback`. Do not retry a failed
`filings` call automatically — the delivery may already have reached ELSTER.

Sample code notice: These examples are intentionally minimal and intended to demonstrate API integration. Production applications should add appropriate error handling, secret management, retries, logging and monitoring.
