using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;
using Unchase.Swashbuckle.AspNetCore.Extensions.Options;

namespace BookLibrary.Api.Swagger;

/// <summary>
/// Configures swagger gen.
/// </summary>
internal sealed class SwaggerConfigureOptions : IConfigureOptions<SwaggerGenOptions>
{
    /// <summary>
    /// Configuration.
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Creates new instance of <see cref="SwaggerConfigureOptions"/>.
    /// </summary>
    /// <param name="configuration">Configuration.</param>
    public SwaggerConfigureOptions(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Invoked to configure SwaggerGenOptions instance.
    /// </summary>
    /// <param name="options">The options instance to configure.</param>
    public void Configure(SwaggerGenOptions options)
    {
        var descriptionFiles = Directory.EnumerateFiles(AppContext.BaseDirectory, "BookLibrary.*.xml").ToArray();

        foreach (var df in descriptionFiles)
        {
            options.IncludeXmlCommentsWithRemarks(df);
        }

        options.SwaggerDoc("v1", new OpenApiInfo { Title = "BookLibrary API", Version = "v1" });

        var pathBase = _configuration["PathBase"] ?? "/";

        options.EnableAnnotations();
        options.ExampleFilters();
        options.AddServer(new OpenApiServer { Description = "BookLibrary API", Url = pathBase });

        options.AddEnumsWithValuesFixFilters(o =>
        {
            o.DescriptionSource = DescriptionSources.DescriptionAttributesThenXmlComments;
            o.IncludeDescriptions = true;

            foreach (var df in descriptionFiles)
            {
                o.IncludeXmlCommentsFrom(df);
            }
        });

        // adds an ability to use same routes with different http methods
        options.ResolveConflictingActions(apiDescriptions =>
        {
            var descriptions = apiDescriptions as ApiDescription[] ?? apiDescriptions.ToArray();
            var first = descriptions.First(); // build relative to the 1st method
            var parameters = descriptions.SelectMany(d => d.ParameterDescriptions).ToList();

            first.ParameterDescriptions.Clear();
            // add parameters and make them optional
            foreach (var parameter in parameters)
            {
                if (first.ParameterDescriptions.All(x => x.Name != parameter.Name))
                {
                    first.ParameterDescriptions.Add(new ApiParameterDescription
                    {
                        ModelMetadata = parameter.ModelMetadata,
                        Name = parameter.Name,
                        ParameterDescriptor = parameter.ParameterDescriptor,
                        Source = parameter.Source,
                        IsRequired = false,
                        DefaultValue = null
                    });
                }
            }

            return first;
        });
    }
}