using BookLibrary.Application.Infrastructure;
using BookLibrary.Infrastructure.ValueConverters;
using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace BookLibrary.Infrastructure.EntityFramework;

internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds entity framework.
    /// </summary>
    /// <param name="services">Service registrator.</param>
    /// <returns>Service registrator.</returns>
    public static IServiceCollection AddEntityFramework(this IServiceCollection services)
    {
        services
            .AddTransient<IConfigureOptions<ApplicationContextSettings>, ConfigureApplicationContextSettings>()
            .AddOptions<ApplicationContextSettings>()
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services
            .AddScoped<IDomainEventDispatcher<ApplicationContext>, DomainEventDispatcher<ApplicationContext>>()
            .AddScoped<IApplicationContext>(sp => sp.GetRequiredService<ApplicationContext>())
            .AddScoped<IDbSets>(sp => sp.GetRequiredService<ApplicationContext>())
            .AddDbContext<ApplicationContext>((sp, options) =>
            {
                var env = sp.GetRequiredService<IHostEnvironment>();
                var config = sp.GetRequiredService<IOptions<ApplicationContextSettings>>();

                if (!env.IsProduction())
                {
                    options
                        .EnableDetailedErrors()
                        .EnableSensitiveDataLogging();
                }

                options
                    .ConfigureWarnings(w => w.Ignore(CoreEventId.FirstWithoutOrderByAndFilterWarning))
                    .ReplaceService<IValueConverterSelector, ValueObjectsConverterSelectorByConvention>()
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .UseSnakeCaseNamingConvention()
                    .UseExceptionProcessor();

                var accessor = sp.GetService<NpgsqlDataSourceAccessor>();

                if (accessor is not null)
                {
                    options.UseNpgsql(accessor.NpgsqlDataSource, b =>
                    {
                        b.ConfigureNpgsql();
                    });
                }
                else
                {
                    options.UseNpgsql(config.Value.ConnectionString, b =>
                    {
                        b.ConfigureNpgsql();
                        b.ConfigureDataSource(ds => ds
                            .EnableDynamicJson()
                            .BuildMultiHost()
                            .WithTargetSession(TargetSessionAttributes.Primary)
                        );
                    });
                }
            });
    }

    private static void ConfigureNpgsql(this NpgsqlDbContextOptionsBuilder b)
    {
        b.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        b.MigrationsAssembly(typeof(ApplicationContext).Assembly.GetName().Name);
        b.MigrationsHistoryTable("__EFMigrationHistory", ApplicationContext.DefaultScheme);
    }
}

public sealed record NpgsqlDataSourceAccessor(NpgsqlDataSource NpgsqlDataSource);