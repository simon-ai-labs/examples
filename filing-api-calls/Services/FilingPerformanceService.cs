using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using static FilingServices.Samples.Services.WalkthroughOutput;

namespace FilingServices.Samples.Services;

/// <summary>
/// A small load test over REST: the bundled UStVA 2026 reference filing is sent to <c>validate</c> and
/// <c>preview</c> at rising concurrency. Every answer must be <c>200</c> with <c>error: false</c>, and the
/// 95th percentile must stay below <c>FILING_PERF_P95_MS</c> (default 2000 ms). Nothing is transmitted.
/// </summary>
public sealed class FilingPerformanceService(HttpClient http, string? apiKey, string referenceFilingPath, IReadOnlyList<int> concurrencies, int requestsPerWorker, TimeSpan p95Limit)
{
    private const string Route = "api/ustva/2026";

    public async Task<int> RunAsync()
    {
        Write(ConsoleColor.White, "\n================== Performance test ======================");
        var body = new JsonObject { ["data"] = JsonNode.Parse(await File.ReadAllTextAsync(referenceFilingPath)) };
        var failed = false;
        var results = new JsonArray();

        foreach (var operation in new[] { "validate", "preview" })
        {
            // One warm-up call: the first call of an ERiC instance loads its check plugin.
            await SendAsync($"{Route}/{operation}", body);
            foreach (var concurrency in concurrencies)
            {
                var result = await RunLevelAsync($"{Route}/{operation}", body, concurrency, concurrency * requestsPerWorker);
                failed |= !result.Passed;
                results.Add(System.Text.Json.JsonSerializer.SerializeToNode(result));
            }
        }

        await SaveJsonAsync("perf", "summary.json", results);
        return failed ? 1 : 0;
    }

    private async Task<LevelResult> RunLevelAsync(string path, JsonObject body, int concurrency, int requests)
    {
        var latencies = new double[requests];
        var statuses = new int[requests];
        var clock = Stopwatch.StartNew();
        await Parallel.ForEachAsync(Enumerable.Range(0, requests), new ParallelOptions { MaxDegreeOfParallelism = concurrency },
            async (i, _) => (statuses[i], latencies[i]) = await SendAsync(path, body));
        clock.Stop();

        Array.Sort(latencies);
        var result = new LevelResult(
            path, concurrency, requests,
            statuses.Count(status => status == 200),
            Math.Round(requests / clock.Elapsed.TotalSeconds, 1),
            Percentile(latencies, 0.50), Percentile(latencies, 0.95), Percentile(latencies, 0.99), Math.Round(latencies[^1]),
            statuses.GroupBy(status => status).ToDictionary(group => group.Key.ToString(), group => group.Count()));
        var passed = result.Passed && result.P95Ms <= p95Limit.TotalMilliseconds;
        result = result with { Passed = passed };

        Write(passed ? ConsoleColor.Green : ConsoleColor.Red,
            $"  {(passed ? "✔" : "✘")} {path.Split('/')[^1],-8} c={concurrency,3} n={requests,4} ok={result.Ok,4} " +
            $"{result.RequestsPerSecond,6:0.0} req/s  p50={result.P50Ms,5} ms  p95={result.P95Ms,5} ms  p99={result.P99Ms,5} ms  max={result.MaxMs,5} ms");
        return result;
    }

    /// <summary>Status 200 with <c>error: false</c> counts as success; anything else, including a timeout, as 0 or the status.</summary>
    private async Task<(int Status, double Milliseconds)> SendAsync(string path, JsonObject body)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, path) { Content = JsonContent.Create(body) };
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            request.Headers.Add("X-API-Key", apiKey);
        }

        var clock = Stopwatch.StartNew();
        using var response = await http.SendAsync(request);
        var answer = await response.Content.ReadFromJsonAsync<JsonNode>();
        clock.Stop();
        var status = (int)response.StatusCode == 200 && answer?["error"]?.GetValue<bool>() == true ? 422 : (int)response.StatusCode;
        return (status, clock.Elapsed.TotalMilliseconds);
    }

    private static double Percentile(double[] sorted, double percentile) =>
        Math.Round(sorted[Math.Min(sorted.Length - 1, (int)(sorted.Length * percentile))]);

    private sealed record LevelResult(
        string Path, int Concurrency, int Requests, int Ok, double RequestsPerSecond,
        double P50Ms, double P95Ms, double P99Ms, double MaxMs, Dictionary<string, int> Statuses)
    {
        public bool Passed { get; init; } = Ok == Requests;
    }
}
