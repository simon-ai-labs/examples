# Test documents per country

Every EU jurisdiction the Minimum Tax API offers has three GIR and three CbCR documents:

| File | GIR (`gir/<CC>/`) | CbCR (`cbcr/<CC>/`) |
|---|---|---|
| `positive.xml` | Valid against the OECD GIR XML Schema, the OECD rules and the national pack `<CC>-GIR-2025-08`. Expected: `isValid` and `isComplete` both `true`, no issues. | Valid against the OECD CbC XML Schema v2.0 and the consistency checks. Expected: `isValid = true`, no issues. |
| `negative-xsd.xml` | **Technical (XSD)**: `positive.xml` with an element the schema does not define (`globe:Comment` after `NameMNE`). Expected: `SCHEMA-ERR` from the `xsd` layer. | **Technical (XSD)**: `positive.xml` with `cbc:Comment` after `NameMNEGroup`. Expected: `CBCR-SCHEMA-ERR`. |
| `negative-business.xml` | **Business rules**: `FilingInfo/Period/End` is 2023-12-31 while `MessageSpec/ReportingPeriod` stays 2024-12-31. The document is still schema-valid. Expected: OECD rule `60020`. DE, FI and SI also report a national rule (`DE-GIR-99011`, `FI-GIR-6.1`, `SI-GIR-98014`). | **Business rules**: the Reporting Entity's name is three spaces, which the schema accepts. Expected: `CBCR-SEM-002`. |

Each file except the Italian ones states its expectation in a leading XML comment. The Agenzia delle
Entrate rejects any comment in a GIR file (rule `IT-GIR-C0002`), so the IT documents carry none.

## How the positives satisfy the national packs

The GIR documents follow the national formats the packs check, for example:

- **AT**: the `GLOBE_NATIONAL` wrapper with `InfoDaten`, nine-digit Steuernummern and `SendingEntityIN` equal to `FastnrOrg`.
- **DE**: `MessageRefId` and `DocRefId` as `DE2024DE-<UUID v4>` and `DE2024-<UUID v4>`, a 13-digit Steuernummer and a `Contact`.
- **IT**: a formally valid codice fiscale, `IT2024IT<UUID v4>`, a `Contact` and no comments.
- **NL**: `version="1.1"` and no `SendingEntityIN`.
- **PL**: `DocRefId` of digits only, as in the GIR-1 schema.

The walkthrough sends a validation context with every GIR: a complete (empty) filing history, the TINs of
the document marked as checked against the national registers, and the original file bytes. Without it
the stateful OECD rules and the encoding checks withhold their verdict, and even a correct document
comes back with `isComplete = false` and therefore `isValid = false`.

## Origin

All documents were written for this repository. The groups, names, TINs, addresses and amounts are
invented; country, currency and language codes are real because the OECD schemas enumerate them. No
document is copied from a tax authority's published examples.
