using BookLibrary.Application.Infrastructure;
using BookLibrary.Domain.Aggregates.Abonents;
using BookLibrary.Domain.Aggregates.Books;
using BookLibrary.Domain.Exceptions;
using BookLibrary.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLibrary.Application.Features.Books.BorrowBook;

/// <summary>
/// Borrow book command.
/// </summary>
/// <param name="AbonentId">Aboonent identifier.</param>
/// <param name="Isbn">ISBN.</param>
/// <param name="PublicationDate">Book publication date.</param>
/// <param name="ReturnDate">Date when abonent what to return book.</param>
public sealed record BorrowBookCommand(
    Guid AbonentId,
    string Isbn,
    DateOnly? PublicationDate,
    DateOnly? ReturnDate
);

/// <summary>
/// UseCase - borrow book.
/// </summary>
public sealed class BorrowBookUseCase
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
    public async Task ExecuteAsync(BorrowBookCommand command, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        using var _ = _logger.BeginScope(new Dictionary<string, object?>
        {
            ["AbonentId"] = command.AbonentId.ToString(),
            ["ISBN"] = command.Isbn,
            ["PublicationDate"] = command.PublicationDate?.ToString() ?? "N/A",
            ["ReturnDate"] = command.ReturnDate?.ToString() ?? "N/A"
        });

        try
        {
            _logger.LogInformation("Processing borrow book request");

            var book = await FetchBookAsync(command, ct);
            var abonement = await GetAbonenementAsync(command, ct);

            book.Borrow(
                abonement,
                borrowedAt: _timeProvider.GetUtcNow(),
                command.ReturnDate
            );

            await _ctx.SaveChangesAsync(ct);

            _logger.LogInformation("Borrow book request processed");
        }
        catch (Exception e) when (e is not BookLibraryException)
        {
            throw ErrorCodes.BookBorrowingFailed.ToException(e);
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
    private async Task<Book> FetchBookAsync(BorrowBookCommand command, CancellationToken ct)
    {
        var book = await _ctx.Books
            .AsTracking()
            .FirstOrDefaultAsync(
            x => x.Isbn == new Isbn(command.Isbn) &&
                 (command.PublicationDate == null || x.PublicationDate == new BookPublicationDate(command.PublicationDate.Value)) &&
                 x.BorrowInfo == null,
            ct
        ) ?? throw ErrorCodes.ThereNoBookThatCanBeBorrowed.ToException();

        return book;
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
}