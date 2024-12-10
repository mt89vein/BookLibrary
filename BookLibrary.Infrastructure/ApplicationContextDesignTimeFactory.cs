using BookLibrary.Infrastructure.ValueConverters;
using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql;

namespace BookLibrary.Infrastructure;

internal sealed class ApplicationContextDesignTimeFactory : IDesignTimeDbContextFactory<ApplicationContext>
{
    public ApplicationContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();

        var npgsqlDataSource = new NpgsqlDataSourceBuilder("Server=localhost")
            .EnableDynamicJson()
            .BuildMultiHost()
            .WithTargetSession(TargetSessionAttributes.Primary);

        optionsBuilder
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

        return new ApplicationContext(optionsBuilder.Options, new FakeDispatcher(), null!);
    }

    private sealed class FakeDispatcher : IDomainEventDispatcher<ApplicationContext>
    {
        public void SetDbContext(ApplicationContext dbContext)
        {
        }

        public Task DispatchDomainEventsAsync(CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }
    }
}