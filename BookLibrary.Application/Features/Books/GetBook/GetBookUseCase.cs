using BookLibrary.Application.Infrastructure;
using BookLibrary.Domain.Aggregates.Books;
using BookLibrary.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace BookLibrary.Application.Features.Books.GetBook;

/// <summary>
/// UseCase - get book by id.
/// </summary>
public sealed class GetBookUseCase
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
    /// <param name="bookId">Book identifier.</param>
    /// <param name="ct">Token to cancel operation.</param>
    /// <exception cref="BookLibraryException">
    /// When there are problem with getting book by id.
    /// </exception>
    public async Task<BookDto> ExecuteAsync(Guid bookId, CancellationToken ct = default)
    {
        var bookIdentifier = new BookId(bookId);

        using var _ = _logger.BeginScope(new Dictionary<string, object?>
        {
            ["BookId"] = bookIdentifier.ToString()
        });

        try
        {
            _logger.LogDebug("Getting book by id");

            var book = await _ctx.Books.FindAsync([bookIdentifier], ct)
                       ?? throw ErrorCodes.BookNotFound.ToException();

            _logger.LogDebug("Successfully get book by id");

            return new BookDto(book);
        }
        catch (Exception e) when (e is not BookLibraryException)
        {
            throw ErrorCodes.BookGettingFailed.ToException(e);
        }
    }
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
    public BookDto(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);

        Id = book.Id.Value;
        Title = book.Title.Value;
        Isbn = book.Isbn.Value;
        PublicationDate = book.PublicationDate.Value;
        Authors = book.Authors.Select(x => new AuthorDto(x)).ToArray();
        BorrowInfo = book.BorrowInfo is not null
            ? new BorrowInfoDto(book.BorrowInfo)
            : null;
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
    /// Abonent identifier.
    /// </summary>
    public Guid AbonentId { get; set; }

    /// <summary>
    /// DateTime when book was borrowed.
    /// </summary>
    public DateTimeOffset BorrowedAt { get; set; }

    /// <summary>
    /// Date when book must be returned.
    /// </summary>
    public DateOnly ReturnBefore { get; set; }

    /// <summary>
    /// Creates empty instance.
    /// </summary>
    public BorrowInfoDto() { }

    /// <summary>
    /// Creates new instance of <see cref="BorrowInfoDto"/> from <see cref="BorrowInfo"/>.
    /// </summary>
    /// <param name="borrowInfo">Book borrowing info.</param>
    public BorrowInfoDto(BorrowInfo borrowInfo)
    {
        ArgumentNullException.ThrowIfNull(borrowInfo);

        AbonentId = borrowInfo.AbonentId.Value;
        BorrowedAt = borrowInfo.BorrowedAt;
        ReturnBefore = borrowInfo.ReturnBefore;
    }
}