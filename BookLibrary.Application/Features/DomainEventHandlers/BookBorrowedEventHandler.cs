using BookLibrary.Application.Infrastructure;
using BookLibrary.Domain.Aggregates.Books;
using Mediator;
using Microsoft.Extensions.Logging;

namespace BookLibrary.Application.Features.DomainEventHandlers;

/// <summary>
/// Handles event, when book borrowed.
/// </summary>
public sealed class BorrowedBookEventHandler : INotificationHandler<BookBorrowedEvent>
{
    private readonly IMetricCollector _metricCollector;
    private readonly ILogger<BorrowedBookEventHandler> _logger;

    public BorrowedBookEventHandler(
        IMetricCollector metricCollector,
        ILogger<BorrowedBookEventHandler> logger
    )
    {
        _logger = logger;
        _metricCollector = metricCollector;
    }

    public ValueTask Handle(BookBorrowedEvent notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        _logger.LogInformation("Book {BookId} borrowed by abonent {AbonentId} until {ReturnBefore}",
            notification.BookId.Value,
            notification.AbonentId.Value,
            notification.ReturnBefore
        );

        _metricCollector.BookBorrowed();

        return ValueTask.CompletedTask;
    }
}