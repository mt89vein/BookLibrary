using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace BookLibrary.Infrastructure.HealthChecks;

/// <summary>
/// Startup filter, that force to wait before shutdown.
/// </summary>
internal sealed class GracefulShutdownStartupFilter : IStartupFilter
{
    /// <summary>
    /// Provides hooks for application lifetime events.
    /// </summary>
    private readonly IHostApplicationLifetime _hostLifetime;

    /// <summary>
    /// How much seconds application should be working until shutdown.
    /// </summary>
    private readonly int _gracefulShutdownSeconds;

    /// <summary>
    /// Creates new instance of <see cref="GracefulShutdownStartupFilter"/>.
    /// </summary>
    /// <param name="hostLifetime">Provides hooks for application lifetime events.</param>
    /// <param name="configuration">Config.</param>
    public GracefulShutdownStartupFilter(
        IHostApplicationLifetime hostLifetime,
        IConfiguration configuration
    )
    {
        _hostLifetime = hostLifetime;
        _gracefulShutdownSeconds = configuration.GetValue<int>(WebHostDefaults.ShutdownTimeoutKey);
    }

    /// <summary>
    /// Extends the provided <paramref name="next" /> and returns an <see cref="System.Action" /> of the same type.
    /// </summary>
    /// <param name="next">The Configure method to extend.</param>
    /// <returns>A modified <see cref="System.Action" />.</returns>
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return builder =>
        {
            if (_gracefulShutdownSeconds > 0)
            {
                _hostLifetime.ApplicationStopping.Register(() =>
                {
                    Thread.Sleep(TimeSpan.FromSeconds(_gracefulShutdownSeconds));
                });
            }

            next(builder);
        };
    }
}