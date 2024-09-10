using BookLibrary.Domain.Aggregates.Abonents;
using BookLibrary.Domain.Exceptions;
using Seedwork;

namespace BookLibrary.Domain.Aggregates.Books;

/// <summary>
/// The day, month, year when book was published.
/// </summary>
public sealed class BookPublicationDate : ValueObject
{
    /// <summary>
    /// Publication date.
    /// </summary>
    public DateOnly Value { get; }

    public BookPublicationDate(DateOnly value)
    {
        if (value == default)
        {
            throw ErrorCodes.InvalidBookPublishYear.ToException();
        }

        Value = value;
    }

    public static implicit operator DateOnly(BookPublicationDate publicationDate)
    {
        ArgumentNullException.ThrowIfNull(publicationDate);

        return publicationDate.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value.ToString();
    }
}

/// <summary>
/// Abonent that wants to borrow a book.
/// </summary>
public sealed class Abonement : ValueObject
{
    /// <summary>
    /// Abonent identifier.
    /// </summary>
    public AbonentId AbonentId { get; }

    /// <summary>
    /// Count of books, that abonent already borrowed.
    /// </summary>
    public int BorrowedBooksCount { get; }

    /// <summary>
    /// Creates new instance of <see cref="Abonement"/>.
    /// </summary>
    /// <param name="abonentId">identifier.</param>
    /// <param name="borrowedBooksCount">Count of books, that abonent already borrowed.</param>
    /// <exception cref="BookLibraryException">
    /// When abonent identifier or books count invalid.
    /// </exception>
    public Abonement(AbonentId abonentId, int borrowedBooksCount)
    {
        if (abonentId == default)
        {
            throw ErrorCodes.InvalidBorrowerAbonentId.ToException();
        }

        if (borrowedBooksCount < 0)
        {
            throw ErrorCodes.InvalidBorrowerBooksCount.ToException();
        }

        AbonentId = abonentId;
        BorrowedBooksCount = borrowedBooksCount;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return AbonentId.ToString();
        yield return BorrowedBooksCount.ToString();
    }
}