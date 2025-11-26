using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Infrastructure.HealthChecks;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registeres graceful shutdown.
    /// </summary>
    /// <param name="services">Service registrator.</param>
    /// <returns>Service registrator.</returns>
    public static IServiceCollection AddGracefulShutdown(this IServiceCollection services)
    {
        return services.AddSingleton<IStartupFilter, GracefulShutdownStartupFilter>();
    }

    /// <summary>
    /// Registeres health check services.
    /// </summary>
    /// <param name="services">Service registrator.</param>
    /// <returns>Service registrator.</returns>
    public static IServiceCollection AddHealthChecking(this IServiceCollection services)
    {
        services.AddSingleton<ReadinessProbe>();
        services.AddSingleton<LivenessProbe>();
        services.AddSingleton<StartupProbe>();

        return services
            .AddHealthChecks()
            .AddCheck<ReadinessProbe>(nameof(ReadinessProbe), tags: ReadinessProbe.Tags)
            .AddCheck<LivenessProbe>(nameof(LivenessProbe), tags: LivenessProbe.Tags)
            .AddCheck<StartupProbe>(nameof(StartupProbe), tags: StartupProbe.Tags)
            .Services;
    }

    /// <summary>
    /// Configures endpoints for health checking.
    /// </summary>
    /// <param name="builder">Endpoints builder.</param>
    /// <returns>Endpoints builder.</returns>
    public static IEndpointRouteBuilder MapHealthCheckingEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapHealthChecks("/healthy", new HealthCheckOptions
        {
            Predicate = hc => hc.Tags.IsSupersetOf(ReadinessProbe.Tags)
        });

        builder.MapHealthChecks("/ready", new HealthCheckOptions
        {
            Predicate = hc => hc.Tags.IsSupersetOf(LivenessProbe.Tags)
        });

        builder.MapHealthChecks("/started", new HealthCheckOptions
        {
            Predicate = hc => hc.Tags.IsSupersetOf(StartupProbe.Tags)
        });

        return builder;
    }
}

/// <summary>
/// Healthy, when app is started.
/// </summary>
internal sealed class LivenessProbe : IHealthCheck
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    public static IReadOnlyCollection<string> Tags { get; } = [nameof(LivenessProbe)];

    public LivenessProbe(IHostApplicationLifetime hostApplicationLifetime)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        return _hostApplicationLifetime.ApplicationStarted.IsCancellationRequested
            ? HealthCheckHelper.Healthy
            : HealthCheckHelper.Unhealthy;
    }
}

/// <summary>
/// Healty when app is started and not stopping.
/// </summary>
internal sealed class ReadinessProbe : IHealthCheck
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public static IReadOnlyCollection<string> Tags { get; } = [nameof(ReadinessProbe)];

    public ReadinessProbe(IHostApplicationLifetime hostApplicationLifetime)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        return _hostApplicationLifetime.ApplicationStarted.IsCancellationRequested &&
               !_hostApplicationLifetime.ApplicationStopping.IsCancellationRequested
            ? HealthCheckHelper.Healthy
            : HealthCheckHelper.Unhealthy;
    }
}

/// <summary>
/// Healty when app is started.
/// </summary>
internal sealed class StartupProbe : IHealthCheck
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public static IReadOnlyCollection<string> Tags { get; } = [nameof(StartupProbe)];

    public StartupProbe(IHostApplicationLifetime hostApplicationLifetime)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        return _hostApplicationLifetime.ApplicationStarted.IsCancellationRequested
            ? HealthCheckHelper.Healthy
            : HealthCheckHelper.Unhealthy;
    }
}