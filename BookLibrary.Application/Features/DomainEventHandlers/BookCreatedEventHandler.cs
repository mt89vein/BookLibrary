using BookLibrary.Application.Infrastructure;
using BookLibrary.Domain.Aggregates.Books;
using Mediator;
using Microsoft.Extensions.Logging;

namespace BookLibrary.Application.Features.DomainEventHandlers;

/// <summary>
/// Handles event, when book created.
/// </summary>
public sealed class BookCreatedEventHandler : INotificationHandler<BookCreatedEvent>
{
    private readonly IMetricCollector _metricCollector;
    private readonly ILogger<BookCreatedEventHandler> _logger;

    public BookCreatedEventHandler(
        IMetricCollector metricCollector,
        ILogger<BookCreatedEventHandler> logger
    )
    {
        _logger = logger;
        _metricCollector = metricCollector;
    }

    public ValueTask Handle(BookCreatedEvent notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        _logger.LogInformation("Book {Title} created. ISBN: {ISBN}, PublicationDate: {PublicationDate}",
            notification.Title,
            notification.Isbn,
            notification.PublicationDate
        );

        _metricCollector.BooksCreated(notification.Count);

        return ValueTask.CompletedTask;
    }
}