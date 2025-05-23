using BookLibrary.Application.Extensions;
using BookLibrary.Application.Infrastructure;
using BookLibrary.Domain.Aggregates.Abonents;
using BookLibrary.Domain.Aggregates.Books;
using BookLibrary.Domain.Exceptions;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLibrary.Application.Features.Books.GetBorrowedBooks;

/// <summary>
/// Query for getting borrowed books.
/// </summary>
/// <param name="AbonentId">Abonent identifier.</param>
public sealed record GetBorrowedBooksQuery(Guid AbonentId);

/// <summary>
/// UseCase - get borrowed books by abonent.
/// </summary>
public sealed partial class GetBorrowedBooksUseCase
{
    private readonly IApplicationContext _ctx;
    private readonly ILogger<GetBorrowedBooksUseCase> _logger;

    public GetBorrowedBooksUseCase(
        IApplicationContext ctx,
        ILogger<GetBorrowedBooksUseCase> logger
    )
    {
        _ctx = ctx;
        _logger = logger;
    }

    /// <summary>
    /// Execute the UseCase.
    /// </summary>
    /// <param name="query">Borrow book command.</param>
    /// <param name="ct">Token for cancel operation.</param>
    /// <exception cref="BookLibraryException">
    /// When can't return borrowed books.
    /// </exception>
    public async Task<Result<BorrowedBooksDto>> ExecuteAsync(GetBorrowedBooksQuery query, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var abonentId = new AbonentId(query.AbonentId);

        using var _ = _logger.BeginScope(new Dictionary<string, object?>
        {
            [LoggingScope.Abonent.ID] = abonentId.ToString()
        });

        try
        {
            GettingBorrowedBooksByAbonentId(abonentId);

            var books = await FetchBorrowedBooksByAbonentId(abonentId, ct);

            GetBorrowedBooksSucceeded(abonentId);

            return new BorrowedBooksDto(books);
        }
        catch (Exception e) when (e is not BookLibraryException)
        {
            return Result
                .Fail(ErrorCodes.BorrowedBooksGettingFailed.ToDomainError(e))
                .Log(nameof(GetBorrowedBooksUseCase));
        }
    }

    /// <summary>
    /// Returns books borrowed by abonent.
    /// </summary>
    /// <param name="abonentId">Abonent identifier.</param>
    /// <param name="ct">Token for cancel operation.</param>
    /// <returns>Borrowed books.</returns>
    private Task<Book[]> FetchBorrowedBooksByAbonentId(AbonentId abonentId, CancellationToken ct)
    {
        return _ctx.Books
            .TagWithFileMember()
            .Where(x => x.BorrowInfo != null && x.BorrowInfo.AbonentId == abonentId)
            .ToArrayAsync(cancellationToken: ct);
    }

    [LoggerMessage(
        eventId: 0,
        level: LogLevel.Information,
        message: "Getting borrowed books by {AbonentId}")]
    private partial void GettingBorrowedBooksByAbonentId(AbonentId abonentId);

    [LoggerMessage(
        eventId: 1,
        level: LogLevel.Information,
        message: "Successfully get borrowed books by {AbonentId}")]
    private partial void GetBorrowedBooksSucceeded(AbonentId abonentId);
}

/// <summary>
/// Borrowed by abonent books.
/// </summary>
public sealed class BorrowedBooksDto
{
    /// <summary>
    /// Borrowed books.
    /// </summary>
    public IReadOnlyCollection<BorrowedBookDto> Books { get; set; }

    public BorrowedBooksDto()
    {
        Books = Array.Empty<BorrowedBookDto>();
    }

    public BorrowedBooksDto(IReadOnlyCollection<Book> books)
    {
        Books = books.Select(b => new BorrowedBookDto(b)).ToArray();
    }
}

/// <summary>
/// Borrowed by abonent book.
/// </summary>
public sealed class BorrowedBookDto
{
    /// <summary>
    /// Book identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Book title.
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Book ISBN.
    /// </summary>
    public string Isbn { get; set; } = null!;

    /// <summary>
    /// Book publication date.
    /// </summary>
    public DateOnly PublicationDate { get; set; }

    /// <summary>
    /// DateTime when book was borrowed.
    /// </summary>
    public DateTimeOffset BorrowedAt { get; set; }

    /// <summary>
    /// Date when book must be return to library.
    /// </summary>
    public DateOnly ReturnBefore { get; set; }

    public BorrowedBookDto()
    {
    }

    public BorrowedBookDto(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);
        ArgumentNullException.ThrowIfNull(book.BorrowInfo);

        Id = book.Id.Value;
        Title = book.Title.Value;
        Isbn = book.Isbn.Value;
        PublicationDate = book.PublicationDate.Value;
        BorrowedAt = book.BorrowInfo.BorrowedAt;
        ReturnBefore = book.BorrowInfo.ReturnBefore;
    }
}