using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace BookLibrary.Api.Swagger;

/// <summary>
/// Extensions methods for <see cref="IServiceCollection"/>.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register swagger.
    /// </summary>
    /// <param name="services">Service registrator.</param>
    /// <returns>Service registrator.</returns>
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigureOptions>();
        services.AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly());

        return services.AddSwaggerGen();
    }

    /// <summary>
    /// Registers swagger UI middleware.
    /// </summary>
    /// <param name="endpoints">Endpoints.</param>
    /// <param name="rootApp">Root application builder.</param>
    /// <param name="configure">Additional configuration.</param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapSwaggerUI(
        this IEndpointRouteBuilder endpoints,
        IApplicationBuilder rootApp,
        Action<SwaggerUIOptions>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(endpoints);
        ArgumentNullException.ThrowIfNull(rootApp);

        var pathBase = rootApp.ApplicationServices.GetRequiredService<IConfiguration>()["PathBase"];
        var options = rootApp.ApplicationServices.GetRequiredService<IOptions<SwaggerGenOptions>>();

        var swaggerDocs = options.Value.SwaggerGeneratorOptions.SwaggerDocs.Keys.ToDictionary(
            documentName => documentName,
            documentName => $"{pathBase}/swagger/{documentName}/swagger.json"
        );

        rootApp.UseSwagger();
        rootApp.UseSwaggerUI(c =>
        {
            c.DocumentTitle = "BookLibrary API";
            c.RoutePrefix = "swagger";

            foreach (var (documentName, url) in swaggerDocs)
            {
                c.SwaggerEndpoint(url, documentName);
            }

            c.EnableDeepLinking();
            c.DisplayRequestDuration();

            configure?.Invoke(c);
        });

        endpoints.MapGet("", context =>
        {
            context.Response.Redirect($"{pathBase}/swagger");

            return Task.CompletedTask;
        });

        return endpoints;
    }
}