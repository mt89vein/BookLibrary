namespace BookLibrary.IntegrationTests.Infrastructure;

public static class TestMetrics
{
    private static long _poolWaitTicks;
    private static int _pooledCount;
    private static long _cleanupTicks;
    private static int _cleanupCount;
    private static long _dbUsageTicks;
    private static int _dbUsageCount;
    private static long _webHostStartTicks;
    private static int _webHostStartCount;

    public static void RecordPoolWait(TimeSpan t)
    {
        Interlocked.Add(ref _poolWaitTicks, t.Ticks);
        Interlocked.Increment(ref _pooledCount);
    }

    public static void RecordCleanup(TimeSpan t)
    {
        Interlocked.Add(ref _cleanupTicks, t.Ticks);
        Interlocked.Increment(ref _cleanupCount);
    }

    public static void RecordDbUsage(TimeSpan t)
    {
        Interlocked.Add(ref _dbUsageTicks, t.Ticks);
        Interlocked.Increment(ref _dbUsageCount);
    }

    public static void RecordWebHostStart(TimeSpan t)
    {
        Interlocked.Add(ref _webHostStartTicks, t.Ticks);
        Interlocked.Increment(ref _webHostStartCount);
    }

    public static void Print(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);

        output.WriteLine($"Pool wait ({_pooledCount}): {TimeSpan.FromTicks(_poolWaitTicks)} or {TimeSpan.FromTicks(_poolWaitTicks).TotalMilliseconds} ms");
        output.WriteLine($"Cleanup ({_cleanupCount}):   {TimeSpan.FromTicks(_cleanupTicks)} or {TimeSpan.FromTicks(_cleanupTicks).TotalMilliseconds} ms");
        output.WriteLine($"DB usage ({_dbUsageCount}):  {TimeSpan.FromTicks(_dbUsageTicks)} or {TimeSpan.FromTicks(_dbUsageTicks).TotalMilliseconds} ms");
        output.WriteLine($"HostStart ({_webHostStartCount}):  {TimeSpan.FromTicks(_webHostStartTicks)} or {TimeSpan.FromTicks(_webHostStartTicks).TotalMilliseconds} ms");
    }
}