using System.Text.Json.Nodes;

namespace InvoiceServices.Samples.Services;

public static class InvoiceDataFactory
{
    // The walkthrough's primary document is XRechnung: it is the only format whose capabilities
    // cover validate, read, convert and render, which the following steps exercise end to end.
    public static JsonObject CreateWalkthroughInvoiceRequest() =>
        CreateInvoiceRequest("xRechnung", "3.0.2", "xRechnung", "ubl");

    public static JsonObject CreateInvoiceRequest(string format, string version, string? profile, string? syntax)
    {
        var target = new JsonObject
        {
            ["format"] = format,
            ["version"] = version
        };
        if (profile != null)
        {
            target["profile"] = profile;
        }
        if (syntax != null)
        {
            target["syntax"] = syntax;
        }

        return new JsonObject
        {
            ["target"] = target,
            ["invoice"] = new JsonObject
            {
                ["typeCode"] = "380",
                ["invoiceId"] = "INV-2026-0001",
                ["issueDate"] = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                ["dueDate"] = DateTime.UtcNow.AddDays(30).ToString("yyyy-MM-dd"),
                ["invoiceCurrency"] = "EUR",
                ["buyerReference"] = "04011000-12345-34",
                ["seller"] = new JsonObject
                {
                    ["name"] = "Tech Solutions GmbH",
                    ["identifiers"] = new JsonArray(),
                    ["electronicAddress"] = new JsonObject
                    {
                        ["schemeId"] = "EM",
                        ["value"] = "seller@techsolutions.example"
                    },
                    ["legalRegistrationIdentifier"] = new JsonObject
                    {
                        ["value"] = "HRB 12345",
                        ["schemeIdentifier"] = "0002"
                    },
                    ["vatIdentifier"] = "DE123456789",
                    ["taxRegistrationId"] = "123/456/7890",
                    ["address"] = new JsonObject
                    {
                        ["line1"] = "Musterstraße 42",
                        ["city"] = "Berlin",
                        ["postCode"] = "10115",
                        ["countryCode"] = "DE"
                    },
                    ["contact"] = string.Equals(profile, "basic", StringComparison.OrdinalIgnoreCase) ? null : new JsonObject
                    {
                        ["name"] = "Max Mustermann",
                        ["telephone"] = "+49 30 1234567",
                        ["email"] = "accounting@techsolutions.example"
                    }
                },
                ["buyer"] = new JsonObject
                {
                    ["name"] = "Enterprise Services AG",
                    ["identifiers"] = new JsonArray(),
                    ["electronicAddress"] = new JsonObject
                    {
                        ["schemeId"] = "EM",
                        ["value"] = "buyer@enterprise.example"
                    },
                    ["legalRegistrationIdentifier"] = new JsonObject
                    {
                        ["value"] = "HRB 98765",
                        ["schemeIdentifier"] = "0002"
                    },
                    ["vatIdentifier"] = "DE987654321",
                    ["address"] = new JsonObject
                    {
                        ["line1"] = "Hauptstraße 100",
                        ["city"] = "München",
                        ["postCode"] = "80331",
                        ["countryCode"] = "DE"
                    }
                },
                ["payment"] = new JsonObject
                {
                    ["meansCode"] = "58",
                    ["remittanceInformation"] = "INV-2026-0001",
                    ["creditTransfers"] = new JsonArray
                    {
                        new JsonObject
                        {
                            ["accountId"] = "DE89370400440532013000",
                            ["accountName"] = string.Equals(profile, "basic", StringComparison.OrdinalIgnoreCase) ? null : "Tech Solutions GmbH"
                        }
                    }
                },
                ["precedingInvoiceReferences"] = new JsonArray(),
                ["lines"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["lineId"] = "1",
                        ["quantity"] = 10,
                        ["unitCode"] = "HUR",
                        ["netUnitPrice"] = 100.00m,
                        ["lineNetAmount"] = 1000.00m,
                        ["item"] = new JsonObject
                        {
                            ["name"] = "IT Consulting Services",
                            ["classifications"] = new JsonArray(),
                            ["attributes"] = new JsonArray(),
                            ["vatCategory"] = new JsonObject
                            {
                                ["code"] = "S",
                                ["rate"] = 19.00m
                            }
                        },
                        ["allowancesCharges"] = new JsonArray()
                    }
                },
                ["allowancesCharges"] = new JsonArray(),
                ["vatBreakdowns"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["category"] = new JsonObject
                        {
                            ["code"] = "S",
                            ["rate"] = 19.00m
                        },
                        ["taxableAmount"] = 1000.00m,
                        ["taxAmount"] = 190.00m
                    }
                },
                ["totals"] = new JsonObject
                {
                    ["lineNetAmount"] = 1000.00m,
                    ["taxExclusiveAmount"] = 1000.00m,
                    ["taxTotalAmount"] = 190.00m,
                    ["taxInclusiveAmount"] = 1190.00m,
                    ["payableAmount"] = 1190.00m
                },
                ["notes"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["text"] = "Payment due within 30 days without deductions."
                    }
                },
                ["supportingDocuments"] = new JsonArray()
            }
        };
    }
}
