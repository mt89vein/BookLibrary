using BookLibrary.Application.Infrastructure;
using JetBrains.Annotations;
using Mediator;

namespace BookLibrary.Infrastructure.Books;

/// <summary>
/// Watcher that tracks changes in book stats.
/// </summary>
[UsedImplicitly]
public sealed class BookStatChangeWatcher :
    INotificationHandler<BookCreated>,
    INotificationHandler<BookBorrowCreated>,
    INotificationHandler<BookBorrowDeleted>
{
    private readonly ApplicationContext _ctx;
    private readonly IUuidGenerator _uuidGenerator;

    public BookStatChangeWatcher(ApplicationContext ctx, IUuidGenerator uuidGenerator)
    {
        _ctx = ctx;
        _uuidGenerator = uuidGenerator;
    }

    public ValueTask Handle(BookCreated notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        _ctx.BookStatChanges.Add(new BookStatChange
        {
            Id = _uuidGenerator.GenerateNew(),
            Isbn = notification.Book.Isbn,
            PublicationDate = notification.Book.PublicationDate,
            AvailableCount = 1,
            BorrowedCount = 0,
        });

        return ValueTask.CompletedTask;
    }

    public ValueTask Handle(BookBorrowCreated notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        _ctx.BookStatChanges.Add(new BookStatChange
        {
            Id = _uuidGenerator.GenerateNew(),
            Isbn = notification.Book.Isbn,
            PublicationDate = notification.Book.PublicationDate,
            AvailableCount = -1,
            BorrowedCount = 1,
        });

        return ValueTask.CompletedTask;
    }

    public ValueTask Handle(BookBorrowDeleted notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        _ctx.BookStatChanges.Add(new BookStatChange
        {
            Id = _uuidGenerator.GenerateNew(),
            Isbn = notification.Book.Isbn,
            PublicationDate = notification.Book.PublicationDate.Value,
            AvailableCount = 1,
            BorrowedCount = -1,
        });

        return ValueTask.CompletedTask;
    }
}