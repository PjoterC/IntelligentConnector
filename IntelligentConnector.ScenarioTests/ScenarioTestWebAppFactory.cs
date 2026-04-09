using IntelligentConnector.Core.Interfaces;
using IntelligentConnector.Core.Models;
using IntelligentConnector.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntelligentConnector.ScenarioTests;

/// <summary>
/// Custom WebApplicationFactory that:
///   - replaces PostgreSQL with an isolated in-memory database
///   - replaces the real HTTP-based connector with a predictable fake
///   - disables HTTPS redirection so the test HTTP client works without TLS
/// Each factory instance gets its own in-memory database, so tests are isolated.
/// </summary>
public class ScenarioTestWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            // ── Replace the real Npgsql DbContext with an in-memory one ──────────
            // AddDbContext registers several generic descriptors (options, options-
            // configuration, factories) all parameterised with AppDbContext.
            // Removing only DbContextOptions<T> leaves Npgsql's configuration
            // delegate in place, causing a "two providers" conflict.
            // The safest fix: remove every descriptor whose ServiceType is, or is
            // a closed generic of AppDbContext, then re-add cleanly.
            var toRemove = services
                .Where(d =>
                    d.ServiceType == typeof(AppDbContext) ||
                    (d.ServiceType.IsGenericType &&
                     d.ServiceType.GenericTypeArguments.Contains(typeof(AppDbContext))))
                .ToList();
            foreach (var d in toRemove)
                services.Remove(d);

            // Unique name per factory instance → fully isolated DB per test
            var dbName = $"TestDb_{Guid.NewGuid()}";
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(dbName));

            // ── Replace the real API connector with a fast, deterministic fake ──
            var connectorDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IPublicApiConnector));
            if (connectorDescriptor is not null)
                services.Remove(connectorDescriptor);

            services.AddScoped<IPublicApiConnector, FakePublicApiConnector>();

            // ── Disable HTTPS redirection so test HTTP client receives 200, not 307 ──
            services.Configure<Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionOptions>(
                opt => opt.HttpsPort = null);
        });
    }
}

/// <summary>
/// Returns fixed, deterministic data instead of making real network calls.
/// </summary>
public class FakePublicApiConnector : IPublicApiConnector
{
    // Minimal valid 1×1 PNG – keeps test payload tiny
    private static readonly byte[] s_pngBytes = Convert.FromBase64String(
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAC0lEQVQI12NgAAIABQ" +
        "AABjkB6QAAAABJRU5ErkJggg==");

    public Task<CatFact> GetCatFactAsync() =>
        Task.FromResult(new CatFact("fact-001", "Cats sleep 70% of their lives.", "img-001"));

    public Task<CatImageData> GetCatImageDataAsync() =>
        Task.FromResult(new CatImageData { Id = "img-001" });

    public Task<CatImage> GetCatImageAsync(CatFact fact, CatImageData imageData) =>
        Task.FromResult(new CatImage { ImageStream = new MemoryStream(s_pngBytes) });
}
