using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BookLibrary.Api.Swagger;

internal sealed class MultipleProducesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attrs = context.MethodInfo.GetCustomAttributes(false)
            .OfType<SwaggerResponseAttribute>()
            .GroupBy(attr => attr.StatusCode)
            .Select(group => new { StatusCode = group.Key, Attributes = group.ToArray() })
            .OrderBy(x => x.StatusCode)
            .ToArray();

        var duplicates = attrs
            .Where(x => x.Attributes.Length > 1)
            .ToArray();

        if (duplicates.Length == 0)
        {
            return;
        }

        foreach (var details in duplicates)
        {
            var response = operation.Responses[details.StatusCode.ToString()];

            foreach (var attr in details.Attributes)
            {
                if (attr is SwaggerErrorCodeResponse errorCodeResponse)
                {
                    var contentType = errorCodeResponse.ContentTypes.Single();
                    if (!response.Content.TryGetValue(contentType, out var mediaType))
                    {
                        response.Content[contentType] = mediaType = new OpenApiMediaType();
                    }

                    mediaType.Schema = context.SchemaGenerator.GenerateSchema(attr.Type, context.SchemaRepository);
                    mediaType.Example = errorCodeResponse.GetExample();
                }
            }
        }
    }
}