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

    public override string ToString()
    {
        return Value.ToString();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value.ToString();
    }
}