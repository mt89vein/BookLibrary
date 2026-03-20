using System.Diagnostics;
using System.Threading.Channels;

namespace BookLibrary.IntegrationTests.Infrastructure;

public sealed class DatabasePool
{
    private readonly Channel<DatabaseLease> _channel;

    public DatabasePool(IReadOnlyCollection<DatabaseLease> databases)
    {
        ArgumentNullException.ThrowIfNull(databases);

        if (databases.Count == 0)
        {
            throw new InvalidOperationException("No connection strings specified");
        }

        _channel = Channel.CreateBounded<DatabaseLease>(
            new BoundedChannelOptions(databases.Count)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleWriter = false,
                SingleReader = false,
                AllowSynchronousContinuations = false
            });

        foreach (var database in databases)
        {
            _channel.Writer.TryWrite(database);
        }
    }

    public async Task<DatabaseLease> RentAsync()
    {
        var sw = Stopwatch.StartNew();

        var lease = await _channel.Reader.ReadAsync();
        sw.Stop();

        TestMetrics.RecordPoolWait(sw.Elapsed);

        await lease.ResetAsync();

        lease.StartUsage();

        return lease;
    }

    public async Task ReturnAsync(DatabaseLease lease)
    {
        ArgumentNullException.ThrowIfNull(lease);

        lease.StopUsage();
        await _channel.Writer.WriteAsync(lease);
    }
}