using BookLibrary.Application.Infrastructure;
using BookLibrary.Domain.Aggregates.Abonents;
using BookLibrary.Domain.Aggregates.Books;
using BookLibrary.Domain.Exceptions;
using BookLibrary.Domain.ValueObjects;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLibrary.Application.Features.Books.BorrowBook;

/// <summary>
/// Borrow book command.
/// </summary>
/// <param name="AbonentId">Aboonent identifier.</param>
/// <param name="BookId">Book identifier.</param>
/// <param name="Isbn">ISBN.</param>
/// <param name="PublicationDate">Book publication date.</param>
/// <param name="ReturnDate">Date when abonent what to return book.</param>
public sealed record BorrowBookCommand(
    Guid AbonentId,
    Guid? BookId,
    string? Isbn,
    DateOnly? PublicationDate,
    DateOnly? ReturnDate
);

/// <summary>
/// UseCase - borrow book.
/// </summary>
public sealed partial class BorrowBookUseCase
{
    private readonly IApplicationContext _ctx;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<BorrowBookUseCase> _logger;

    public BorrowBookUseCase(
        IApplicationContext ctx,
        TimeProvider timeProvider,
        ILogger<BorrowBookUseCase> logger
    )
    {
        _ctx = ctx;
        _logger = logger;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Execute the UseCase.
    /// </summary>
    /// <param name="command">Borrow book command.</param>
    /// <param name="ct">Token for cancel operation.</param>
    /// <exception cref="BookLibraryException">
    /// When there no free book found or abonent can't borrow book.
    /// </exception>
    public async Task<ResultBase> ExecuteAsync(BorrowBookCommand command, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        using var _ = _logger.BeginScope(new Dictionary<string, object?>
        {
            [LoggingScope.Abonent.ID] = command.AbonentId.ToString(),
            [LoggingScope.Book.ID] = command.BookId?.ToString(),
            [LoggingScope.Book.ISBN] = command.Isbn,
            [LoggingScope.Book.PUBLICATION_DATE] = command.PublicationDate?.ToString() ?? "N/A",
            [LoggingScope.Book.RETURN_DATE] = command.ReturnDate?.ToString() ?? "N/A"
        });

        try
        {
            BorrowBookRequestProcessing(command.BookId);

            var bookFetchResult = await FetchBookAsync(command, ct);

            if (bookFetchResult.IsFailed)
            {
                return bookFetchResult.Log(nameof(BorrowBookUseCase));
            }

            var abonement = await GetAbonenementAsync(command, ct);

            var borrowResult = bookFetchResult.Value.Borrow(
                abonement,
                borrowedAt: _timeProvider.GetUtcNow(),
                command.ReturnDate
            );

            if (borrowResult.IsFailed)
            {
                return borrowResult.Log(nameof(BorrowBookUseCase));
            }

            BorrowBookRequestProcessed(bookFetchResult.Value.Id);

            await _ctx.SaveChangesAsync(ct);

            return Result.Ok();
        }
        catch (Exception e) when (e is not BookLibraryException)
        {
            return Result
                .Fail(ErrorCodes.BookBorrowingFailed.ToDomainError(e))
                .Log(nameof(BorrowBookUseCase));
        }
    }

    /// <summary>
    /// Returns book, that abonent wants.
    /// </summary>
    /// <param name="command">Borrow book command.</param>
    /// <param name="ct">Token for cancel operation.</param>
    /// <exception cref="BookLibraryException">
    /// When there no free book found.
    /// </exception>
    private async Task<Result<Book>> FetchBookAsync(BorrowBookCommand command, CancellationToken ct)
    {
        if (command.BookId.HasValue)
        {
            var bookById = await _ctx.Books
                .AsTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == new BookId(command.BookId.Value),
                    ct
                );

            return bookById is not null
                ? Result.Ok(bookById)
                : ErrorCodes.BookNotFound.ToDomainError();
        }

        var book = await _ctx.Books
            .AsTracking()
            .FirstOrDefaultAsync(
                x => x.Isbn == new Isbn(command.Isbn!) &&
                     (command.PublicationDate == null ||
                      x.PublicationDate == new BookPublicationDate(command.PublicationDate.Value)) &&
                     x.BorrowInfo == null,
                ct
            );

        return book is not null
            ? Result.Ok(book)
            : ErrorCodes.ThereNoBookThatCanBeBorrowed.ToDomainError();
    }

    /// <summary>
    /// Returns abonement.
    /// </summary>
    /// <param name="command">Borrow book command.</param>
    /// <param name="ct">Token for cancel operation.</param>
    private async Task<Abonement> GetAbonenementAsync(BorrowBookCommand command, CancellationToken ct)
    {
        var abonentId = new AbonentId(command.AbonentId);

        var booksCount = await _ctx.Books.CountAsync(
            x => x.BorrowInfo != null &&
                 x.BorrowInfo.AbonentId == abonentId,
            ct
        );

        return new Abonement(new AbonentId(command.AbonentId), booksCount);
    }

    [LoggerMessage(
        eventId: 0,
        level: LogLevel.Information,
        message: "Processing borrow book {BookId} request")]
    private partial void BorrowBookRequestProcessing(Guid? bookId);

    [LoggerMessage(
        eventId: 1,
        level: LogLevel.Information,
        message: "Borrow book {BookId} request processed")]
    private partial void BorrowBookRequestProcessed(BookId bookId);
}