using BookLibrary.Domain.Aggregates.Abonents;
using Mediator;
using Microsoft.Extensions.Logging;

namespace BookLibrary.Application.Features.DomainEventHandlers;

/// <summary>
/// Handles event, when book returned.
/// </summary>
public sealed class BookReturnedEventHandler : INotificationHandler<BookReturnedEvent>
{
    private readonly ILogger<BookReturnedEventHandler> _logger;

    public BookReturnedEventHandler(ILogger<BookReturnedEventHandler> logger)
    {
        _logger = logger;
    }

    public ValueTask Handle(BookReturnedEvent notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        _logger.LogInformation("Book {BookId} returned by abonent {AbonentId}",
            notification.BookId.Value,
            notification.AbonentId.Value
        );

        return ValueTask.CompletedTask;
    }
}