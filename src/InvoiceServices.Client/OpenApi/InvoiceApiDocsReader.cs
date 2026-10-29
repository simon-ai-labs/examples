using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Reader;

namespace InvoiceServices.Client.OpenApi;

/// <summary>One documented operation.</summary>
public sealed record ApiOperation(string Method, string Path, string? Summary, IReadOnlyList<string> ResponseCodes);

/// <summary>What <c>/docs</c> describes, and whether it still matches the generated client.</summary>
public sealed record ApiDescription(
    Uri DocsUrl,
    Uri SpecificationUrl,
    string Title,
    string Version,
    IReadOnlyList<ApiOperation> Operations,
    IReadOnlyList<string> SecuritySchemes,
    IReadOnlyList<string> Drift)
{
    /// <summary>True when the live API offers exactly the operations and schemas the client was generated from.</summary>
    public bool MatchesGeneratedClient => Drift.Count == 0;
}

/// <summary>
/// Reads the published API documentation: discovers the OpenAPI description that the Scalar page at
/// <c>/docs/</c> renders, parses it, and compares it with the description the Kiota client was built from.
/// </summary>
public sealed partial class InvoiceApiDocsReader(HttpClient http)
{
    internal const string HttpClientName = "InvoiceServices.Docs";
    private const string SnapshotResource = "InvoiceServices.Client.OpenApi.v1.snapshot.json";

    public async Task<ApiDescription> ReadAsync(CancellationToken cancellationToken = default)
    {
        var docsUrl = new Uri(http.BaseAddress!, "/docs/");
        var page = await http.GetStringAsync(docsUrl, cancellationToken).ConfigureAwait(false);
        var specUrl = FindSpecificationUrl(page, docsUrl);
        var json = await http.GetStringAsync(specUrl, cancellationToken).ConfigureAwait(false);
        var live = Parse(json, specUrl.ToString());
        var generated = Parse(ReadSnapshot(), SnapshotResource);

        return new ApiDescription(
            docsUrl,
            specUrl,
            live.Info?.Title ?? "",
            live.Info?.Version ?? "",
            Operations(live),
            live.Components?.SecuritySchemes?.Keys.Order(StringComparer.Ordinal).ToArray() ?? [],
            Compare(generated, live));
    }

    /// <summary>
    /// Scalar keeps the description URL in its configuration object. It resolves a relative URL
    /// against the site root, which is also where ASP.NET Core serves <c>openapi/v1.json</c>.
    /// </summary>
    internal static Uri FindSpecificationUrl(string docsPage, Uri docsUrl)
    {
        var match = SourceUrl().Match(docsPage);
        if (!match.Success)
            throw new InvalidOperationException($"No OpenAPI source found on {docsUrl}.");
        var url = Regex.Unescape(match.Groups["url"].Value);
        return Uri.TryCreate(url, UriKind.Absolute, out var absolute) && absolute.Scheme is "https" or "http"
            ? absolute
            : new Uri(new Uri(docsUrl.GetLeftPart(UriPartial.Authority)), url.TrimStart('/'));
    }

    internal static OpenApiDocument Parse(string json, string location)
    {
        var result = OpenApiDocument.Parse(json, OpenApiConstants.Json, new OpenApiReaderSettings());
        if (result.Document is null || result.Diagnostic?.Errors.Count > 0)
            throw new InvalidOperationException($"The OpenAPI description at {location} is invalid: " +
                string.Join("; ", result.Diagnostic?.Errors.Select(e => e.Message) ?? []));
        return result.Document;
    }

    internal static IReadOnlyList<string> Compare(OpenApiDocument generated, OpenApiDocument live)
    {
        static HashSet<string> Surface(OpenApiDocument document) =>
        [
            .. Operations(document).Select(o => $"operation {o.Method} {o.Path}"),
            .. document.Components?.Schemas?.Keys.Select(k => $"schema {k}") ?? [],
        ];

        var before = Surface(generated);
        var after = Surface(live);
        return
        [
            .. after.Except(before).Order(StringComparer.Ordinal).Select(x => $"added {x}"),
            .. before.Except(after).Order(StringComparer.Ordinal).Select(x => $"removed {x}"),
        ];
    }

    private static List<ApiOperation> Operations(OpenApiDocument document) =>
    [
        .. from path in document.Paths
           from operation in path.Value.Operations ?? []
           orderby path.Key, operation.Key.Method
           select new ApiOperation(
               operation.Key.Method,
               path.Key,
               operation.Value.Summary,
               operation.Value.Responses?.Keys.Order(StringComparer.Ordinal).ToArray() ?? []),
    ];

    private static string ReadSnapshot()
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(SnapshotResource)
            ?? throw new InvalidOperationException("The embedded OpenAPI snapshot is missing.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    [GeneratedRegex("""
        "sources"\s*:\s*\[\s*\{[^}]*"url"\s*:\s*"(?<url>[^"]+)"
        """, RegexOptions.CultureInvariant)]
    private static partial Regex SourceUrl();
}
