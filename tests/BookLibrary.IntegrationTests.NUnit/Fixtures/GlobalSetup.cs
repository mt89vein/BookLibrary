using BookLibrary.Infrastructure;
using BookLibrary.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Testcontainers.PostgreSql;

[assembly: Parallelizable(ParallelScope.Fixtures)]
[assembly: LevelOfParallelism(Constants.LEVEL_OF_PARALLELISM)]

[SetUpFixture]
public sealed class GlobalSetup
{
    private string? _adminConnectionString;
    private PostgreSqlContainer? _postgreSqlContainer;

    public static DatabasePool Pool { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task InitAsync()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTests");
        Environment.SetEnvironmentVariable("DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE", "false");

        if (_postgreSqlContainer is not null)
        {
            throw new InvalidOperationException("Already initialized");
        }

        _postgreSqlContainer = PostgresContainerFactory.Create();
        await _postgreSqlContainer.StartAsync();

        _adminConnectionString = new NpgsqlConnectionStringBuilder(_postgreSqlContainer.GetConnectionString())
        {
            Database = "postgres",
            IncludeErrorDetail = true,
            Pooling = false,
            ApplicationName = "BookLibrary.IntegrationTests.Admin"
        }.ToString();

        await DatabaseTemplateInitializer.InitializeTemplateAsync(_adminConnectionString, static async connectionString =>
        {
            await using var factory = new WebApplicationFactory<BookLibraryEntryPoint>()
                .WithWebHostBuilder(o => o.UseSetting("ConnectionStrings:DefaultConnection", connectionString)
            );
            var context = factory.Services.GetRequiredService<ApplicationContext>();
            await context.Database.MigrateAsync();
            await context.Database.EnsureCreatedAsync();
        });

        var connections = new List<DatabaseLease>();

        for (var i = 0; i < Constants.LEVEL_OF_PARALLELISM; i++)
        {
            var dbName = $"bl_test_db_{i}";
            var builder = new NpgsqlConnectionStringBuilder(_postgreSqlContainer.GetConnectionString())
            {
                Database = dbName,
                IncludeErrorDetail = true,
                Pooling = false,
                ApplicationName = "BookLibrary.IntegrationTests.PooledDb"
            };

            await DatabaseCloner.CreateFromTemplateAsync(_adminConnectionString, dbName);

            connections.Add(new DatabaseLease(
                    builder.ToString(),
                    dbName,
                    _adminConnectionString
                )
            );
        }

        Pool = new DatabasePool(connections);
    }

    [OneTimeTearDown]
    public async Task CleanupAsync()
    {
        TestMetrics.Print(Console.Out);
        if (_postgreSqlContainer is not null)
        {
            await _postgreSqlContainer.DisposeAsync();
        }
    }
}