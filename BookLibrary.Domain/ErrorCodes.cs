using Sstv.DomainExceptions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace BookLibrary.Domain.Exceptions;

/// <summary>
/// Error codes.
/// </summary>
[ExceptionConfig(ClassName = "BookLibraryException")]
[ErrorDescription(Prefix = "BL")]
public enum ErrorCodes
{
    [ErrorDescription(Description = "Unknown error")]
    Default = 0,

    [ErrorDescription(Description = "Invalid data")]
    InvalidData = 1,

    [ErrorDescription(Description = "Abonent not found")]
    AbonentNotFound = 2,

    [ErrorDescription(Description = "Book not found")]
    BookNotFound = 3,

    [ErrorDescription(Description = "Invalid book identifier")]
    InvalidBookId = 4,

    [ErrorDescription(Description = "Invalid abonent identifier")]
    InvalidAbonentId = 5,

    [ErrorDescription(Description = "Cannot return not borrowed book")]
    CannotReturnNotBorrowedBook = 6,

    [ErrorDescription(Description = "Invalid book name")]
    InvalidBookTitle = 7,

    [ErrorDescription(Description = "Invalid abonent name")]
    InvalidAbonentName = 8,

    [ErrorDescription(Description = "Invalid abonent surname name")]
    InvalidAbonentSurname = 9,

    [ErrorDescription(Description = "Invalid ISBN")]
    InvalidIsbn = 10,

    [ErrorDescription(Description = "Invalid email")]
    InvalidEmail = 11,

    [ErrorDescription(Description = "Invalid abonent identifier for borrow book")]
    InvalidBorrowerAbonentId = 12,

    [ErrorDescription(Description = "Book already borrowed")]
    BookAlreadyBorrowed = 13,

    [ErrorDescription(Description = "Invalid book publish year")]
    InvalidBookPublishYear = 14,

    [ErrorDescription(Description = "Book not borrowed by anyone")]
    BookNotBorrowedByAnyone = 15,

    [ErrorDescription(Description = "Invalid book author name")]
    InvalidBookAuthorName = 16,

    [ErrorDescription(Description = "Invalid book author surname")]
    InvalidBookAuthorSurname = 17,

    [ErrorDescription(Description = "Book must have an author")]
    BookMustHaveAnAuthors = 18,

    [ErrorDescription(Description = "Book adding failed")]
    BookAddingFailed = 19,

    [ErrorDescription(Description = "Book getting failed")]
    BookGettingFailed = 20,

    [ErrorDescription(Description = "Abonent registering failed")]
    AbonentRegisteringFailed = 21,

    [ErrorDescription(Description = "Email already exists")]
    EmailAlreadyExists = 22,

    [ErrorDescription(Description = "User undefined")]
    UserUndefined = 23,

    [ErrorDescription(Description = "There no book that can be borrowed")]
    ThereNoBookThatCanBeBorrowed = 24,

    [ErrorDescription(Description = "Invalid borrowed books count")]
    InvalidBorrowerBooksCount = 25,

    [ErrorDescription(Description = "Book borrowing failed")]
    BookBorrowingFailed = 26,

    [ErrorDescription(Description = "Too many books borrowed already")]
    TooManyBooksBorrowedAlready = 27,

    [ErrorDescription(Description = "Invalid abonent identifier")]
    InvalidBookReturnAbonentId = 28,

    [ErrorDescription(Description = "Book can't be returned if you not borrowed it")]
    BookNotBorrowedByAbonent = 29,

    [ErrorDescription(Description = "Book returning failed")]
    BookReturningFailed = 30,
}