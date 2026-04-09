using System.Net;
using Xunit;

namespace IntelligentConnector.ScenarioTests;

/// <summary>
/// Scenario tests for the /cat endpoints.
///
/// Each test creates its own ScenarioTestWebAppFactory so it gets a fresh,
/// isolated in-memory database and never interferes with other tests.
///
/// Run manually:
///   dotnet test  (from the solution root)
///   dotnet test IntelligentConnector.ScenarioTests  (project only)
/// </summary>
public class CatEndpointScenarioTests
{
    // ── /cat/newfact ──────────────────────────────────────────────────────────

    [Fact]
    public async Task NewFact_ReturnsOkAndImage()
    {
        await using var factory = new ScenarioTestWebAppFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/cat/newfact");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("image/png", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task NewFact_CalledTwice_BothReturnImages()
    {
        // The fake connector always returns the same fact/image IDs.
        // The second call triggers the "already exists" retry loop and
        // ultimately purges old facts – the endpoint must still return an image.
        await using var factory = new ScenarioTestWebAppFactory();
        var client = factory.CreateClient();

        var first = await client.GetAsync("/cat/newfact");
        var second = await client.GetAsync("/cat/newfact");

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);
        Assert.Equal("image/png", first.Content.Headers.ContentType?.MediaType);
        Assert.Equal("image/png", second.Content.Headers.ContentType?.MediaType);
    }

    // ── /cat/rememberfact ────────────────────────────────────────────────────

    [Fact]
    public async Task RememberFact_WithNoFactsStored_ReturnsImage()
    {
        // Empty DB → endpoint should return a "nothing yet" image, not an error
        await using var factory = new ScenarioTestWebAppFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/cat/rememberfact");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("image/png", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task RememberFact_AfterGettingNewFact_ReturnsImage()
    {
        await using var factory = new ScenarioTestWebAppFactory();
        var client = factory.CreateClient();

        // Step 1 – store a fact
        var newFactResponse = await client.GetAsync("/cat/newfact");
        Assert.Equal(HttpStatusCode.OK, newFactResponse.StatusCode);

        // Step 2 – retrieve a remembered fact
        var rememberResponse = await client.GetAsync("/cat/rememberfact");
        Assert.Equal(HttpStatusCode.OK, rememberResponse.StatusCode);
        Assert.Equal("image/png", rememberResponse.Content.Headers.ContentType?.MediaType);
    }

    // ── /cat/clearfacts ───────────────────────────────────────────────────────

    [Fact]
    public async Task ClearFacts_ReturnsOkWithConfirmationMessage()
    {
        await using var factory = new ScenarioTestWebAppFactory();
        var client = factory.CreateClient();

        var response = await client.DeleteAsync("/cat/clearfacts");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("cleared", body, StringComparison.OrdinalIgnoreCase);
    }

    // ── Full workflow ─────────────────────────────────────────────────────────

    [Fact]
    public async Task FullWorkflow_GetNewFact_Remember_Clear_Remember()
    {
        await using var factory = new ScenarioTestWebAppFactory();
        var client = factory.CreateClient();

        // 1. Get and store a new fact
        var step1 = await client.GetAsync("/cat/newfact");
        Assert.Equal(HttpStatusCode.OK, step1.StatusCode);

        // 2. Remember it
        var step2 = await client.GetAsync("/cat/rememberfact");
        Assert.Equal(HttpStatusCode.OK, step2.StatusCode);

        // 3. Clear all stored facts
        var step3 = await client.DeleteAsync("/cat/clearfacts");
        Assert.Equal(HttpStatusCode.OK, step3.StatusCode);

        // 4. After clearing, remember should gracefully handle empty DB
        var step4 = await client.GetAsync("/cat/rememberfact");
        Assert.Equal(HttpStatusCode.OK, step4.StatusCode);
        Assert.Equal("image/png", step4.Content.Headers.ContentType?.MediaType);
    }
}
