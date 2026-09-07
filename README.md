# RapidAPI E-Invoicing Sample Walkthrough (.NET)

This sample console application demonstrates step-by-step how to interact with the **E-Invoicing Service** on RapidAPI:
[E-Invoicing Service on RapidAPI](https://rapidapi.com/alexandersimon90/api/e-invoicing-service)

It covers all five core functions documented in `RAPIDAPI.md`:
1. **Capabilities**: Querying supported formats, profiles, and operations (`GET /v1/capabilities`).
2. **Generation**: Generating a compliant electronic invoice (XRechnung, Peppol BIS 3.0, ZUGFeRD) from pure JSON (`POST /v1/invoices/generate`).
3. **Validation**: Validating an electronic invoice against official Schematron, XSD, and business rules (`POST /v1/invoices/validate`).
4. **Reading**: Parsing raw XML or hybrid PDF into a canonical structured invoice object (`POST /v1/invoices/read`).
5. **Conversion**: Converting between invoice syntaxes and formats (e.g., UBL to CII, XRechnung to Factur-X) (`POST /v1/invoices/convert`).
6. **Rendering**: Generating human-readable HTML and print-ready PDF visualizations (`POST /v1/invoices/render`).

---

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- An active RapidAPI Account and API key from [RapidAPI E-Invoicing Service](https://rapidapi.com/alexandersimon90/api/e-invoicing-service)

---

## Configuration

Set your RapidAPI key as an environment variable, or pass it as an argument:

```bash
# Set environment variable (recommended)
export RAPIDAPI_KEY="your-rapidapi-key-here"

# On Windows PowerShell:
$env:RAPIDAPI_KEY="your-rapidapi-key-here"
```

---

## Running the Sample

```bash
# Run using environment variable
dotnet run --project invoice-api-calls/invoice-api-calls.csproj

# Or run passing your RapidAPI key directly
dotnet run --project invoice-api-calls/invoice-api-calls.csproj -- <YOUR_RAPIDAPI_KEY>
```

You can also test against a local instance of the API:
```bash
dotnet run --project invoice-api-calls/invoice-api-calls.csproj -- http://localhost:5000 local-mode
```

---

## What the Sample Does

1. **Step 1: Check Capabilities**
   Calls `GET /v1/capabilities` and displays supported standards (XRechnung, Peppol, ZUGFeRD) and their available operations.

2. **Step 2: Generate an Invoice**
   Creates a canonical invoice model in memory and requests `POST /v1/invoices/generate` for:
   - Target: `XRechnung 3.0.2` (UBL syntax).
   XRechnung is the format whose capabilities cover validate, read, convert and render, so it is used
   as the primary document for the end-to-end steps below. Saves the resulting XML locally.

3. **Step 3: Validate the Invoice**
   Base64-encodes the generated XML and calls `POST /v1/invoices/validate`.
   Prints the validation status (`valid: true/false`) and any diagnostic issues/rules.

4. **Step 4: Read the Invoice**
   Calls `POST /v1/invoices/read` to extract the invoice ID, seller, buyer, line items, and totals into structured JSON.

5. **Step 5: Convert the Invoice**
   Calls `POST /v1/invoices/convert` to transform the XRechnung UBL invoice into `XRechnung 3.0.2` in `CII` syntax.
   Saves the converted document to `converted_xrechnung_cii.xml`.

6. **Step 6: Render Human-Readable Visualizations**
   Calls `POST /v1/invoices/render`:
   - `outputFormat: "html"` -> saved as `rendered_invoice.html`
   - `outputFormat: "pdf"` -> saved as `rendered_invoice.pdf`
