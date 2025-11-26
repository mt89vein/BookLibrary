using BookLibrary.Api.Auth;
using BookLibrary.Api.ProblemDetails;
using BookLibrary.Api.Swagger;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BookLibrary.Api;

public static class ApiModule
{
    public static void Register(WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddSwagger();
        builder.AddApiControllers();
        builder.AddMockAuthentication();
    }

    public static void MapEndpoints(WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.MapSwaggerUI(app);
        app.MapControllers().RequireAuthorization();
    }

    /// <summary>
    /// Adds authentication mock.
    /// </summary>
    private static void AddMockAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddAuthentication()
            .AddScheme<MockEmailAuthenticationOptions, MockEmailAuthenticationHandler>(
                MockAuthenticationConstants.SCHEME_NAME,
                MockAuthenticationConstants.SCHEME_NAME,
                null
            );
    }

    /// <summary>
    /// Registeres API controllers.
    /// </summary>
    /// <returns>Service registrator.</returns>
    private static void AddApiControllers(this WebApplicationBuilder builder)
    {
        builder
            .Services
            .AddFluentValidationAutoValidation(o =>
            {
                o.DisableDataAnnotationsValidation = true;
            })
            .AddBookLibraryProblemDetails()
            .AddEndpointsApiExplorer()
            .AddControllers(o =>
            {
                o.OutputFormatters.RemoveType<StringOutputFormatter>();
                o.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
            })
            .AddControllersAsServices()
            .AddValidationProblemDetails()
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                o.AllowInputFormatterExceptionMessages = false;
            });
    }
}