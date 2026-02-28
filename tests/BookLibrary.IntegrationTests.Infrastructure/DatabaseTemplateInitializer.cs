using Npgsql;

namespace BookLibrary.IntegrationTests.Infrastructure;

public static class DatabaseTemplateInitializer
{
    /// <summary>
    /// Configures template.
    /// </summary>
    /// <param name="adminConnectionString">Connection string.</param>
    /// <param name="migrate">Migrate and seed callback.</param>
    public static async Task InitializeTemplateAsync(
        string adminConnectionString,
        Func<string, Task>? migrate = null
    )
    {
        ArgumentNullException.ThrowIfNull(adminConnectionString);

        await using var conn = new NpgsqlConnection(adminConnectionString);
        await conn.OpenAsync();

        await CreateTemplateDbAsync(conn);
        await InitDbAsync(adminConnectionString, migrate);
        await RestrictTemplateConnectionsAsync(conn);
    }

    private static async Task CreateTemplateDbAsync(NpgsqlConnection conn)
    {
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = $"CREATE DATABASE {Constants.TEMPLATE_DB_NAME};";
        await cmd.ExecuteNonQueryAsync();
    }

    private static async Task InitDbAsync(string adminConnectionString, Func<string, Task>? migrate)
    {
        if (migrate is not null)
        {
            await migrate(new NpgsqlConnectionStringBuilder(adminConnectionString)
            {
                Database = Constants.TEMPLATE_DB_NAME,
            }.ToString());
        }
    }

    private static async Task RestrictTemplateConnectionsAsync(NpgsqlConnection conn)
    {
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = $"UPDATE pg_database SET datistemplate = TRUE WHERE datname = '{Constants.TEMPLATE_DB_NAME}';";
        await cmd.ExecuteNonQueryAsync();
    }
}