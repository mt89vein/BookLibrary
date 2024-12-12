using FluentResults;
using Sstv.DomainExceptions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace BookLibrary.Domain.Exceptions;

/// <summary>
/// Error codes.
/// </summary>
[ExceptionConfig(ClassName = "BookLibraryException")]
[ErrorDescription(Prefix = "BL", Level = Level.Medium)]
public enum ErrorCodes
{
    [ErrorDescription(Description = "Unknown error", Level = Level.Critical)]
    Default = 0,

    [ErrorDescription(Description = "Invalid data", Level = Level.Critical)]
    InvalidData = 1,

    [ErrorDescription(Description = "Abonent not found", Level = Level.NotError)]
    AbonentNotFound = 2,

    [ErrorDescription(Description = "Book not found", Level = Level.NotError)]
    BookNotFound = 3,

    [ErrorDescription(Description = "Invalid book identifier", Level = Level.Medium)]
    InvalidBookId = 4,

    [ErrorDescription(Description = "Invalid abonent identifier", Level = Level.Medium)]
    InvalidAbonentId = 5,

    [ErrorDescription(Description = "Cannot return not borrowed book", Level = Level.NotError)]
    CannotReturnNotBorrowedBook = 6,

    [ErrorDescription(Description = "Invalid book name", Level = Level.Medium)]
    InvalidBookTitle = 7,

    [ErrorDescription(Description = "Invalid abonent name", Level = Level.Medium)]
    InvalidAbonentName = 8,

    [ErrorDescription(Description = "Invalid abonent surname name", Level = Level.Medium)]
    InvalidAbonentSurname = 9,

    [ErrorDescription(Description = "Invalid ISBN", Level = Level.Medium)]
    InvalidIsbn = 10,

    [ErrorDescription(Description = "Invalid email", Level = Level.Medium)]
    InvalidEmail = 11,

    [ErrorDescription(Description = "Invalid abonent identifier for borrow book", Level = Level.Medium)]
    InvalidBorrowerAbonentId = 12,

    [ErrorDescription(Description = "Book already borrowed", Level = Level.Low)]
    BookAlreadyBorrowed = 13,

    [ErrorDescription(Description = "Invalid book publish year", Level = Level.Medium)]
    InvalidBookPublishYear = 14,

    [ErrorDescription(Description = "Book not borrowed by anyone", Level = Level.NotError)]
    BookNotBorrowedByAnyone = 15,

    [ErrorDescription(Description = "Invalid book author name", Level = Level.Medium)]
    InvalidBookAuthorName = 16,

    [ErrorDescription(Description = "Invalid book author surname", Level = Level.Medium)]
    InvalidBookAuthorSurname = 17,

    [ErrorDescription(Description = "Book must have an author", Level = Level.Low)]
    BookMustHaveAnAuthors = 18,

    [ErrorDescription(Description = "Book adding failed", Level = Level.Critical)]
    BookAddingFailed = 19,

    [ErrorDescription(Description = "Book getting failed", Level = Level.Critical)]
    BookGettingFailed = 20,

    [ErrorDescription(Description = "Abonent registering failed", Level = Level.Critical)]
    AbonentRegisteringFailed = 21,

    [ErrorDescription(Description = "Email already exists", Level = Level.NotError)]
    EmailAlreadyExists = 22,

    [ErrorDescription(Description = "User undefined", Level = Level.Critical)]
    UserUndefined = 23,

    [ErrorDescription(Description = "There no book that can be borrowed", Level = Level.NotError)]
    ThereNoBookThatCanBeBorrowed = 24,

    [ErrorDescription(Description = "Invalid borrowed books count", Level = Level.High)]
    InvalidBorrowerBooksCount = 25,

    [ErrorDescription(Description = "Book borrowing failed", Level = Level.Critical)]
    BookBorrowingFailed = 26,

    [ErrorDescription(Description = "Too many books borrowed already", Level = Level.NotError)]
    TooManyBooksBorrowedAlready = 27,

    [ErrorDescription(Description = "Invalid abonent identifier", Level = Level.Medium)]
    InvalidBookReturnAbonentId = 28,

    [ErrorDescription(Description = "Book can't be returned if you not borrowed it", Level = Level.NotError)]
    BookNotBorrowedByAbonent = 29,

    [ErrorDescription(Description = "Book returning failed", Level = Level.Critical)]
    BookReturningFailed = 30,

    [ErrorDescription(Description = "Borrowed books getting failed", Level = Level.Critical)]
    BorrowedBooksGettingFailed = 31,

    [ErrorDescription(Description = "This book not found or not borrowed by abonent", Level = Level.NotError)]
    BookNotFoundOrNotBorrowedByAbonent = 32,

    [ErrorDescription(Description = "Books getting failed", Level = Level.Critical)]
    GetBookPageFailed = 33,

    [ErrorDescription(Description = "Book return date must be later than borrowing time", Level = Level.Low)]
    InvalidBookBorrowingPeriod = 34,
}

/// <summary>
/// Domain error.
/// </summary>
public sealed class DomainErrorResult : Error
{
    /// <summary>
    /// Error code.
    /// </summary>
    public ErrorCodes ErrorCode { get; }

    /// <summary>
    /// Unique error identifier.
    /// </summary>
    public Guid ErrorId { get; }

    /// <summary>
    /// Inner exception.
    /// </summary>
    public Exception? InnerException { get; }

    /// <summary>
    /// Creates new instance of <see cref="DomainErrorResult"/>.
    /// </summary>
    /// <param name="errorCode">Error code.</param>
    /// <param name="innerException">Exception if any.</param>
    public DomainErrorResult(ErrorCodes errorCode, Exception? innerException = null)
    {
        ErrorId = Guid.CreateVersion7();
        ErrorCode = errorCode;
        InnerException = innerException;

        var errorDescription = errorCode.GetDescription();
        Message = $"{errorDescription.ErrorCode}: {errorDescription.Description}";
        Metadata.Add("Code", errorDescription.ErrorCode);
        Metadata.Add("CriticalityLevel", Enum.GetName(errorDescription.Level));
        Metadata.Add("ErrorId", ErrorId.ToString());

        if (innerException is not null)
        {
            CausedBy(innerException);
        }

        DomainExceptionSettings.Instance.OnErrorCreated?.Invoke(errorDescription, this);
    }
}

public static class ErrorCodeExtensions
{
    public static DomainErrorResult ToDomainError(this ErrorCodes errorCodes, Exception? innerException = null)
    {
        return new DomainErrorResult(errorCodes, innerException);
    }
}