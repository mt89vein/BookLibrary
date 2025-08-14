using BookLibrary.Api.ProblemDetails;
using BookLibrary.Domain.Exceptions;
using Microsoft.OpenApi.Any;
using Sstv.DomainExceptions.Extensions.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;

namespace BookLibrary.Api.Swagger;

/// <summary>
/// Error code response.
/// </summary>
public sealed class SwaggerErrorCodeResponse : SwaggerResponseAttribute
{
    public ErrorCodes ErrorCode { get; }

    public SwaggerErrorCodeResponse(ErrorCodes errorCode, string? description = null)
        : base(
            ErrorCodeMapping.MapToStatusCode(errorCode.GetDescription()),
            description,
            typeof(ErrorCodeProblemDetails),
            contentTypes: [MediaTypeNames.Application.ProblemJson]
        )
    {
        ErrorCode = errorCode;
    }

    public IOpenApiAny GetExample()
    {
        var error = ErrorCode.GetDescription();

        return new OpenApiObject
        {
            ["code"] = new OpenApiString(error.ErrorCode),
            ["type"] = new OpenApiString(error.HelpLink),
            ["title"] = new OpenApiString(error.Description),
            ["status"] = new OpenApiInteger(StatusCode),
            ["criticalityLevel"] = new OpenApiString(Enum.GetName(error.Level))
        };
    }
}