using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BookLibrary.Infrastructure.HealthChecks;

/// <summary>
/// Health checks helpers.
/// </summary>
internal static class HealthCheckHelper
{
    /// <summary>
    /// Cached task - healthy.
    /// </summary>
    public static Task<HealthCheckResult> Healthy { get; } = Task.FromResult(HealthCheckResult.Healthy());

    /// <summary>
    /// Cached task - unhealthy.
    /// </summary>
    public static Task<HealthCheckResult> Unhealthy { get; } = Task.FromResult(HealthCheckResult.Unhealthy());
}