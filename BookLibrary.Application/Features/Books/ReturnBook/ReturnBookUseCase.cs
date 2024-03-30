using BookLibrary.Application.Features.Books.BorrowBook;
using BookLibrary.Application.Infrastructure;
using BookLibrary.Domain.Aggregates.Abonents;
using BookLibrary.Domain.Aggregates.Books;
using BookLibrary.Domain.Exceptions;
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
public sealed class ReturnBookUseCase
{
    private readonly IApplicationContext _ctx;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<BorrowBookUseCase> _logger;

    public ReturnBookUseCase(
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
    /// When can't return book.
    /// </exception>
    public async Task ExecuteAsync(ReturnBookCommand command, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        using var _ = _logger.BeginScope(new Dictionary<string, object?>
        {
            ["AbonentId"] = command.AbonentId.ToString(),
            ["BookId"] = command.BookId.ToString()
        });

        try
        {
            _logger.LogInformation("Processing return book request");

            var book = await FetchBookAsync(command, ct);

            book.Return(
                new AbonentId(command.AbonentId),
                returnedAt: _timeProvider.GetUtcNow()
            );

            await _ctx.SaveChangesAsync(ct);

            _logger.LogInformation("Return book request processed");
        }
        catch (Exception e) when (e is not BookLibraryException)
        {
            throw ErrorCodes.BookReturningFailed.ToException(e);
        }
    }

    /// <summary>
    /// Returns book, that abonent wants to return.
    /// </summary>
    /// <param name="command">Borrow book command.</param>
    /// <param name="ct">Token for cancel operation.</param>
    /// <exception cref="BookLibraryException">
    /// When there no book with such id.
    /// </exception>
    private async Task<Book> FetchBookAsync(ReturnBookCommand command, CancellationToken ct)
    {
        var book = await _ctx.Books
                       .AsQueryable()
                       .AsTracking()
                       .FirstOrDefaultAsync(x => x.Id == new BookId(command.BookId), ct) ??
                   throw ErrorCodes.ThereNoBookThatCanBeBorrowed.ToException();

        return book;
    }
}