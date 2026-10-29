using System.Diagnostics;
using System.Globalization;
using InvoiceServices.Client.OpenApi.Generated.Models;

namespace InvoiceServices.Client.Cli;

/// <summary>What one scenario run observed; compared between transports by the smoke command.</summary>
internal sealed record ScenarioResult(
    string Transport,
    bool Valid,
    IReadOnlyList<string> IssueRules,
    string? InvoiceId,
    decimal? PayableAmount,
    string? ConvertedFileName,
    string? RenderedContentType)
{
    public IReadOnlyList<string> Differences(ScenarioResult other)
    {
        List<string> differences = [];
        void Check<T>(string name, T left, T right)
        {
            if (!EqualityComparer<T>.Default.Equals(left, right)) differences.Add($"{name}: {left} vs {right}");
        }
        Check("valid", Valid, other.Valid);
        Check("issues", string.Join(",", IssueRules), string.Join(",", other.IssueRules));
        Check("invoiceId", InvoiceId, other.InvoiceId);
        Check("payableAmount", PayableAmount, other.PayableAmount);
        Check("convertedFileName", ConvertedFileName, other.ConvertedFileName);
        Check("renderedContentType", RenderedContentType, other.RenderedContentType);
        return differences;
    }
}

/// <summary>Capabilities, validate, read, convert UBL → CII and render PDF, identical for every transport.</summary>
internal sealed class InvoiceScenario(TextWriter log)
{
    public async Task<ScenarioResult> RunAsync(IInvoiceOperations operations, FileInfo file, DirectoryInfo output, CancellationToken cancellationToken)
    {
        log.WriteLine($"[{operations.Transport}] {file.Name}");
        var document = new DocumentInputDto
        {
            Content = await File.ReadAllBytesAsync(file.FullName, cancellationToken),
            FileName = file.Name,
            ContentType = "application/xml",
        };

        var capabilities = await Step(operations, "capabilities", ct => operations.GetCapabilitiesAsync(ct), cancellationToken);
        log.WriteLine($"  standards: {string.Join(", ", capabilities.Standards?.Select(s => $"{s.Format} {s.Version}") ?? [])}");

        var validation = await Step(operations, "validate", ct => operations.ValidateAsync(new() { Document = document }, ct), cancellationToken);
        var rules = validation.Issues?.Select(i => i.RuleId ?? i.Code ?? "?").Order(StringComparer.Ordinal).ToArray() ?? [];
        log.WriteLine($"  valid: {validation.Valid}, issues: {(rules.Length == 0 ? "none" : string.Join(", ", rules))}");

        var read = await Step(operations, "read", ct => operations.ReadAsync(new() { Document = document }, ct), cancellationToken);
        // The description types amounts as strings while the service sends JSON numbers; see README.
        var payable = decimal.TryParse(read.Invoice?.Totals?.PayableAmount, NumberStyles.Number, CultureInfo.InvariantCulture, out var amount)
            ? amount : (decimal?)null;
        log.WriteLine($"  invoice {read.Invoice?.InvoiceId}, payable {payable} {read.Invoice?.InvoiceCurrency}");

        var converted = await Step(operations, "convert", ct => operations.ConvertAsync(new()
        {
            Document = document,
            Target = new() { Format = ApiInvoiceFormat.XRechnung, Version = "3.0.2", Profile = ApiInvoiceProfile.XRechnung, Syntax = ApiInvoiceSyntax.Cii },
        }, ct), cancellationToken);
        await Save(output, operations.Transport, converted.Document?.FileName, converted.Document?.Content, cancellationToken);

        var rendered = await Step(operations, "render", ct => operations.RenderAsync(new()
        {
            Document = document,
            OutputFormat = ApiRenderOutputFormat.Pdf,
        }, ct), cancellationToken);
        await Save(output, operations.Transport, rendered.FileName, rendered.Content, cancellationToken);

        return new ScenarioResult(operations.Transport, validation.Valid == true, rules, read.Invoice?.InvoiceId, payable,
            converted.Document?.FileName, rendered.ContentType);
    }

    private async Task<T> Step<T>(IInvoiceOperations operations, string name, Func<CancellationToken, Task<T>> call, CancellationToken cancellationToken)
    {
        var watch = Stopwatch.StartNew();
        var result = await call(cancellationToken);
        log.WriteLine($"  {name,-12} {watch.ElapsedMilliseconds,6} ms  [{operations.Transport}]");
        return result;
    }

    private async Task Save(DirectoryInfo output, string transport, string? fileName, byte[]? content, CancellationToken cancellationToken)
    {
        if (fileName is null || content is null) throw new InvalidOperationException("The service returned no document.");
        var directory = Directory.CreateDirectory(Path.Combine(output.FullName, transport));
        var path = Path.Combine(directory.FullName, Path.GetFileName(fileName));
        await File.WriteAllBytesAsync(path, content, cancellationToken);
        log.WriteLine($"  saved {path} ({content.Length:N0} bytes)");
    }
}
