using BookLibrary.Application.Features.Abonents.RegisterAbonent;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Application.Features.Abonents;

/// <summary>
/// Extensions methods for <see cref="IServiceCollection"/>.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register abonent features.
    /// </summary>
    /// <param name="services">Service registrator.</param>
    /// <returns>Service registrator.</returns>
    public static IServiceCollection AddAbonentsFeatures(this IServiceCollection services)
    {
        return services.AddScoped<RegisterAbonentUseCase>();
    }
}