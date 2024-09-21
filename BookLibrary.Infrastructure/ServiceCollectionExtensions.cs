using BookLibrary.Application.Infrastructure;
using BookLibrary.Infrastructure.Books;
using BookLibrary.Infrastructure.ValueConverters;
using EntityFramework.Exceptions.PostgreSQL;
using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Npgsql;
using Sstv.Outbox;
using Sstv.Outbox.EntityFrameworkCore.Npgsql;

namespace BookLibrary.Infrastructure;

/// <summary>
/// Extensions methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds infrastucture services.
    /// </summary>
    /// <param name="services">Service registrator.</param>
    /// <returns>Service registrator.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IUuidGenerator, UuidGenerator>();
        services.AddSingleton(TimeProvider.System);

        services.AddOutboxItem<ApplicationContext, BookStatChange>(o =>
            {
                o.OutboxItemsLimit = 10000;
                o.WorkerType = EfCoreWorkerTypes.BatchStrictOrdering;
            })
            .WithBatchHandler<BookStatChange, BookStatChangeApplier>();

        return services;
    }

    /// <summary>
    /// Adds entity framework.
    /// </summary>
    /// <param name="services">Service registrator.</param>
    /// <returns>Service registrator.</returns>
    public static IServiceCollection AddEntityFramework(this IServiceCollection services)
    {
        LinqToDBForEFTools.Initialize();

        NpgsqlDataSource? npgsqlDataSource = null; // should be singleton

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

                options.ConfigureWarnings(w => w.Ignore(CoreEventId.FirstWithoutOrderByAndFilterWarning));

                npgsqlDataSource ??= new NpgsqlDataSourceBuilder(config.Value.ConnectionString)
                    .EnableDynamicJson()
                    .BuildMultiHost()
                    .WithTargetSession(TargetSessionAttributes.Primary);

                options
                    .ReplaceService<IValueConverterSelector, ValueObjectsConverterSelectorByConvention>()
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .UseNpgsql(npgsqlDataSource, builder =>
                    {
                        builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        builder.MigrationsAssembly(typeof(ApplicationContext).Assembly.GetName().Name);
                        builder.MigrationsHistoryTable("__EFMigrationHistory", ApplicationContext.DefaultScheme);
                    })
                    .UseSnakeCaseNamingConvention()
                    .UseExceptionProcessor();
            });
    }
}