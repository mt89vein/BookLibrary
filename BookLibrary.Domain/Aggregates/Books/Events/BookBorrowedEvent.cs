using BookLibrary.Domain.Aggregates.Abonents;
using Seedwork;

namespace BookLibrary.Domain.Aggregates.Books;

/// <summary>
/// Event occurs when book was borrowed by abonent.
/// </summary>
/// <param name="BookId">Book identifier.</param>
/// <param name="AbonentId">Abonent identifier.</param>
/// <param name="BorrowedAt">DateTime when book was borrowed.</param>
/// <param name="ReturnBefore">DateTime when book must be returned.</param>
public sealed record BookBorrowedEvent(
    BookId BookId,
    AbonentId AbonentId,
    DateTimeOffset BorrowedAt,
    DateTimeOffset ReturnBefore
) : IDomainEvent;