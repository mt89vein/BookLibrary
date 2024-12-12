using BookLibrary.Application.Infrastructure;
using BookLibrary.Domain.Aggregates.Abonents;
using BookLibrary.Domain.Aggregates.Books;
using BookLibrary.Domain.Exceptions;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace BookLibrary.Application.Features.Books.GetBook;

/// <summary>
/// Query for getting book by id.
/// </summary>
/// <param name="BookId">Book identifier.</param>
/// <param name="AbonentId">Abonent identifier.</param>
public sealed record GetBookByIdQuery(Guid BookId, Guid AbonentId);

/// <summary>
/// UseCase - get book by id.
/// </summary>
public sealed partial class GetBookUseCase
{
    private readonly IApplicationContext _ctx;
    private readonly ILogger<GetBookUseCase> _logger;

    public GetBookUseCase(
        IApplicationContext ctx,
        ILogger<GetBookUseCase> logger
    )
    {
        _ctx = ctx;
        _logger = logger;
    }

    /// <summary>
    /// Execute the UseCase.
    /// </summary>
    /// <param name="query">Query.</param>
    /// <param name="ct">Token to cancel operation.</param>
    /// <exception cref="BookLibraryException">
    /// When there are problem with getting book by id.
    /// </exception>
    public async Task<Result<BookDto>> ExecuteAsync(GetBookByIdQuery query, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var bookId = new BookId(query.BookId);

        using var _ = _logger.BeginScope(new Dictionary<string, object?>
        {
            [LoggingScope.Book.ID] = bookId.ToString()
        });

        try
        {
            GettingBookById(bookId);

            var fetchBookResult = await FetchBookByIdAsync(bookId, ct);

            if (fetchBookResult.IsFailed)
            {
                return fetchBookResult
                    .ToResult()
                    .Log(nameof(GetBookUseCase));
            }

            GetBookSucceed(bookId);

            return new BookDto(fetchBookResult.Value, new AbonentId(query.AbonentId));
        }
        catch (Exception e) when (e is not BookLibraryException)
        {
            return Result
                .Fail(ErrorCodes.BookGettingFailed.ToDomainError(e))
                .Log(nameof(GetBookUseCase));
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
        var bookById = await _ctx.Books.FindAsync([bookId], ct);

        return bookById is not null
            ? Result.Ok(bookById)
            : ErrorCodes.BookNotFound.ToDomainError();
    }

    [LoggerMessage(
        eventId: 0,
        level: LogLevel.Information,
        message: "Getting book {BookId}")]
    private partial void GettingBookById(BookId bookId);

    [LoggerMessage(
        eventId: 1,
        level: LogLevel.Information,
        message: "Successfully get book by {BookId}")]
    private partial void GetBookSucceed(BookId bookId);
}

/// <summary>
/// Book DTO.
/// </summary>
public sealed class BookDto
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
    /// ISBN.
    /// </summary>
    public string Isbn { get; set; } = null!;

    /// <summary>
    /// Year when book was published.
    /// </summary>
    public DateOnly PublicationDate { get; set; }

    /// <summary>
    /// Book authors.
    /// </summary>
    public IReadOnlyCollection<AuthorDto> Authors { get; set; } = Array.Empty<AuthorDto>();

    /// <summary>
    /// Borrowing info.
    /// </summary>
    public BorrowInfoDto? BorrowInfo { get; set; }

    /// <summary>
    /// Creates empty instance.
    /// </summary>
    public BookDto() { }

    /// <summary>
    /// Creates new instance of <see cref="BookDto"/> from <see cref="Book"/>.
    /// </summary>
    /// <param name="book">Book.</param>
    /// <param name="abonentId">AbonentId, that requests book by id.</param>
    public BookDto(Book book, AbonentId abonentId)
    {
        ArgumentNullException.ThrowIfNull(book);

        Id = book.Id.Value;
        Title = book.Title.Value;
        Isbn = book.Isbn.Value;
        PublicationDate = book.PublicationDate.Value;
        Authors = book.Authors.Select(x => new AuthorDto(x)).ToArray();
        BorrowInfo = new BorrowInfoDto(book.BorrowInfo, abonentId);
    }
}

/// <summary>
/// Author DTO.
/// </summary>
public sealed class AuthorDto
{
    /// <summary>
    /// First name.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Last name.
    /// </summary>
    public string Surname { get; set; } = null!;

    /// <summary>
    /// Middle name.
    /// </summary>
    public string? Patronymic { get; set; }

    /// <summary>
    /// Creates empty instance.
    /// </summary>
    public AuthorDto() { }

    /// <summary>
    /// Creates new instance of <see cref="AuthorDto"/> from <see cref="Author"/>.
    /// </summary>
    /// <param name="author">Author.</param>
    public AuthorDto(Author author)
    {
        ArgumentNullException.ThrowIfNull(author);

        Name = author.Name;
        Surname = author.Surname;
        Patronymic = author.Patronymic;
    }
}

/// <summary>
/// Borrow info DTO.
/// </summary>
public sealed class BorrowInfoDto
{
    /// <summary>
    /// Book status.
    /// </summary>
    public string Status { get; set; } = null!;

    /// <summary>
    /// DateTime when book was borrowed.
    /// </summary>
    public DateTimeOffset? BorrowedAt { get; set; }

    /// <summary>
    /// Date when book must be returned.
    /// </summary>
    public DateOnly? ReturnBefore { get; set; }

    /// <summary>
    /// Creates empty instance.
    /// </summary>
    public BorrowInfoDto() { }

    /// <summary>
    /// Creates new instance of <see cref="BorrowInfoDto"/> from <see cref="BorrowInfo"/>.
    /// </summary>
    /// <param name="borrowInfo">Book borrowing info.</param>
    /// <param name="abonentId">AbonentId, that requests book by id.</param>
    public BorrowInfoDto(BorrowInfo? borrowInfo, AbonentId abonentId)
    {
        if (borrowInfo is not null)
        {
            Status = borrowInfo.AbonentId == abonentId ? "Borrowed by you" : "Borrowed";
        }
        else
        {
            Status = "Available";
        }

        BorrowedAt = borrowInfo?.BorrowedAt;
        ReturnBefore = borrowInfo?.ReturnBefore;
    }
}