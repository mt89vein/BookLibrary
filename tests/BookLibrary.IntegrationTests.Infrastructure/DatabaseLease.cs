using System.Diagnostics;

namespace BookLibrary.IntegrationTests.Infrastructure;

public sealed class DatabaseLease
{
    private readonly string _adminConnectionString;
    private readonly Stopwatch _usageSw = new();

    public string DatabaseName { get; }

    public string ConnectionString { get; }

    public DatabaseLease(string connectionString, string databaseName, string adminConnectionString)
    {
        ConnectionString = connectionString;
        DatabaseName = databaseName;
        _adminConnectionString = adminConnectionString;
    }

    public void StartUsage()
    {
        _usageSw.Restart();
    }

    public void StopUsage()
    {
        _usageSw.Stop();
        TestMetrics.RecordDbUsage(_usageSw.Elapsed);
    }

    public async Task ResetAsync()
    {
        var sw = Stopwatch.StartNew();
        await DatabaseCloner.ResetDatabaseAsync(_adminConnectionString, DatabaseName);
        sw.Stop();
        TestMetrics.RecordCleanup(sw.Elapsed);
    }
}