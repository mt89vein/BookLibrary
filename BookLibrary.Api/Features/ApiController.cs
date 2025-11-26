using BookLibrary.Api.ProblemDetails;
using BookLibrary.Domain.Exceptions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sstv.DomainExceptions;
using Sstv.DomainExceptions.Extensions.ProblemDetails;
using System.Diagnostics;
using System.Net.Mime;
using System.Text.Json;

namespace BookLibrary.Api.Features;

/// <summary>
/// Base api controller.
/// </summary>
[ApiController]
public abstract class ApiController : ControllerBase
{
    [NonAction]
    public IActionResult Ok(ResultBase result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsSuccess)
        {
            return Ok();
        }
        const string TRACE_ID_KEY = "traceId";
        const string ERROR_DETAILS_KEY = "errorDetails";

        ErrorCodeProblemDetails pb;

        if (result.Reasons.FirstOrDefault(x => x is ExceptionalError) is ExceptionalError { Exception: DomainException de })
        {
            pb = new ErrorCodeProblemDetails(de)
            {
                Status = ErrorCodeMapping.MapToStatusCode(de.GetDescription())
            };
        }
        else if (result.Reasons.FirstOrDefault(x => x is DomainErrorResult) is DomainErrorResult errorResult)
        {
            var errorDescription = errorResult.ErrorCode.GetDescription();
            pb = new ErrorCodeProblemDetails(errorDescription)
            {
                Status = ErrorCodeMapping.MapToStatusCode(errorDescription)
            };

            foreach (var (key, value) in errorResult.Metadata)
            {
                if (!string.IsNullOrWhiteSpace(key) && value is not null)
                {
                    pb.Extensions[JsonNamingPolicy.CamelCase.ConvertName(key)] = value;
                }
            }
        }
        else
        {
            var errorDescription = ErrorCodes.Default.GetDescription();
            pb = new ErrorCodeProblemDetails(errorDescription)
            {
                Status = ErrorCodeMapping.MapToStatusCode(errorDescription)
            };
        }

        var traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

        if (!string.IsNullOrWhiteSpace(traceId))
        {
            pb.Extensions[TRACE_ID_KEY] = traceId;
        }

        var addErrorDetails = !HttpContext.RequestServices.GetRequiredService<IHostEnvironment>().IsProduction();

        if (addErrorDetails)
        {
            pb.Extensions[ERROR_DETAILS_KEY] = result.ToString();
        }

        Response.Headers.CacheControl = "no-cache, no-store";
        Response.Headers.Expires = "Thu, 01 Jan 1970 00:00:00 GMT";
        Response.Headers.Pragma = "no-cache";

        return new JsonResult(pb)
        {
            ContentType = MediaTypeNames.Application.ProblemJson,
            StatusCode = pb.Status
        };
    }

    [NonAction]
    public IActionResult Ok<T>(Result<T> result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return result.IsSuccess
            ? base.Ok(result.Value)
            : Ok((ResultBase)result);
    }
}