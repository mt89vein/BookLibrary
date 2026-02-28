using BookLibrary.Application;
using BookLibrary.Domain.Aggregates.Books;
using BookLibrary.Infrastructure;
using BookLibrary.IntegrationTests.Infrastructure;
using BookLibrary.TestHelpers;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Net.Http.Json;

namespace BookLibrary.IntegrationTests.NUnit.Fixtures;

/// <summary>
/// Base API test.
/// </summary>
[TestFixture]
[Parallelizable(ParallelScope.All)]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public abstract class ApiTestBase : WebApplicationFactory<BookLibraryEntryPoint>
{
    /// <summary>
    /// Current db lease for test.
    /// </summary>
    private DatabaseLease? _lease;

    private HttpClient? _httpClient;
    private ApplicationContext? _db;

    protected virtual string? User => "integration-test@user.com";

    protected HttpClient HttpClient => _httpClient ??= CreateClient();

    protected ApplicationContext Db => _db ??= Services.GetRequiredService<ApplicationContext>();

    [SetUp]
    public async Task SetUpAsync()
    {
        try
        {
            _lease = await GlobalSetup.Pool.RentAsync();

            if (!string.IsNullOrWhiteSpace(User))
            {
                await RegisterUserAsync(User);
                await LoginUserAsync(User);
            }
        }
        catch (Exception)
        {
            await TearDownAsync();

            throw;
        }
    }

    [TearDown]
    public async Task TearDownAsync()
    {
        await DisposeAsync();
        if (_lease is not null)
        {
            await GlobalSetup.Pool.ReturnAsync(_lease);
            _lease = null;
        }
    }

    protected override TestServer CreateServer(IWebHostBuilder builder)
    {
        var sw = Stopwatch.StartNew();
        var server = base.CreateServer(builder);
        sw.Stop();

        TestMetrics.RecordWebHostStart(sw.Elapsed);

        return server;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var sw = Stopwatch.StartNew();
        var host = base.CreateHost(builder);
        sw.Stop();

        TestMetrics.RecordWebHostStart(sw.Elapsed);

        return host;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.ConfigureWebHost(builder);

        if (_lease is null)
        {
            throw new InvalidOperationException("Database lease is not acquired.");
        }

        builder.ConfigureAppConfiguration(b =>
        {
            ConfigureTestConfiguration(b);
            b.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _lease.ConnectionString,
                [$"FeatureManagement:{FeatureFlags.AUTO_MIGRATIONS_ENABLED}"] = bool.FalseString
            });
        });

        builder.ConfigureServices(services => services.AddCookiePolicy(p =>
        {
            p.HttpOnly = HttpOnlyPolicy.None;
            p.Secure = CookieSecurePolicy.None;
        }));
    }

    protected virtual void ConfigureTestConfiguration(IConfigurationBuilder config)
    {
        // for easier custom configuration per test
    }

    protected async Task RegisterUserAsync(string email)
    {
        using var response = await HttpClient.PostAsJsonAsync(
            new Uri("api/v1/abonents", UriKind.Relative),
            new { Email = email, Surname = "Integration", Name = "Testing", Patronymic = "User" });

        response.EnsureSuccessStatusCode();
    }

    protected async Task LoginUserAsync(string email)
    {
        using var response = await HttpClient.PostAsync(new Uri("api/v1/account/login?email=" + email, UriKind.Relative), null);

        response.EnsureSuccessStatusCode();
    }

    protected async Task<Book> SeedBookAsync(Action<BookBuilder>? configure = null)
    {
        var builder = new BookBuilder(Services.GetRequiredService<TimeProvider>());

        configure?.Invoke(builder);

        var book = builder.Build();

        Db.Books.Add(book);
        await Db.SaveChangesAsync();
        Db.ChangeTracker.Clear();

        return book;
    }
}