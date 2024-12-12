using BookLibrary.Application.Infrastructure;
using BookLibrary.Domain.Aggregates.Books;
using BookLibrary.Domain.Exceptions;
using BookLibrary.Domain.ValueObjects;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace BookLibrary.Application.Features.Books.AddNewBook;

/// <summary>
/// Add new book command.
/// </summary>
/// <param name="Isbn">ISBN.</param>
/// <param name="Title">Book title.</param>
/// <param name="PublicationDate">Book publication date.</param>
/// <param name="Authors">Book authors.</param>
/// <param name="Count">Books count.</param>
/// <param name="UserId">Who added books.</param>
public record AddNewBookCommand(
    string Isbn,
    string Title,
    DateOnly PublicationDate,
    IReadOnlyCollection<BookAuthor> Authors,
    int Count,
    Guid UserId
);

public record BookAuthor(string Name, string Surname, string? Patronymic);

/// <summary>
/// UseCase - add new book.
/// </summary>
public sealed partial class AddNewBookUseCase
{
    private readonly IApplicationContext _ctx;
    private readonly IUuidGenerator _uuidGenerator;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<AddNewBookUseCase> _logger;

    public AddNewBookUseCase(
        IApplicationContext ctx,
        IUuidGenerator uuidGenerator,
        TimeProvider timeProvider,
        ILogger<AddNewBookUseCase> logger
    )
    {
        _ctx = ctx;
        _uuidGenerator = uuidGenerator;
        _logger = logger;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Execute the UseCase.
    /// </summary>
    /// <param name="command">Command to add new books.</param>
    /// <param name="ct">Token to cancel operation.</param>
    /// <exception cref="BookLibraryException">
    /// When there are problem with adding books.
    /// </exception>
    public async Task<Result> ExecuteAsync(AddNewBookCommand command, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        using var _ = _logger.BeginScope(new Dictionary<string, object?>
        {
            [LoggingScope.User.ID] = command.UserId.ToString(),
            [LoggingScope.Book.ISBN] = command.Isbn,
            [LoggingScope.Book.PUBLICATION_DATE] = command.PublicationDate.ToString(),
            [LoggingScope.Book.COUNT] = command.Count.ToString(),
        });

        try
        {
            AddingNewBooks(command.Count);

            var books = Enumerable.Range(0, command.Count).Select(_ => new Book(
                new BookId(_uuidGenerator.GenerateNew()),
                new BookTitle(command.Title),
                new Isbn(command.Isbn),
                new BookPublicationDate(command.PublicationDate),
                command.Authors.Select(x => new Author(x.Name, x.Surname, x.Patronymic)).ToArray(),
                createdAt: _timeProvider.GetUtcNow()
            )).ToArray();

            _ctx.Books.AddRange(books);

            NewBooksAdded(books.Length);

            await _ctx.SaveChangesAsync(ct);

            return Result.Ok();
        }
        catch (Exception e) when (e is not BookLibraryException)
        {
            return Result
                .Fail(ErrorCodes.BookAddingFailed.ToDomainError(e))
                .Log(nameof(AddNewBookUseCase));
        }
    }

    [LoggerMessage(
        eventId: 0,
        level: LogLevel.Information,
        message: "Adding new {Count} books")]
    private partial void AddingNewBooks(int count);

    [LoggerMessage(
        eventId: 1,
        level: LogLevel.Information,
        message: "Added {Count} books")]
    private partial void NewBooksAdded(int count);
}