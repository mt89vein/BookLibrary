using BookLibrary.Application.Features.Abonents;
using BookLibrary.Application.Features.Books;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Application;

/// <summary>
/// Extensions methods for <see cref="IServiceCollection"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register application.
    /// </summary>
    /// <param name="services">Service registrator.</param>
    /// <returns>Service registrator.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services
            .AddAbonentsFeatures()
            .AddBooksFeatures();
    }
}