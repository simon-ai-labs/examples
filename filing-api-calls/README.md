# filing-api-calls

.NET console walkthrough for the Filing Service REST API, using the bundled
UStVA 2026 test filing:

1. `POST /api/ustva/2026/validate` — validates the test filing.
2. `POST /api/ustva/2026/validate` — validates an invalid variant and shows the findings.
3. `POST /api/ustva/2026/preview` — saves the PDF as `ustva-2026-preview.pdf`.
4. `POST /api/ustva/2026/filings` with `simulation: true` — optional test transmission,
   only when a certificate is configured.

Other Datenarten work the same way under `/api/{datenart}/{year}/...`; see `/docs`.

## Run

```bash
dotnet run --project filing-api-calls/filing-api-calls.csproj -- http://localhost:8080 [API_KEY]
```

Optional environment variables: `FILING_API_BASE_URL`, `FILING_API_KEY`,
`FILING_CERTIFICATE_PATH` and `FILING_CERTIFICATE_PIN`.
