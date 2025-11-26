using BookLibrary.Application.Extensions;
using BookLibrary.Application.Infrastructure;
using BookLibrary.Domain.Aggregates.Books;
using JetBrains.Annotations;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Sstv.Outbox;

namespace BookLibrary.Infrastructure.Books;

/// <summary>
/// Changes in statistics.
/// </summary>
public sealed class BookStatChange : IOutboxItem
{
    /// <summary>
    /// Change identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// ISBN.
    /// </summary>
    public string Isbn { get; set; } = null!;

    /// <summary>
    /// Year when book was published.
    /// </summary>
    public DateOnly PublicationDate { get; set; }

    /// <summary>
    /// Change in availability of books for borrow.
    /// </summary>
    public int AvailableCount { get; set; }

    /// <summary>
    /// Change in borrowed books count.
    /// </summary>
    public int BorrowedCount { get; set; }
}

/// <summary>
/// Applies changes to book statistics.
/// </summary>
[UsedImplicitly]
internal sealed class BookStatChangeApplier : IOutboxItemBatchHandler<BookStatChange>
{
    /// <summary>
    /// Application context.
    /// </summary>
    private readonly IApplicationContext _ctx;

    /// <summary>
    /// Creates new instance of <see cref="BookStatChangeApplier"/>.
    /// </summary>
    /// <param name="ctx">Application context.</param>
    public BookStatChangeApplier(IApplicationContext ctx)
    {
        _ctx = ctx;
    }

    /// <inheritdoc />
    public async Task<OutboxBatchResult> HandleAsync(
        IReadOnlyCollection<BookStatChange> items,
        OutboxOptions options,
        CancellationToken ct = default
    )
    {
        var books = items
            .GroupBy(x => (x.Isbn, x.PublicationDate))
            .ToDictionary(x => x.Key, v => new BookStatChange
            {
                Isbn = v.Key.Isbn,
                PublicationDate = v.Key.PublicationDate,
                AvailableCount = v.Sum(x => x.AvailableCount),
                BorrowedCount = v.Sum(x => x.BorrowedCount),
            });

        var bookStatPredicate = PredicateBuilder.False<BookStat>();

        foreach (var book in books)
        {
            bookStatPredicate = bookStatPredicate.Or(p => p.Isbn == book.Key.Isbn && p.PublicationDate == book.Key.PublicationDate);
        }

        var bookStats = await _ctx.BookStats
            .TagWithFileMember()
            .AsTracking()
            .Where(bookStatPredicate)
            .ToArrayAsync(ct);

        foreach (var bookStat in bookStats)
        {
            if (!books.Remove((bookStat.Isbn, bookStat.PublicationDate), out var bookStatChange))
            {
                continue;
            }

            bookStat.AvailableCount = NonNegativeSum(NonNegative(bookStat.AvailableCount), bookStatChange.AvailableCount);
            bookStat.BorrowedCount = NonNegativeSum(NonNegative(bookStat.BorrowedCount), bookStatChange.BorrowedCount);
        }

        if (books.Count == 0)
        {
            await _ctx.SaveChangesAsync(ct);

            return OutboxBatchResult.FullyProcessed;
        }

        var bookPredicate = PredicateBuilder.False<Book>();

        foreach (var book in books)
        {
            bookPredicate = bookPredicate.Or(p => p.Isbn == book.Key.Isbn && p.PublicationDate == book.Key.PublicationDate);
        }

        var bookInfo = await _ctx
            .Books
            .TagWithFileMember()
            .Where(bookPredicate)
            .GroupBy(x => new { Isbn = (string)x.Isbn, PublicationDate = (DateOnly)x.PublicationDate })
            .Select(x => x.First())
            .ToArrayAsync(ct);

        var result =
            from bi in bookInfo
            join b in books on
                new { Isbn = bi.Isbn.Value, PublicationDate = bi.PublicationDate.Value } equals
                new { b.Key.Isbn, b.Key.PublicationDate }
            select new BookStat
            {
                Isbn = bi.Isbn.Value,
                PublicationDate = bi.PublicationDate.Value,
                AvailableCount = NonNegative(b.Value.AvailableCount),
                BorrowedCount = NonNegative(b.Value.BorrowedCount),
                Authors = string.Join(",", bi.Authors.Select(x => string.Join(" ", [x.Surname, x.Name, x.Patronymic]))),
                Title = bi.Title,
            };

        _ctx.BookStats.AddRange(result);

        await _ctx.SaveChangesAsync(ct);

        return OutboxBatchResult.FullyProcessed;

        static int NonNegativeSum(int left, int right)
        {
            var sum = left + right;

            return NonNegative(sum);
        }

        static int NonNegative(int value)
        {
            return value < 0 ? 0 : value;
        }
    }
}

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