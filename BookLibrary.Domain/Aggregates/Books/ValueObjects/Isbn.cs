using BookLibrary.Domain.Exceptions;
using Seedwork;
using System.Text.RegularExpressions;

namespace BookLibrary.Domain.ValueObjects;

/// <summary>
/// International standard book number <see href="https://en.wikipedia.org/wiki/ISBN"/>.
/// </summary>
public sealed partial class Isbn : ValueObject
{
    private static readonly Regex _regex = GetIsbnRegex();

    public string Value { get; }

    public Isbn(string isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn))
        {
            throw ErrorCodes.InvalidIsbn
                .ToException()
                .WithDetailedMessage("ISBN cannot be null or empty.");
        }

        if (!_regex.IsMatch(isbn))
        {
            throw ErrorCodes.InvalidIsbn
                .ToException()
                .WithDetailedMessage("ISBN is not valid")
                .WithAdditionalData("ISBN", isbn);
        }

        Value = isbn;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(Isbn isbn)
    {
        ArgumentNullException.ThrowIfNull(isbn);

        return isbn.Value;
    }

    [GeneratedRegex(@"^(\d{9}(?:\d|X)|(\d{12}(?:\d|X)))$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex GetIsbnRegex();
}


