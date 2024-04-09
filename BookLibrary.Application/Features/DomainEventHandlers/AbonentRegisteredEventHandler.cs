using BookLibrary.Application.Infrastructure;
using BookLibrary.Domain.Aggregates.Abonents.Events;
using Mediator;
using Microsoft.Extensions.Logging;

namespace BookLibrary.Application.Features.DomainEventHandlers;

/// <summary>
/// Handles event, when abonent registered.
/// </summary>
public sealed class AbonentRegisteredEventHandler : INotificationHandler<AbonentRegistredEvent>
{
    private readonly IMetricCollector _metricCollector;
    private readonly ILogger<AbonentRegistredEvent> _logger;

    public AbonentRegisteredEventHandler(
        IMetricCollector metricCollector,
        ILogger<AbonentRegistredEvent> logger
    )
    {
        _logger = logger;
        _metricCollector = metricCollector;
    }

    public ValueTask Handle(AbonentRegistredEvent notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        _logger.LogInformation("Abonent {AbonentId} registered", notification.AbonentId);

        _metricCollector.AbonentRegistered();

        return ValueTask.CompletedTask;
    }
}