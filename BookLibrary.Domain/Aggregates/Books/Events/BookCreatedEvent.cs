using Seedwork;

namespace BookLibrary.Domain.Aggregates.Books;

/// <summary>
/// Event occurs when new book added to library.
/// </summary>
/// <param name="BookId">Book identifier.</param>
public sealed record BookCreatedEvent(BookId BookId) : IDomainEvent;