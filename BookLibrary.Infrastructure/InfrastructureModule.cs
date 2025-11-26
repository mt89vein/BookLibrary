using BookLibrary.Application.Infrastructure;
using BookLibrary.Infrastructure.Books;
using BookLibrary.Infrastructure.EntityFramework;
using BookLibrary.Infrastructure.ErrorHandling;
using BookLibrary.Infrastructure.HealthChecks;
using BookLibrary.Infrastructure.Logging;
using BookLibrary.Infrastructure.OpenTelemetry;
using FluentResults;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Json;
using Sstv.DomainExceptions.Extensions.DependencyInjection;
using Sstv.DomainExceptions.Extensions.SerilogEnricher;
using Sstv.Outbox;
using Sstv.Outbox.EntityFrameworkCore.Npgsql;
using System.Net;

namespace BookLibrary.Infrastructure;

public static class InfrastructureModule
{
    public static void Register(WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddTelemetry();
        builder.Services.AddGracefulShutdown();
        builder.Services.AddHealthChecking();
        builder.Services.AddBookLibraryException();
        builder.Services.AddEntityFramework();
        builder.Services.AddSingleton<IUuidGenerator, UuidGenerator>();
        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddSingleton<ExceptionTracker>();
        builder.Services.AddHostedService<ExceptionTrackerLifecycle>();

        builder.Services.AddOutboxItem<ApplicationContext, BookStatChange>(o =>
            {
                o.OutboxItemsLimit = 1000;
                o.WorkerType = EfCoreWorkerTypes.BatchStrictOrdering;
            })
            .WithBatchHandler<BookStatChange, BookStatChangeApplier>();

        builder.Services.AddSerilog(c =>
        {
            c.ReadFrom.Configuration(builder.Configuration);
            c.WriteTo.Console(new JsonFormatter());
            c.Enrich.FromLogContext();
            c.Enrich.WithProperty("Host", builder.Configuration.GetValue("HOSTNAME", Dns.GetHostName()));
            // add here env etc
            c.Enrich.WithDomainException();
        });
    }

    public static void MapEndpoints(WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseFluentLogger();
        app.UseErrorCodesDebugView();
        app.UseOpenTelemetryPrometheusScrapingEndpoint();
        app.MapHealthCheckingEndpoints();
    }

    private static IApplicationBuilder UseFluentLogger(this IApplicationBuilder app)
    {
        Result.Setup(b =>
            b.Logger = new FluentLogger(app.ApplicationServices.GetRequiredService<ILogger<FluentLogger>>())
        );

        return app;
    }
}