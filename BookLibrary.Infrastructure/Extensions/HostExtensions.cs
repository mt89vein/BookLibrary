using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Infrastructure.Extensions;

/// <summary>
/// Extensions for <see cref="IHost"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public static class HostExtensions
{
    /// <summary>
    /// Apply db migrations.
    /// </summary>
    /// <param name="host">Host.</param>
    /// <param name="ct">Token for cancel operation.</param>
    public static async Task MigrateAsync(this IHost host, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(host);

        using var scope = host.Services.CreateScope();
        await using var ctx = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

        await ctx.Database.MigrateAsync(ct);
    }
}