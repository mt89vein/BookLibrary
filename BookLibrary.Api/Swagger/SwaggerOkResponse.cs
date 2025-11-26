using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;

namespace BookLibrary.Api.Swagger;

public sealed class SwaggerOkResponse<T> : SwaggerResponseAttribute
{
    public SwaggerOkResponse(string? description = null)
        : base(StatusCodes.Status200OK, description, typeof(T), MediaTypeNames.Application.Json)
    {
    }
}