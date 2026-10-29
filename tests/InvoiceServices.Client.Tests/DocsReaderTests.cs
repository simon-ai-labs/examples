using System.Net;
using System.Reflection;
using System.Text.Json.Nodes;
using InvoiceServices.Client.OpenApi;

namespace InvoiceServices.Client.Tests;

public sealed class DocsReaderTests
{
    // Shape of the page Scalar.AspNetCore serves at /docs/.
    private const string ScalarPage = """
        <script type="module">
            import { initialize } from './scalar.aspnetcore.js'
            initialize('%2Fdocs%2F', false,
            {"authentication":{"preferredSecurityScheme":["MarketplaceApiKey"]},"_integration":"dotnet","sources":[{"title":"v1","url":"openapi/v1.json"}]}, '')
        </script>
        """;

    private static string Snapshot()
    {
        using var stream = typeof(InvoiceApiDocsReader).Assembly.GetManifestResourceStream("InvoiceServices.Client.OpenApi.v1.snapshot.json")!;
        return new StreamReader(stream).ReadToEnd();
    }

    [Theory]
    [InlineData("openapi/v1.json", "https://api.test/openapi/v1.json")]
    [InlineData("/openapi/v1.json", "https://api.test/openapi/v1.json")]
    [InlineData("https://other.test/spec.json", "https://other.test/spec.json")]
    public void FindsTheDescriptionScalarRenders(string url, string expected)
    {
        var page = ScalarPage.Replace("openapi/v1.json", url, StringComparison.Ordinal);
        Assert.Equal(new Uri(expected), InvoiceApiDocsReader.FindSpecificationUrl(page, new Uri("https://api.test/docs/")));
    }

    [Fact]
    public void PageWithoutSourcesIsRejected() =>
        Assert.Throws<InvalidOperationException>(() => InvoiceApiDocsReader.FindSpecificationUrl("<html></html>", new Uri("https://api.test/docs/")));

    [Fact]
    public async Task ReadsDocsAndReportsNoDriftForTheGeneratedDescription()
    {
        var description = await Reader(Snapshot()).ReadAsync(TestContext.Current.CancellationToken);

        Assert.Equal(new Uri("https://api.test/openapi/v1.json"), description.SpecificationUrl);
        Assert.Equal("v1", description.Version);
        Assert.Contains(description.Operations, o => o is { Method: "POST", Path: "/v1/invoices/validate" });
        Assert.Contains("MarketplaceApiKey", description.SecuritySchemes);
        Assert.True(description.MatchesGeneratedClient);
    }

    [Fact]
    public async Task ReportsAddedAndRemovedOperationsAndSchemas()
    {
        var spec = JsonNode.Parse(Snapshot())!.AsObject();
        spec["paths"]!.AsObject().Remove("/v1/invoices/render");
        spec["paths"]!["/v1/invoices/sign"] = JsonNode.Parse("""{"post":{"responses":{"200":{"description":"OK"}}}}""");
        spec["components"]!["schemas"]!.AsObject().Remove("PayeeDto");

        var description = await Reader(spec.ToJsonString()).ReadAsync(TestContext.Current.CancellationToken);

        Assert.False(description.MatchesGeneratedClient);
        Assert.Contains("added operation POST /v1/invoices/sign", description.Drift);
        Assert.Contains("removed operation POST /v1/invoices/render", description.Drift);
        Assert.Contains("removed schema PayeeDto", description.Drift);
    }

    [Fact]
    public async Task InvalidDescriptionFailsClearly()
    {
        var failure = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Reader("""{"openapi":"3.1.1","info":{}}""").ReadAsync(TestContext.Current.CancellationToken));
        Assert.Contains("openapi/v1.json", failure.Message, StringComparison.Ordinal);
    }

    private static InvoiceApiDocsReader Reader(string specification) =>
        new(new HttpClient(new Site(specification)) { BaseAddress = new Uri("https://api.test") });

    private sealed class Site(string specification) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(request.RequestUri!.AbsolutePath switch
            {
                "/docs/" => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(ScalarPage) },
                "/openapi/v1.json" => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(specification) },
                _ => new HttpResponseMessage(HttpStatusCode.NotFound),
            });
    }
}
