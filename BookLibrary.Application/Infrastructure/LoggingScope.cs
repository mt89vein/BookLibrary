using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Application.Infrastructure;

/// <summary>
/// Logging scope names.
/// </summary>
[SuppressMessage("Design", "CA1034: Nested types should not be visible")]
public static class LoggingScope
{
    public static class User
    {
        public const string ID = "UserId";
    }

    public static class Abonent
    {
        public const string ID = "AbonentId";
    }

    public static class Book
    {
        public const string ID = "BookId";

        public const string ISBN = "ISBN";

        public const string PUBLICATION_DATE = "PublicationDate";

        public const string RETURN_DATE = "ReturnDate";

        public const string COUNT = "Count";
    }
}