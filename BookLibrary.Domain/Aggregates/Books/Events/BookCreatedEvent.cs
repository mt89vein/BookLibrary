using BookLibrary.Domain.ValueObjects;
using Seedwork;

namespace BookLibrary.Domain.Aggregates.Books;

/// <summary>
/// Event occurs when new book added to library.
/// </summary>
/// <param name="Title">Book title.</param>
/// <param name="Isbn">Book ISBN.</param>
/// <param name="PublicationDate">The day, month, year when book was published.</param>
/// <param name="Count">Count of created books.</param>
public sealed record BookCreatedEvent(
    BookTitle Title,
    Isbn Isbn,
    BookPublicationDate PublicationDate,
    int Count
) : IDomainEvent;