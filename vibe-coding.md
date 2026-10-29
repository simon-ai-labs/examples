# Vibe coding a tool on the E-Invoicing Service

This guide is for people who build software together with an AI coding assistant (Claude Code, Cursor,
Copilot, …) and want to end up with a tool that uses the
[E-Invoicing Service](https://invoice-service-api.simon-ai-enterprise.de): checking incoming
XRechnung/ZUGFeRD invoices, reading them into an ERP, generating outgoing invoices, converting or
rendering them.

**How to use it:** put this file into your project and tell your assistant:

> Read `vibe-coding.md`. Act as the guide described there and take me through the stages, one at a time.

The assistant then asks you questions, suggests decisions, writes the code with you and checks each
stage before moving on. You can also read it yourself as a checklist.

---

## Part 1 — Instructions for the AI assistant

You are the guide for someone who may not be a professional developer. Your job is to get them to
working, maintainable software, not just to code that runs once.

1. **One stage at a time.** Follow the stages in Part 3 in order. Start each stage by saying in two or
   three sentences what it is for. Do not start the next stage until its "Done when" list is met, and
   show the user the evidence (command output, a saved file, a passing test).
2. **Ask, then recommend.** When a decision is needed, ask one question at a time, offer two or three
   options in plain language and say which one you recommend and why. Choose sensible defaults
   yourself for things the user does not care about, and tell them what you chose.
3. **Prove it against the real service.** Every feature is finished only after a real call to the API
   worked. Never claim something works because it compiles. If there is no API key yet, say clearly
   what is untested.
4. **Small steps.** Change little at a time, run it, then continue. Commit after each working step if
   the project uses git, with a message that says what now works.
5. **Protect the API key.** Never write the key into source code, a committed file, a URL, a log line
   or a command-line argument. Use environment variables or the platform's secret store, and add the
   file that holds it (for example `.env`) to `.gitignore` before the key is first entered.
6. **Treat invoices as sensitive data.** They contain names, addresses, bank details and amounts. Do
   not log invoice content, do not send it anywhere except the service, and keep sample files
   anonymous.
7. **Use the service's own description.** The source of truth is the OpenAPI description
   (`/openapi/v1.json`, linked from `/docs`) and `GET /v1/capabilities`. Look things up there instead
   of guessing field names, enum spellings or versions.
8. **Explain errors in plain words.** When something fails, show what the service answered, explain
   what it means and propose the fix.
9. **Keep it simple.** No extra frameworks, layers or abstractions unless the user's goal needs them.
   A small tool that works and is tested beats a big one that does not.

---

## Part 2 — What you need to know about the API

| Item | Value |
|---|---|
| Base URL | `https://invoice-service-api.simon-ai-enterprise.de` |
| Interactive docs | [`/docs/`](https://invoice-service-api.simon-ai-enterprise.de/docs/) (public) |
| Machine-readable description | `/openapi/v1.json` (OpenAPI 3.1, public) |
| MCP endpoint | `/mcp` (Streamable HTTP) — the same operations as tools for AI agents |
| API key | `imp_` + 64 hex characters, created in the [customer portal](https://invoice-service-api.simon-ai-enterprise.de/portal) after subscribing through Google Cloud Marketplace |
| Auth header | `X-API-Key: <key>` on every API call (not needed for `/docs` and `/openapi/v1.json`) |

### Business cases

| Business case | Endpoint | MCP tool | Typical user |
|---|---|---|---|
| Check an incoming invoice | `POST /v1/invoices/validate` | `validate_invoice` | Accounts payable: is the XRechnung/ZUGFeRD file formally and semantically correct? XSD, EN 16931 and XRechnung Schematron, code lists, calculation rules, PDF/A-3b for PDFs. Issues carry rule ID and path, so an invoice can be rejected with a reason |
| Read an incoming invoice | `POST /v1/invoices/read` | `read_invoice` | ERP import: XML or PDF in any supported format becomes one JSON model (`InvoiceDto`), validation result included |
| Generate an outgoing invoice | `POST /v1/invoices/generate` | `generate_invoice` | Billing from ERP or shop: JSON invoice data becomes an XRechnung (B2G) or ZUGFeRD/Factur-X (B2B, as hybrid PDF) |
| Convert the format | `POST /v1/invoices/convert` | `convert_invoice` | A recipient needs another format, e.g. ZUGFeRD → XRechnung or UBL ↔ CII. Lost information is reported as a warning |
| Render for people | `POST /v1/invoices/render` | `render_invoice` | Visual check, approval workflow, archive view: XML as HTML or PDF |
| Query capabilities | `GET /v1/capabilities` | `invoice_capabilities` | Which formats, versions, profiles and combinations each operation supports |

### Request building blocks

A **document** you send (validate, read, convert, render):

```json
{
  "document": {
    "content": "<the file as Base64>",
    "fileName": "invoice.xml",
    "contentType": "application/xml",
    "expectedRepresentation": null
  }
}
```

Use `application/pdf` for PDF files. `expectedRepresentation` is optional; set it when you know what the
file must be, for example `{ "format": "xRechnung", "version": "3.0.2", "profile": "xRechnung", "syntax": "ubl" }`.

A **target** (generate, convert) has the same four fields: `format` (`xRechnung`, `zugferd`, `peppol`),
`version`, `profile` (e.g. `xRechnung`, `en16931`, `extended`) and `syntax` (`ubl`, `cii`). Enum values
are case-sensitive. **Take the versions from `GET /v1/capabilities`** instead of hard-coding them; the
service pins them and may update them.

Render additionally takes `"outputFormat": "html"` or `"pdf"`. Generate takes `invoice` (an
`InvoiceDto`) and `target`.

### Responses

- Documents come back as Base64 in `content`, together with `fileName` and `contentType`.
- **validate** answers `200` also for invalid invoices: check `valid`, then `issues[]` with `severity`,
  `ruleId`, `businessPath` and `message`. Severity `information` does not make an invoice invalid.
- **generate** and **convert** return `warnings[]` (`code`, `businessPath`, `message`); show them to the
  user, they mean information was lost or is missing.
- **read** returns `invoice`, `detectedRepresentation` and `validation`.

### Errors

Errors are JSON problem details with `code`, `title`, `detail` and `traceId`.

| Status | Meaning | What the tool should do |
|---|---|---|
| 400 | Request malformed (missing field, wrong enum) | Bug in your code: fix the request |
| 401 | Key missing or invalid | Tell the user to check the configured key |
| 403 | Subscription does not allow this | Show the message, do not retry |
| 422 | The document could not be processed | Show `detail` to the user, the file is the problem |
| 429 | Rate limit | Wait (respect `Retry-After`) and retry |
| 503 | Engine temporarily unavailable | Retry with backoff |

### Good to know

- The service stores no documents and every operation depends only on its input, so retrying after a
  timeout, 429 or 5xx is safe.
- Validation takes well under a second; rendering a PDF can take several seconds, longer under load.
  Set the HTTP timeout generously (90 seconds is a safe start).
- Known quirk: decimal amounts such as `payableAmount` are described as `type: string` in the OpenAPI
  description, but the service sends JSON numbers. Accept both when reading. Code generators that
  trust the description may read these fields as `null`.
- A free, official test invoice is in this repository:
  [`invoice-api-calls/TestInvoices/01.06_minimal_test_ubl.xml`](invoice-api-calls/TestInvoices/01.06_minimal_test_ubl.xml)
  (from the KoSIT XRechnung test suite). Removing its `cbc:BuyerReference` element produces a known
  validation error, `BR-DE-15`.

### Reference implementation

[`invoice-api-calls`](invoice-api-calls/) in this repository runs every business case against the live
service, over REST and MCP. When in doubt about the exact shape of a call, look at
[`InvoiceBusinessCases.cs`](invoice-api-calls/Services/InvoiceBusinessCases.cs).

---

## Part 3 — The route

### Stage 0 — Decide what you are building

**Goal:** one sentence that says who uses the tool, for what, and what "done" looks like.

The guide asks, one at a time:

1. Who uses it, and in which situation? (e.g. "our bookkeeper checks supplier invoices before paying")
2. Which business cases from Part 2 does the first version need? Recommend starting with **one**.
3. What kind of tool? Command line, small web app, desktop app, a plug-in for an existing system, or an
   AI agent using the MCP endpoint. Recommend the simplest one that fits the user.
4. Which language or platform? If the user has no preference, recommend what they already know or what
   their existing system uses.
5. Where do invoices come from and where do results go? (folder, upload, email, ERP)

**Done when:** the user agrees with a short written goal, a list of features for version 1, and a list
of things explicitly left out for later. Save it as `GOAL.md` in the project.

**Watch out:** building everything at once. Version 1 should do one job well.

---

### Stage 1 — Project setup and API key

**Goal:** an empty project that runs, with the key stored safely.

1. Create the project with the language's standard tooling.
2. Add `.gitignore` (including `.env` or the secret file) **before** the key is entered.
3. Read the key from an environment variable, e.g. `INVOICE_API_KEY`, and the base URL from
   `INVOICE_API_BASE_URL` with the production URL as default.
4. Provide a `.env.example` with the variable names but no values.

**Prompt to paste:**

> Set up a minimal <language> project for the tool in GOAL.md. Read INVOICE_API_KEY and
> INVOICE_API_BASE_URL from the environment, never from code. Add .gitignore and .env.example.
> Make it print "configuration OK" when the key is set and a clear message when it is not.

**Done when:** running the project prints "configuration OK" with the key set and a helpful message
without it; `git status` does not show the secret file.

---

### Stage 2 — First contact

**Goal:** prove that the tool can talk to the service.

1. Fetch `/openapi/v1.json` (no key needed) and keep it next to the code for reference.
2. Call `GET /v1/capabilities` with the key and print the formats and versions.

**Prompt to paste:**

> Call GET /v1/capabilities on the E-Invoicing Service with the X-API-Key header and print each
> standard with its version and the profiles per operation. Show me the raw answer once.

**Done when:** the output lists XRechnung and ZUGFeRD with versions. If you get 401, fix the key
before going on.

---

### Stage 3 — The first business case, end to end

**Goal:** the most important feature from Stage 0 works with the official test invoice.

1. Load a file, Base64-encode it, build the request from Part 2 and send it.
2. Turn the answer into something the user understands: "valid" or a list of problems with rule and
   path, the saved converted file, the extracted invoice data.
3. Test it with the good test invoice **and** with a broken one (test invoice without
   `BuyerReference` → `BR-DE-15`).

**Prompt to paste:**

> Implement <business case> for the tool in GOAL.md. Use the request format from vibe-coding.md
> Part 2 and take versions from /v1/capabilities. Run it with
> invoice-api-calls/TestInvoices/01.06_minimal_test_ubl.xml and with a copy that has
> cbc:BuyerReference removed, and show me both results.

**Done when:** both runs give the expected result against the live service, and the user has seen and
understood the output.

**Watch out:** hard-coded versions, enum spelling (`xRechnung`, not `XRechnung`), forgetting
`contentType`, sending the raw file instead of Base64.

---

### Stage 4 — Errors and resilience

**Goal:** the tool behaves well when things go wrong.

Walk through the error table in Part 2 and make the tool handle each case:

- 401/403: clear message, no retry.
- 400/422: show the service's `detail`; 400 means your request is wrong.
- 429/503/timeouts: retry a few times with increasing waits, respecting `Retry-After`.
- Network down, file missing, file not an invoice: a short message, no crash dump.
- Handle errors in **one** place instead of wrapping every call.

**Prompt to paste:**

> Go through the error table in vibe-coding.md Part 2. Make the tool handle each case as described,
> in one central place. Then prove it: wrong key, a non-invoice file, a missing file. Show me what
> the user sees each time.

**Done when:** each of those situations produces a short, understandable message and the tool does
not crash or hang. Secrets and invoice content never appear in the output or logs.

---

### Stage 5 — The remaining features

**Goal:** everything planned for version 1 works.

Repeat Stage 3 for each further business case from `GOAL.md`. Typical additions:

- after **read**, map the `InvoiceDto` fields the user needs into their system;
- **generate** from the user's own data: start from the `invoice` returned by read for the test
  invoice and change it, then validate the generated document;
- **convert**: show the warnings, they tell the user what was lost;
- **render**: save HTML or PDF next to the source file.

**Done when:** every feature in `GOAL.md` has been run against the live service with a real or test
invoice, and anything not done is moved to the "later" list.

---

### Stage 6 — Tests

**Goal:** changes later do not silently break what works now.

1. **Unit tests** for your own logic (mapping, file handling, error messages) with the HTTP calls
   replaced by fakes that return answers recorded from the real service.
2. **A few live tests** that call the real service with the test invoice, run only when
   `INVOICE_API_KEY` is set and clearly reported as skipped otherwise — never silently passed.
3. Run all tests with one command.

**Prompt to paste:**

> Add tests: unit tests for our own logic with faked HTTP answers recorded from the real service,
> and live tests against the real service that run only when INVOICE_API_KEY is set and report
> "skipped" otherwise. Give me one command to run everything.

**Done when:** the test command passes, and breaking something on purpose (e.g. a wrong field name)
makes a test fail.

---

### Stage 7 — Security and data protection review

**Goal:** the tool is safe to give to others.

The guide checks with the user:

- [ ] The key is not in the code, the git history, logs or error messages.
- [ ] Invoice content is not logged; output files go where the user expects and nowhere else.
- [ ] Only HTTPS is used for the service.
- [ ] Uploaded or processed files are cleaned up if the tool is a web app.
- [ ] Dependencies are current and from official sources.
- [ ] If others use the tool, it is clear who provides the API key and who pays for usage.

**Done when:** every box is ticked or consciously accepted with a note in `GOAL.md`.

---

### Stage 8 — Ship it

**Goal:** someone else can install and use the tool without you.

1. A `README.md` with: what it does, how to install, how to set the key, one example run, known
   limitations.
2. One command to build or start it.
3. If it uses git and a hosting service, a CI build that runs the unit tests.
4. A fresh-machine test: follow the README from scratch, ideally on another computer or with another
   person.

**Done when:** a second person got it running from the README alone.

---

## Part 4 — What "good software" means here

Before calling version 1 finished, all of these should be true:

- [ ] It solves the one problem written in `GOAL.md`, for the user named there.
- [ ] Every feature was run against the live service.
- [ ] Errors produce short, understandable messages; retries handle 429, 503 and timeouts.
- [ ] Versions come from `/v1/capabilities`, field names from the OpenAPI description.
- [ ] No secrets or invoice content in code, history or logs.
- [ ] Tests run with one command and fail when something breaks.
- [ ] The README lets someone else install and run it.
- [ ] The code is small enough that the user can explain what each part does.

---

## Part 5 — When you are stuck

| Symptom | Likely cause | Fix |
|---|---|---|
| `401 marketplace_authentication_required` | Key missing, mistyped or not sent | Check `INVOICE_API_KEY` and the `X-API-Key` header |
| `400` with a field name in `detail` | Wrong field name or enum spelling | Compare with `/openapi/v1.json`; enums are case-sensitive |
| `422` | The file is not a supported invoice or is damaged | Try the official test invoice; check `contentType` |
| `valid: false` with `BR-DE-…` rules | The invoice breaks XRechnung rules | Show the rule and path to the user; that is the expected result for bad invoices |
| Amounts are `null` after reading | Generated client trusts `type: string` for decimals | Read amounts as numbers or strings |
| Timeout when rendering PDF | Default HTTP timeout too short | Raise it to 90 seconds or more |
| Generated and converted files overwrite each other | The service returns the same file name | Prefix or separate output folders |
| The assistant keeps guessing field names | It has not read the description | Tell it: "look it up in /openapi/v1.json first" |
