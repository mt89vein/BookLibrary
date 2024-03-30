using BookLibrary.Application.Infrastructure;
using BookLibrary.Domain.Aggregates.Books;
using BookLibrary.Domain.Exceptions;
using BookLibrary.Domain.ValueObjects;
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
public sealed class AddNewBookUseCase
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
    public async Task ExecuteAsync(AddNewBookCommand command, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        using var _ = _logger.BeginScope(new Dictionary<string, object?>
        {
            ["UserId"] = command.UserId.ToString(),
            ["ISBN"] = command.Isbn,
            ["PublicationDate"] = command.PublicationDate.ToString(),
            ["Count"] = command.Count.ToString(),
        });

        try
        {
            _logger.LogInformation("Adding {Count} books", command.Count);

            var books = Enumerable.Range(0, command.Count).Select(_ => new Book(
                new BookId(_uuidGenerator.GenerateNew()),
                new BookTitle(command.Title),
                new Isbn(command.Isbn),
                new BookPublicationDate(command.PublicationDate),
                command.Authors.Select(x => new Author(x.Name, x.Surname, x.Patronymic)).ToArray(),
                createdAt: _timeProvider.GetUtcNow()
            )).ToArray();

            foreach (var book in books.Skip(1))
            {
                book.ClearDomainEvents();
            }

            _ctx.Books.AddRange(books);

            await _ctx.SaveChangesAsync(ct);

            _logger.LogInformation("Added {Count} books", command.Count);
        }
        catch (Exception e) when (e is not BookLibraryException)
        {
            throw ErrorCodes.BookAddingFailed.ToException(e);
        }
    }
}