using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace BookLibrary.Infrastructure.OpenTelemetry;

internal sealed partial class ExceptionTracker : IDisposable
{
    private readonly Meter _meter;
    private readonly ILogger<ExceptionTracker> _logger;

    private readonly Counter<long> _exceptionsCounter;

    private static readonly ConcurrentDictionary<Type, string> _trimmedTypeNames = new();

    private const int LABEL_MAX_LENGTH = 127;
    private const int LOG_EVERY_N_EXCEPTIONS = 100;

    [ThreadStatic]
    private static bool _handlingFirstChanceException;

    [ThreadStatic]
    private static Dictionary<Type, long>? _sampler;

    private static bool _started;

    public ExceptionTracker(
        IMeterFactory meterFactory,
        ILogger<ExceptionTracker> logger
    )
    {
        _logger = logger;

        _meter = meterFactory.Create(GetType().FullName!);
        _exceptionsCounter = _meter.CreateCounter<long>(name: "dotnet.exceptions", description: "Count of throwed exceptions.");
    }

    public void Dispose()
    {
        _meter.Dispose();

        Stop();
    }

    public void Start()
    {
        if (_started)
        {
            return;
        }

        _started = true;
        AppDomain.CurrentDomain.FirstChanceException += HandleFirstChanceException;
    }

    public void Stop()
    {
        if (!_started)
        {
            return;
        }

        AppDomain.CurrentDomain.FirstChanceException -= HandleFirstChanceException;
    }

    private void HandleFirstChanceException(object? sender, FirstChanceExceptionEventArgs e)
    {
        if (_handlingFirstChanceException)
        {
            return;
        }

        if (e.Exception is TaskCanceledException or OperationCanceledException)
        {
            return;
        }

        _handlingFirstChanceException = true;

        try
        {
            var exceptionType = e.Exception.GetType();

            _sampler ??= new Dictionary<Type, long>();

            ref var counter = ref CollectionsMarshal.GetValueRefOrAddDefault(_sampler, exceptionType, out var exists);

            var needLog = !exists || Interlocked.Increment(ref counter) % LOG_EVERY_N_EXCEPTIONS == 0;

            if (needLog)
            {
                LogExceptionSample(e.Exception, exceptionType.FullName);
            }

            _exceptionsCounter.Add(1, new KeyValuePair<string, object?>("error_type", Trim(exceptionType)));
        }
        catch (Exception)
        {
            // skip
        }
        finally
        {
            _handlingFirstChanceException = false;
        }
    }

    private static string Trim(Type type)
    {
        return _trimmedTypeNames.GetOrAdd(type, static type =>
        {
            var typeFullName = type.FullName!;

            Span<char> typeLabel = stackalloc char[Math.Min(LABEL_MAX_LENGTH, typeFullName.Length)];

            typeFullName.AsSpan(0, typeLabel.Length).CopyTo(typeLabel);

            var writePos = 0;
            for (var readPos = 0; readPos < typeLabel.Length; readPos++)
            {
                var c = typeLabel[readPos];

                if (char.IsAscii(c))
                {
                    typeLabel[writePos++] = c;
                }
            }

            typeLabel = typeLabel[..writePos];

            return typeLabel.ToString();
        });
    }

    [LoggerMessage(
        eventId: 0,
        level: LogLevel.Information,
        message: "Sample of throwed exception {ExceptionType}"
    )]
    private partial void LogExceptionSample(Exception ex, string? exceptionType);
}

internal sealed class ExceptionTrackerLifecycle : IHostedService
{
    private readonly ExceptionTracker _exceptionTracker;

    public ExceptionTrackerLifecycle(ExceptionTracker exceptionTracker)
    {
        _exceptionTracker = exceptionTracker;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _exceptionTracker.Start();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _exceptionTracker.Stop();

        return Task.CompletedTask;
    }
}