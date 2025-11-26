using BookLibrary.Application.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Sstv.DomainExceptions.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace BookLibrary.Infrastructure.OpenTelemetry;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registeres metric collectors.
    /// </summary>
    /// <param name="services">Service registrator.</param>
    /// <returns>Service registrator.</returns>
    public static IServiceCollection AddTelemetry(this IServiceCollection services)
    {
        services.AddSingleton<IMetricCollector, MetricCollector>();

        services.AddOpenTelemetry()
            .ConfigureResource(builder =>
            {
                builder.AddEnvironmentVariableDetector();
                builder.AddService("BookLibrary");
            }).WithMetrics(b =>
            {
                b.AddMeter("BookLibrary.*");
                b.AddRuntimeInstrumentation();
                b.AddHttpClientInstrumentation();
                b.AddAspNetCoreInstrumentation();
                b.AddDomainExceptionInstrumentation();
                b.AddMeter("Microsoft.AspNetCore.Hosting");
                b.AddMeter("Microsoft.AspNetCore.Http.Connections");
                b.AddMeter("Microsoft.AspNetCore.Diagnostics");
                b.AddMeter("Microsoft.AspNetCore.RateLimiting");

                b.AddPrometheusExporter();
            })
            .WithTracing(b =>
            {
                b.AddAspNetCoreInstrumentation();
                b.AddEntityFrameworkCoreInstrumentation();
                b.AddHttpClientInstrumentation();
                b.SetErrorStatusOnException();
                b.AddConsoleExporter();
            });

        return services;
    }
}

/// <summary>
/// Collects metrics.
/// </summary>
internal sealed class MetricCollector : IMetricCollector, IDisposable
{
    /// <summary>
    /// OpenTelemetry metric collector.
    /// </summary>
    private readonly Meter _meter;

    /// <summary>
    /// Counts created books.
    /// </summary>
    private readonly Counter<long> _bookCreatedCount;

    /// <summary>
    /// Counts books that was borrowed.
    /// </summary>
    private readonly Counter<long> _bookBorrowedCount;

    /// <summary>
    /// Counts returned books.
    /// </summary>
    private readonly Counter<long> _bookReturnedCount;

    /// <summary>
    /// Counts abonent registrations.
    /// </summary>
    private readonly Counter<long> _abonentRegisteredCount;

    /// <summary>
    /// Creates new instance of <see cref="MetricCollector"/>.
    /// </summary>
    /// <param name="meterFactory">Factory of meters.</param>
    public MetricCollector(IMeterFactory meterFactory)
    {
        _meter = meterFactory.Create("BookLibrary", "1.0.0");

        _bookCreatedCount = _meter.CreateCounter<long>(name: "bl.books.created", description: "Counts created books.");
        _bookBorrowedCount = _meter.CreateCounter<long>(name: "bl.books.borrowed", description: "Counts books that was borrowed.");
        _bookReturnedCount = _meter.CreateCounter<long>(name: "bl.books.returned", description: "Counts books that was returned.");
        _abonentRegisteredCount = _meter.CreateCounter<long>(name: "bl.abonents.registered", description: "Counts abonent registrations.");
    }

    /// <summary>
    /// Increment counter of created books.
    /// </summary>
    /// <param name="count">Count of created books.</param>
    public void BooksCreated(int count)
    {
        _bookCreatedCount.Add(count);
    }

    /// <summary>
    /// Increment counter of borrowed books.
    /// </summary>
    public void BookBorrowed()
    {
        _bookBorrowedCount.Add(1);
    }

    /// <summary>
    /// Increment counter of returned books.
    /// </summary>
    public void BookReturned()
    {
        _bookReturnedCount.Add(1);
    }

    /// <summary>
    /// Increment counter of registered abonents.
    /// </summary>
    public void AbonentRegistered()
    {
        _abonentRegisteredCount.Add(1);
    }

    /// <summary>
    /// Cleanup resources.
    /// </summary>
    public void Dispose()
    {
        _meter.Dispose();
    }
}