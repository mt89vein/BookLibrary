using BookLibrary.Domain.Exceptions;
using Seedwork;

namespace BookLibrary.Domain.Aggregates.Books;

/// <summary>
/// Book title.
/// </summary>
public sealed class BookTitle : ValueObject
{
    public const int BOOK_TITLE_MAX_LENGTH = 300;
    public const string BOOK_TITLE_MAX_LENGTH_ERROR_MESSAGE = "Too long book title";

    public string Value { get; }

    public BookTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw ErrorCodes.InvalidBookTitle.ToException();
        }

        if (value.Length > BOOK_TITLE_MAX_LENGTH)
        {
            throw ErrorCodes.InvalidBookTitle.ToException()
                .WithDetailedMessage(BOOK_TITLE_MAX_LENGTH_ERROR_MESSAGE);
        }

        Value = value;
    }

    public override string ToString()
    {
        return Value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}