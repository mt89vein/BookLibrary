using Sstv.DomainExceptions;

namespace BookLibrary.Api.Extensions;

/// <summary>
/// Extensions methods for <see cref="ILogger"/>.
/// </summary>
public static partial class LoggingExtensions
{
    /// <summary>
    /// Logs domain exception.
    /// </summary>
    /// <param name="logger">Logger.</param>
    /// <param name="logLevel">Log level.</param>
    /// <param name="domainException">Domain exception with error code.</param>
    /// <param name="errorCode">Error code.</param>
    /// <param name="message">Error message.</param>
    [LoggerMessage(Message = "{ErrorCode}: {Message}")]
    public static partial void LogDomainException(
        this ILogger logger,
        LogLevel logLevel,
        DomainException domainException,
        string errorCode,
        string message
    );
}