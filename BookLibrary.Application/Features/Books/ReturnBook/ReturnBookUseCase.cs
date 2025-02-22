using BookLibrary.Application.Extensions;
using BookLibrary.Application.Infrastructure;
using BookLibrary.Domain.Aggregates.Abonents;
using BookLibrary.Domain.Aggregates.Books;
using BookLibrary.Domain.Exceptions;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLibrary.Application.Features.Books.ReturnBook;

/// <summary>
/// Return book command.
/// </summary>
/// <param name="BookId">Book identifier.</param>
/// <param name="AbonentId">Abonent identifier.</param>
public sealed record ReturnBookCommand(Guid BookId, Guid AbonentId);

/// <summary>
/// UseCase - return book.
/// </summary>
public sealed partial class ReturnBookUseCase
{
    private readonly IApplicationContext _ctx;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<ReturnBookUseCase> _logger;

    public ReturnBookUseCase(
        IApplicationContext ctx,
        TimeProvider timeProvider,
        ILogger<ReturnBookUseCase> logger
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
    /// When can't return book.
    /// </exception>
    public async Task<Result> ExecuteAsync(ReturnBookCommand command, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        using var _ = _logger.BeginScope(new Dictionary<string, object?>
        {
            [LoggingScope.Abonent.ID] = command.AbonentId.ToString(),
            [LoggingScope.Book.ID] = command.BookId.ToString()
        });

        try
        {
            ReturnBorrowedBookRequestProcessing(command.BookId, command.AbonentId);

            var fetchBookResult = await FetchBookByIdAsync(new BookId(command.BookId), ct);

            if (fetchBookResult.IsFailed)
            {
                return fetchBookResult
                    .ToResult()
                    .Log(nameof(ReturnBookUseCase));
            }

            var returnBookResult = fetchBookResult.Value.Return(
                new AbonentId(command.AbonentId),
                returnedAt: _timeProvider.GetUtcNow()
            );

            if (returnBookResult.IsFailed)
            {
                return returnBookResult.Log(nameof(ReturnBookUseCase));
            }

            BorrowedBookReturned(command.BookId, command.AbonentId);

            await _ctx.SaveChangesAsync(ct);

            return Result.Ok();
        }
        catch (Exception e) when (e is not BookLibraryException)
        {
            return Result
                .Fail(ErrorCodes.BookReturningFailed.ToDomainError(e))
                .Log(nameof(ReturnBookUseCase));
        }
    }

    /// <summary>
    /// Fetches book by it's identifier.
    /// </summary>
    /// <param name="bookId">Book identifier.</param>
    /// <param name="ct">Token for cancelling operation.</param>
    /// <returns>Book or failed result.</returns>
    private async ValueTask<Result<Book>> FetchBookByIdAsync(BookId bookId, CancellationToken ct)
    {
        var bookById = await _ctx.Books
            .AsTracking()
            .TagWithFileMember()
            .FirstOrDefaultAsync(x => x.Id == bookId, ct);

        return bookById is not null
            ? Result.Ok(bookById)
            : ErrorCodes.BookNotFound.ToDomainError();
    }

    [LoggerMessage(
        eventId: 0,
        level: LogLevel.Information,
        message: "Returning book {BookId} by {AbonentId} started")]
    private partial void ReturnBorrowedBookRequestProcessing(Guid? bookId, Guid abonentId);

    [LoggerMessage(
        eventId: 1,
        level: LogLevel.Information,
        message: "Successfully returned book {BookId} by {AbonentId}")]
    private partial void BorrowedBookReturned(Guid? bookId, Guid abonentId);
}