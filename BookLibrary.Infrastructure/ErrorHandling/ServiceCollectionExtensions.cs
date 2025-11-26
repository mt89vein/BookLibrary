using BookLibrary.Domain.Exceptions;
using BookLibrary.Infrastructure.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sstv.DomainExceptions;
using Sstv.DomainExceptions.Extensions.DependencyInjection;
using Sstv.DomainExceptions.Extensions.ProblemDetails;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Infrastructure.ErrorHandling;

/// <summary>
/// Extensions methods for <see cref="IServiceCollection"/>.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registeres <see cref="BookLibraryException"/>.
    /// </summary>
    /// <param name="services">Service registrator.</param>
    /// <returns>Service registrator.</returns>
    public static IServiceCollection AddBookLibraryException(this IServiceCollection services)
    {
        return services.AddDomainExceptions(o =>
        {
            o.WithErrorCodesDescriptionSource(BookLibraryException.ErrorCodesDescriptionSource);
            o.UseDomainExceptionHandler();
            o.ConfigureSettings = (sp, settings) =>
            {
                var exceptionLogger = sp.GetRequiredService<ILogger<DomainException>>();

                settings.OnErrorCreated += (errorDescription, error) =>
                {
                    Activity.Current?.AddTag("error.description", errorDescription.Description);
                    Activity.Current?.AddTag("error.code", errorDescription.ErrorCode);
                    Activity.Current?.AddTag("error.level", Enum.GetName(errorDescription.Level));

                    if (error is DomainErrorResult err)
                    {
                        if (err.InnerException is not null)
                        {
                            Activity.Current?.AddException(err.InnerException);
                        }
                        Activity.Current?.AddTag("error.id", err.ErrorId.ToString());
                    }

                    if (error is DomainException domainException)
                    {
                        Activity.Current?.AddException(domainException);

                        if (domainException.Data.Contains("ErrorId"))
                        {
                            Activity.Current?.AddTag("error.id", domainException.Data["ErrorId"]);
                        }

                        var loglevel = errorDescription.Level switch
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
                        exceptionLogger.LogDomainException(loglevel, domainException, errorDescription.ErrorCode, domainException.Message);
                    }
                };
            };
        });
    }
}