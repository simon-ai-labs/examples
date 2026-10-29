#!/usr/bin/env pwsh
# Regenerates the Kiota REST client from the OpenAPI description that /docs renders and refreshes the
# embedded snapshot the docs reader compares against. Run it when `invoice-client docs` reports drift.
param(
    [string] $BaseAddress = "https://invoice-service-api.simon-ai-enterprise.de"
)
$ErrorActionPreference = "Stop"
$root = Split-Path $PSScriptRoot -Parent
$client = Join-Path $root "src/InvoiceServices.Client/OpenApi"
$spec = "$($BaseAddress.TrimEnd('/'))/openapi/v1.json"

Push-Location $root
try {
    dotnet tool restore
    Invoke-WebRequest $spec -OutFile (Join-Path $client "v1.snapshot.json")
    dotnet kiota generate --language CSharp --openapi (Join-Path $client "v1.snapshot.json") `
        --class-name InvoiceApiClient --namespace-name InvoiceServices.Client.OpenApi.Generated `
        --output (Join-Path $client "Generated") --exclude-backward-compatible --clean-output
    Remove-Item (Join-Path $client "Generated/.kiota.log") -ErrorAction SilentlyContinue
    dotnet build --nologo
}
finally {
    Pop-Location
}
