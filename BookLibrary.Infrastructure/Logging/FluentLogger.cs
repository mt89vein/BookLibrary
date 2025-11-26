using BookLibrary.Domain.Exceptions;
using FluentResults;
using Microsoft.Extensions.Logging;
using Sstv.DomainExceptions;

namespace BookLibrary.Infrastructure.Logging;

internal sealed partial class FluentLogger : IResultLogger
{
    private readonly ILogger<FluentLogger> _logger;

    public FluentLogger(ILogger<FluentLogger> logger)
    {
        _logger = logger;
    }

    public void Log(string context, string content, ResultBase result, LogLevel logLevel)
    {
        var logContext = !string.IsNullOrEmpty(context) ? context : nameof(FluentLogger);

        var metadata = result.Reasons
            .Select(x => x.Metadata)
            .SelectMany(x => x)
            .ToLookup(x => x.Key, x => x.Value)
            .ToDictionary(x => x.Key, x => x.First());

        using var scope = _logger.BeginScope(metadata);

        if (result.IsFailed)
        {
            if (result.Reasons.FirstOrDefault(x => x is ExceptionalError) is ExceptionalError { Exception: DomainException de })
            {
                var errorDescription = de.GetDescription();

                LogResult(
                    GetLogLevel(errorDescription),
                    de,
                    operationName: logContext,
                    errorDescription.ErrorCode,
                    errorDescription.Description
                );
            }
            else if (result.Reasons.FirstOrDefault(x => x is DomainErrorResult) is DomainErrorResult errorResult)
            {
                var errorDescription = errorResult.ErrorCode.GetDescription();

                LogResult(
                    GetLogLevel(errorDescription),
                    exception: errorResult.InnerException ?? ExtractException(result),
                    operationName: logContext,
                    errorDescription.ErrorCode,
                    errorDescription.Description
                );
            }
            else
            {
                LogError(exception: ExtractException(result),
                    operationName: logContext,
                    reason: string.Join(", ", result.Reasons.Select(x => x.Message))
                );
            }
        }
        else
        {
            LogSuccess(operationName: logContext);
        }

        return;

        static Exception? ExtractException(ResultBase result)
        {
            return result.Errors.FirstOrDefault(x => x is ExceptionalError) is ExceptionalError err
                ? err.Exception
                : null;
        }

        static LogLevel GetLogLevel(ErrorDescription errorDescription)
        {
            return errorDescription.Level switch
            {
                Level.Undefined => LogLevel.None,
                Level.NotError => LogLevel.Information,
                Level.Low => LogLevel.Warning,
                Level.Medium => LogLevel.Error,
                Level.High => LogLevel.Error,
                Level.Critical => LogLevel.Critical,
                Level.Fatal => LogLevel.Critical,
                _ => LogLevel.Error
            };
        }
    }

    public void Log<TContext>(string content, ResultBase result, LogLevel logLevel)
    {
        Log(typeof(TContext).Name, content, result, logLevel);
    }

    [LoggerMessage(EventId = 0, Message = "Operation {OperationName} failed with {ErrorCode} - {Description}")]
    private partial void LogResult(LogLevel logLevel, Exception? exception, string operationName, string errorCode, string description);

    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "Operation {OperationName} failed with {Reason}")]
    private partial void LogError(Exception? exception, string operationName, string reason);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Operation succeded {OperationName}")]
    private partial void LogSuccess(string operationName);
}