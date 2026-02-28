using DotNet.Testcontainers.Builders;
using Testcontainers.PostgreSql;

namespace BookLibrary.IntegrationTests.Infrastructure;

public static class PostgresContainerFactory
{
    public static PostgreSqlContainer Create()
    {
        return new PostgreSqlBuilder("postgres:18")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithWaitStrategy(Wait
                .ForUnixContainer()
                .UntilCommandIsCompleted("pg_isready"))
            .WithCleanUp(true)
            .WithCommand("-c", $"max_connections={100 + (10 * Constants.LEVEL_OF_PARALLELISM)}")
            .Build();
    }
}