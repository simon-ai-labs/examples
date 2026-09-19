# Filing Service testapps

This project contains one isolated C# client and one matching test service for every
recommended Filing Service OpenAPI slice.

The Filing Service currently publishes one OpenAPI document per literal
/api/{datenart}/{stand} route. The 101 clients in FilingDataTypeClients.cs
map one-to-one to the catalogue entries from GET /openapi/index.json.
Each matching class in FilingDataTypeServices.cs consumes only its own client.

The shared base classes are transport/orchestration helpers only. Every concrete
client declares its own request records and its own `GetOpenApiDocumentAsync`,
`ValidateAsync`, `PreviewAsync`, `SimulateAsync` and `SubmitAsync` calls. Every
concrete service exposes the same document-specific request types and delegates
only to its matching client. This keeps the OpenAPI document, client, service
and tax/data-type operations together in one explicit slice.

## Run

The default operation is a safe contract smoke test for ustva-2026:

```bash
dotnet run --project filing-service-testapps --   --base-url https://your-filing-service.example
```

List all generated testapps:

```bash
dotnet run --project filing-service-testapps -- --list
```

Verify that the runtime OpenAPI catalogue has a client for every document:

```bash
dotnet run --project filing-service-testapps --   --base-url https://your-filing-service.example --verify-catalog
```

Run contract smoke tests for all 101 slices:

```bash
dotnet run --project filing-service-testapps --   --base-url https://your-filing-service.example --all
```

Run a non-transmitting validation or preview with a JSON payload:

```bash
dotnet run --project filing-service-testapps --   ustva-2026 --operation validate --payload ./payload.json

dotnet run --project filing-service-testapps --   ustva-2026 --operation preview --payload ./payload.json
```

Simulation and production submission require a real test certificate and PIN.
The client sends simulation: true for simulation and simulation: false
for production submission; production submission must remain disabled on the
service unless an intentional real filing is being tested.

Configuration can also be supplied through FILING_SERVICE_BASE_URL,
FILING_SERVICE_API_KEY, FILING_SERVICE_API_KEY_HEADER and FILING_SERVICE_PIN.
