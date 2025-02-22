using BookLibrary.Application.Dto;
using BookLibrary.Application.Extensions;
using BookLibrary.Application.Infrastructure;
using BookLibrary.Domain.Aggregates.Books;
using BookLibrary.Domain.Exceptions;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLibrary.Application.Features.Books.GetPagedBooks;

/// <summary>
/// Query for paged books.
/// </summary>
/// <param name="Page">Page number.</param>
/// <param name="PageSize">Page size.</param>
/// <param name="Isbn">Filter by ISBN.</param>
/// <param name="Title">Filter by title.</param>
public sealed record GetPagedBooksQuery(
    int Page = 1,
    int PageSize = 25,
    string? Title = null,
    string? Isbn = null
);

/// <summary>
/// UseCase - get paged books.
/// </summary>
public sealed partial class GetPagedBooksUseCase
{
    private readonly IApplicationContext _ctx;
    private readonly ILogger<GetPagedBooksUseCase> _logger;

    public GetPagedBooksUseCase(
        IApplicationContext ctx,
        ILogger<GetPagedBooksUseCase> logger
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
    public async Task<Result<BookPageDto>> ExecuteAsync(GetPagedBooksQuery query, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        try
        {
            GettingBooksPage();

            var queryable = _ctx.BookStats.TagWithFileMember();

            if (!string.IsNullOrWhiteSpace(query.Isbn))
            {
                queryable = queryable.Where(x => EF.Functions.Like(x.Isbn, $"%{query.Isbn}%"));
            }

            if (!string.IsNullOrWhiteSpace(query.Title))
            {
                queryable = queryable.Where(x => EF.Functions.ILike(x.Title, $"%{query.Title}%"));
            }

            var pageDto = await queryable
                .Select(b => new
                {
                    b.Title,
                    b.PublicationDate,
                    b.Isbn,
                    b.Authors,
                    Available = b.AvailableCount > 0
                })
                .OrderByDescending(b => b.PublicationDate)
                .ToPagedListAsync(query.Page, query.PageSize, ct);

            GetBooksPageSucceeded();

            return new BookPageDto
            {
                Page = query.Page,
                PageSize = query.PageSize,
                HasNextPage = pageDto.HasNextPage,
                Items = pageDto.Items.Select(x => new BookPageItemDto
                {
                    Title = x.Title,
                    Isbn = x.Isbn,
                    PublicationDate = x.PublicationDate,
                    Available = x.Available,
                    Authors = x.Authors
                }).ToArray()
            };
        }
        catch (Exception e) when (e is not BookLibraryException)
        {
            return Result
                .Fail(ErrorCodes.GetBookPageFailed.ToDomainError(e))
                .Log(nameof(GetPagedBooksUseCase));
        }
    }

    [LoggerMessage(
        eventId: 0,
        level: LogLevel.Information,
        message: "Getting book page")]
    private partial void GettingBooksPage();

    [LoggerMessage(
        eventId: 1,
        level: LogLevel.Information,
        message: "Successfully get book page")]
    private partial void GetBooksPageSucceeded();
}

/// <summary>
/// Book page.
/// </summary>
public sealed class BookPageDto
{
    /// <summary>
    /// Page number.
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Page size.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Page items.
    /// </summary>
    public IReadOnlyCollection<BookPageItemDto> Items { get; set; } = Array.Empty<BookPageItemDto>();

    /// <summary>
    /// Is there next page?
    /// </summary>
    public bool HasNextPage { get; set; }
}

/// <summary>
/// Page item element.
/// </summary>
public sealed class BookPageItemDto
{
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
    public string Authors { get; set; } = null!;

    /// <summary>
    /// Is book available to borrow.
    /// </summary>
    public bool Available { get; set; }
}

/// <summary>
/// Author DTO.
/// </summary>
public sealed class BookPageItemAuthorDto
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
    public BookPageItemAuthorDto() { }

    /// <summary>
    /// Creates new instance of <see cref="BookPageItemAuthorDto"/> from <see cref="Author"/>.
    /// </summary>
    /// <param name="author">Author.</param>
    public BookPageItemAuthorDto(Author author)
    {
        ArgumentNullException.ThrowIfNull(author);

        Name = author.Name;
        Surname = author.Surname;
        Patronymic = author.Patronymic;
    }
}