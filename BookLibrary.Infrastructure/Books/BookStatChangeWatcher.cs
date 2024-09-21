using BookLibrary.Application.Infrastructure;
using BookLibrary.Domain.Aggregates.Books;
using JetBrains.Annotations;
using LinqToDB;
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

    public async ValueTask Handle(BookCreated notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        await _ctx.BookStats
            .Merge()
            .Using([new BookStat
            {
                Isbn = notification.Book.Isbn.Value,
                PublicationDate = notification.Book.PublicationDate.Value,
                AvailableCount = 1,
                BorrowedCount = 0,
                Authors = string.Join(",", notification.Book.Authors.Select(x => string.Join(" ", [x.Surname, x.Name, x.Patronymic]))),
                Title = notification.Book.Title,
            }])
            .OnTargetKey()
            .UpdateWhenMatched((target, source) => new BookStat { AvailableCount = target.AvailableCount + source.AvailableCount })
            .InsertWhenNotMatched()
            .MergeAsync(cancellationToken);
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