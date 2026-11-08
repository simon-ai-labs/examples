# filing-api-calls

.NET console walkthrough for the German Tax Filing Service over REST and MCP, built the same way as
`invoice-api-calls`: there are no request or response classes. The routes come from the service's own
catalog, and every request body is an example from the published contract of its Stand.

- `FilingCatalog` picks the **latest Stand of every Datenart**: a Datenart is a route without its version
  segment (`/api/ustva/2026` → `/api/ustva`, `/api/kmv/bsb/3` → `/api/kmv/bsb`), and only its highest
  version is called. Routes without a version (Bereitstellung, Protokolle, SpezRecht) count on their own.
- `FilingRestService` reads `/openapi/index.json`, downloads the contract of each latest Stand
  (`/openapi/{datenart}-{stand}.json`) and posts to its routes.
- `FilingMcpService` connects to `/mcp` over Streamable HTTP, lists the routes with `list_filing_routes`,
  reads each contract with `get_filing_contract` and calls `validate_filing`, `preview_filing`,
  `decode_filing_document` and `simulate_filing`. It never calls `submit_filing`.
- `FilingBusinessCases` runs the same cases over both transports (below).
- `FilingPerformanceService` is a small load test over REST.

## Business cases per Datenart

| Case | Operation | Expected result |
|---|---|---|
| Reference filing | `POST /api/ustva/2026/validate` | the bundled `TestFilings/ustva-2026.json` is valid; with a four-digit PLZ it is rejected (`error: true`) |
| Check every contract example | `validate` · `validate_filing` | `200` with `error: false` for every example of the Stand |
| Refuse a request without data | `validate` with `{}` | `400` (`filing-request-rejected`) |
| Render the PDF | `preview` · `preview_filing` | `200` with `error: false` and a PDF, saved under `output/<transport>/pdf/` |
| Nothing is sent without credentials | `filings`, `decode`, the Abfrage and SpezRecht operations, without `certificate` | `400` before anything reaches ELSTER |
| Test transmission (opt-in) | `filings` · `simulate_filing` with `simulation: true` | only with `FILING_SIMULATE=true`: every Stand once as a test case (Testmerker), signed with the ERiC test certificate the service brings along (`FILING_TEST_CERTIFICATE`: `organisation`, `idNr` or `steuernummer`; default `organisation`) |

A step that does not get its expected result is printed in red with ERiC's error codes or the service's
problem, the run continues and exits with 1 at the end. Every step, with its status and duration, goes to
`output/rest/summary.json` and `output/mcp/summary.json`.

### Result against production, 2026-11-08

| Transport | Datenarten | Steps | Passed | Failed |
|---|---|---|---|---|
| REST | 147 | 543 | 543 | 0 |
| MCP | 136 | 529 | 529 | 0 |

Deployment of simon-ai-labs/filing-service-api#265 (commit 16de955), whose contract examples are checked
against the real ERiC.

## Performance test

`perf` sends the reference filing to `validate` and `preview` at concurrency 1, 5, 10, 25 and 50,
ten requests per worker, after one warm-up call. Every answer must be `200` with `error: false` and the 95th
percentile must stay below 2 s. It prints requests per second and p50/p95/p99/max per level and writes
`output/perf/summary.json`. Nothing is transmitted.

Against production on 2026-11-08 every level passed: `validate` 15 → 109 requests/s from concurrency 1 to
50 (p95 116 → 637 ms), `preview` 13 → 58 requests/s (p95 106 ms → 1.6 s). Cloud Run scales out above about
25 concurrent requests; the first call of a large form on a fresh instance takes several seconds (EStE
2025 up to 13 s) while ERiC loads its plugin.

| Setting | Default |
|---|---|
| `FILING_PERF_CONCURRENCY` | `1,5,10,25,50` |
| `FILING_PERF_REQUESTS_PER_WORKER` | `10` |
| `FILING_PERF_P95_MS` | `2000` |

## Run

```bash
dotnet run --project filing-api-calls/filing-api-calls.csproj -- https://filing-service-api.simon-ai-enterprise.de [API_KEY]
```

```bash
dotnet run --project filing-api-calls/filing-api-calls.csproj -- perf https://filing-service-api.simon-ai-enterprise.de
```

The first argument may be `rest`, `mcp` or `perf`; without it the REST and MCP walkthroughs run. Settings
come from the environment, User Secrets or a `.env` file: `FILING_API_BASE_URL`, `FILING_API_KEY`,
`FILING_SIMULATE`, `FILING_TEST_CERTIFICATE` and the `FILING_PERF_*` values above.

## Build your own tool

[vibe-coding.md](vibe-coding.md) is a complete prompt for building your own tool on the Filing
Service with an AI assistant. Copy the whole file into the AI chat and send it.
