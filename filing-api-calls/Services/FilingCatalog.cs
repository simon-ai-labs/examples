using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace FilingServices.Samples.Services;

/// <summary>One request body from the contract's <c>examples</c>.</summary>
public sealed record FilingExample(string Name, JsonNode Body);

/// <summary>One operation (<c>validate</c>, <c>preview</c>, <c>filings</c>, <c>decode</c>, …) of one route.</summary>
public sealed record FilingOperation(string Name, string Path, IReadOnlyList<FilingExample> Examples);

/// <summary>The latest Stand of one Datenart: its route, its contract and the operations the contract documents.</summary>
public sealed record FilingStand(string Datenart, string Version, string Route, string Document, IReadOnlyList<FilingOperation> Operations)
{
    public FilingOperation? Find(string operation) => Operations.FirstOrDefault(candidate => candidate.Name == operation);
}

/// <summary>
/// Picks the latest Stand of every Datenart. A Datenart is a route without its last version segment
/// (<c>/api/ustva/2026</c> → <c>/api/ustva</c>, <c>/api/kmv/bsb/3</c> → <c>/api/kmv/bsb</c>); routes
/// without a version (Bereitstellung, Protokolle, SpezRecht) are a Datenart of their own.
/// </summary>
public static partial class FilingCatalog
{
    /// <summary>
    /// From <c>/openapi/index.json</c>: the latest document per family when the Stand is a number,
    /// every document when it is not (KMV names its Mitteilungsart there, E-Bilanz its taxonomy parameter).
    /// </summary>
    public static IReadOnlyList<string> LatestDocuments(JsonNode index) =>
        index["documents"]!.AsArray()
            .Select(entry => (
                Name: entry!["name"]!.GetValue<string>(),
                Family: entry["family"]!.GetValue<string>(),
                Stand: entry["stand"]?.GetValue<string>()))
            .GroupBy(entry => entry.Stand is { } stand && VersionPattern().IsMatch(stand) ? entry.Family : entry.Name)
            .Select(group => group.MaxBy(entry => entry.Stand ?? string.Empty, VersionComparer.Instance).Name)
            .Order(StringComparer.Ordinal)
            .ToList();

    /// <summary>From <c>list_filing_routes</c>: the documents that hold the latest route of each Datenart.</summary>
    public static IReadOnlyList<string> LatestDocumentsFromRoutes(JsonNode routes) =>
        routes["routes"]!.AsArray()
            .Select(entry => (Route: entry!["route"]!.GetValue<string>(), Document: entry["openApiDocument"]!.GetValue<string>()))
            .GroupBy(entry => SplitRoute(entry.Route).Datenart)
            .Select(group => group.MaxBy(entry => SplitRoute(entry.Route).Version, VersionComparer.Instance).Document)
            .Distinct()
            .Order(StringComparer.Ordinal)
            .ToList();

    /// <summary>Reads the latest route of each Datenart a contract describes, with every documented operation.</summary>
    public static IEnumerable<FilingStand> ReadStands(string document, JsonNode contract)
    {
        var operations =
            from path in contract["paths"]!.AsObject()
            from method in path.Value!.AsObject()
            let concretePath = ResolvePathParameters(path.Key, method.Value!)
            let separator = concretePath.LastIndexOf('/')
            select (Route: concretePath[..separator], Operation: new FilingOperation(
                concretePath[(separator + 1)..],
                concretePath,
                ReadExamples(method.Value!, templated: path.Key.Contains('{'))));

        return operations
            .GroupBy(entry => SplitRoute(entry.Route).Datenart)
            .Select(datenart => datenart.GroupBy(entry => entry.Route)
                .MaxBy(route => SplitRoute(route.Key).Version, VersionComparer.Instance)!)
            .Select(route => new FilingStand(
                SplitRoute(route.Key).Datenart,
                SplitRoute(route.Key).Version,
                route.Key,
                document,
                route.Select(entry => entry.Operation).ToList()));
    }

    /// <summary>Datenarten (routes without their version, such as <c>/api/vno-vmv-loeschung</c>) that are left out.</summary>
    public static IReadOnlySet<string> Excluded { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    /// <summary>Datenarten that are checked but never sent as a simulation to ELSTER.</summary>
    public static IReadOnlySet<string> NotSimulated { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    /// <summary>Keeps one Stand per Datenart, the one with the highest version, except the <see cref="Excluded"/> ones.</summary>
    public static IReadOnlyList<FilingStand> Latest(IEnumerable<FilingStand> stands) =>
        stands.Where(stand => !Excluded.Contains(stand.Datenart))
            .GroupBy(stand => stand.Datenart)
            .Select(group => group.MaxBy(stand => stand.Version, VersionComparer.Instance)!)
            .OrderBy(stand => stand.Datenart, StringComparer.Ordinal)
            .ToList();

    private static (string Datenart, string Version) SplitRoute(string route)
    {
        var separator = route.LastIndexOf('/');
        var last = route[(separator + 1)..];
        return VersionPattern().IsMatch(last) ? (route[..separator], last) : (route, string.Empty);
    }

    /// <summary>
    /// A route such as <c>/api/ebilanz/{taxonomie}</c> takes the parameter's documented example. Its body
    /// examples are one per taxonomy, so only the first one, which belongs to that example, is sent.
    /// </summary>
    private static string ResolvePathParameters(string path, JsonNode operation)
    {
        foreach (var parameter in operation["parameters"]?.AsArray() ?? [])
        {
            if (parameter?["in"]?.GetValue<string>() == "path" && parameter["example"] is { } example)
            {
                path = path.Replace("{" + parameter["name"]!.GetValue<string>() + "}", example.GetValue<string>(), StringComparison.Ordinal);
            }
        }

        return path;
    }

    private static List<FilingExample> ReadExamples(JsonNode operation, bool templated)
    {
        var content = operation["requestBody"]?["content"]?["application/json"];
        var examples = content?["examples"]?.AsObject()
            .Select(example => new FilingExample(example.Key, example.Value!["value"]!.DeepClone()))
            .ToList() ?? [];
        if (examples.Count == 0 && content?["example"] is { } single)
        {
            examples.Add(new FilingExample("example", single.DeepClone()));
        }

        return templated ? examples.Take(1).ToList() : examples;
    }

    [GeneratedRegex(@"^\d+(\.\d+)*$", RegexOptions.CultureInvariant)]
    private static partial Regex VersionPattern();

    /// <summary>Compares <c>2</c> &lt; <c>10</c> and <c>6.8</c> &lt; <c>6.9</c> numerically, anything else ordinally.</summary>
    private sealed class VersionComparer : IComparer<string>
    {
        public static VersionComparer Instance { get; } = new();

        public int Compare(string? x, string? y)
        {
            var left = (x ?? string.Empty).Split('.');
            var right = (y ?? string.Empty).Split('.');
            for (var i = 0; i < Math.Max(left.Length, right.Length); i++)
            {
                var a = i < left.Length ? left[i] : "0";
                var b = i < right.Length ? right[i] : "0";
                var result = long.TryParse(a, out var na) && long.TryParse(b, out var nb)
                    ? na.CompareTo(nb)
                    : string.CompareOrdinal(a, b);
                if (result != 0)
                {
                    return result;
                }
            }

            return 0;
        }
    }
}
