using BookLibrary.Api.ProblemDetails;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BookLibrary.Api.Extensions;

/// <summary>
/// Extensions methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registeres API controllers.
    /// </summary>
    /// <param name="services">Service registrator.</param>
    /// <returns>Service registrator.</returns>
    public static IServiceCollection AddApiControllers(this IServiceCollection services)
    {
        return services
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
            })
            .Services;
    }
}