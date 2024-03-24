using BookLibrary.Domain.Aggregates.Books;
using Seedwork;

namespace BookLibrary.Domain.Aggregates.Abonents;

/// <summary>
/// Event occurs when book was borrowed by abonent.
/// </summary>
/// <param name="AbonentId">Abonent identifier.</param>
/// <param name="BookId">Book identifier.</param>
/// <param name="ReturnedAt">DateTime when book was returned.</param>
public sealed record BookReturnedEvent(
    BookId BookId,
    AbonentId AbonentId,
    DateTimeOffset ReturnedAt
) : IDomainEvent;