using Npgsql;

namespace BookLibrary.IntegrationTests.Infrastructure;

public static class DatabaseCloner
{
    /// <summary>
    /// Create fresh database from template (with schema and seeded data).
    /// </summary>
    /// <param name="adminConnectionString">Connection string.</param>
    /// <param name="dbName">Database name.</param>
    public static async Task CreateFromTemplateAsync(
        string adminConnectionString,
        string dbName
    )
    {
        await using var conn = new NpgsqlConnection(adminConnectionString);
        await conn.OpenAsync();

        await CreateFromTemplateAsync(conn, dbName);
    }

    /// <summary>
    /// Fully resets database to initial state.
    /// </summary>
    /// <param name="adminConnectionString">Connection string.</param>
    /// <param name="dbName">Database name.</param>
    public static async Task ResetDatabaseAsync(
        string adminConnectionString,
        string dbName
    )
    {
        await using var conn = new NpgsqlConnection(adminConnectionString);
        await conn.OpenAsync();

        await KillOpenConnectionsAsync(conn, dbName);
        await DropDatabaseAsync(conn, dbName);
        await CreateFromTemplateAsync(conn, dbName);
    }

    private static async Task KillOpenConnectionsAsync(NpgsqlConnection conn, string dbName)
    {
        await using var cmd = conn.CreateCommand();
        var terminate = $"""
                             SELECT pg_terminate_backend(pid)
                             FROM pg_stat_activity
                             WHERE datname = '{dbName}'
                               AND pid <> pg_backend_pid();
                         """;

        cmd.CommandText = terminate;
        await cmd.ExecuteNonQueryAsync();
    }

    private static async Task DropDatabaseAsync(NpgsqlConnection conn, string dbName)
    {
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = $"DROP DATABASE IF EXISTS {dbName};";
        await cmd.ExecuteNonQueryAsync();
    }

    private static async Task CreateFromTemplateAsync(
        NpgsqlConnection conn,
        string dbName
    )
    {
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = $"CREATE DATABASE {dbName} TEMPLATE {Constants.TEMPLATE_DB_NAME};";
        await cmd.ExecuteNonQueryAsync();
    }
}