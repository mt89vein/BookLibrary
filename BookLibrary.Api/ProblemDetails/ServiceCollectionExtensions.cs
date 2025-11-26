using BookLibrary.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sstv.DomainExceptions;
using Sstv.DomainExceptions.DebugViewer;
using Sstv.DomainExceptions.Extensions.DependencyInjection;
using Sstv.DomainExceptions.Extensions.ProblemDetails;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Api.ProblemDetails;

/// <summary>
/// Extensions methods for <see cref="IServiceCollection"/>.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// TraceId problem details extensions name.
    /// </summary>
    private const string TRACE_ID_KEY = "traceId";

    /// <summary>
    /// Default error code.
    /// </summary>
    private static readonly ErrorDescription _defaultErrorCodeDescription =
        ErrorCodes.Default.GetDescription();

    /// <summary>
    /// Default problem details response.
    /// </summary>
    private static readonly ErrorCodeProblemDetails _defaultErrorResponse = new(_defaultErrorCodeDescription);

    /// <summary>
    /// Register problem details.
    /// </summary>
    /// <param name="services">Service registrator.</param>
    /// <returns>Service registrator.</returns>
    public static IServiceCollection AddBookLibraryProblemDetails(this IServiceCollection services)
    {
        return services
            .AddSingleton<IDomainExceptionDebugEnricher, StatusCodeEnricher>()
            .AddProblemDetails(o =>
            {
                o.CustomizeProblemDetails = ctx =>
                {
                    if (ctx.Exception is DomainException de)
                    {
                        ctx.ProblemDetails.Status =
                            ctx.HttpContext.Response.StatusCode =
                                ErrorCodeMapping.MapToStatusCode(de.GetDescription());
                    }
                    else
                    {
                        ctx.ProblemDetails = _defaultErrorResponse;
                        ctx.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        ErrorCodesMeter.Measure(_defaultErrorCodeDescription, null);
                    }

                    if (ctx.Exception is not null)
                    {
                        var addErrorDetails = !ctx.HttpContext.RequestServices.GetRequiredService<IHostEnvironment>().IsProduction();

                        if (addErrorDetails)
                        {
                            ctx.ProblemDetails.Extensions["errorDetails"] = ctx.Exception.ToString();
                        }
                    }

                    if (!ctx.ProblemDetails.Extensions.ContainsKey(TRACE_ID_KEY))
                    {
                        var traceId = Activity.Current?.Id ?? ctx.HttpContext.TraceIdentifier;

                        if (!string.IsNullOrWhiteSpace(traceId))
                        {
                            ctx.ProblemDetails.Extensions[TRACE_ID_KEY] = traceId;
                        }
                    }
                };
            });
    }

    /// <summary>
    /// Configures API behavior to return ProblemDetails on validation.
    /// </summary>
    /// <param name="builder">MVC builder.</param>
    /// <returns>MVC builder.</returns>
    public static IMvcBuilder AddValidationProblemDetails(this IMvcBuilder builder)
    {
        return builder.ConfigureApiBehaviorOptions(o =>
        {
            o.InvalidModelStateResponseFactory = static context =>
            {
                var errorDescription = ErrorCodes.InvalidData.GetDescription();
                ErrorCodesMeter.Measure(errorDescription, null);
                var statusCode = ErrorCodeMapping.MapToStatusCode(errorDescription);

                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Title = errorDescription.Description,
                    Type = !string.IsNullOrWhiteSpace(errorDescription.HelpLink)
                        ? errorDescription.HelpLink
                        : $"https://httpstatuses.io/{statusCode}",
                    Extensions =
                    {
                        ["code"] = errorDescription.ErrorCode,
                        [TRACE_ID_KEY] = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier
                    }
                };

                return new ObjectResult(problemDetails)
                {
                    StatusCode = statusCode
                };
            };
        });
    }
}