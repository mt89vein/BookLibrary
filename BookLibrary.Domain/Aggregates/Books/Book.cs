using BookLibrary.Domain.Aggregates.Abonents;
using BookLibrary.Domain.Exceptions;
using BookLibrary.Domain.ValueObjects;
using FluentResults;
using Seedwork;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Domain.Aggregates.Books;

/// <summary>
/// Aggregate root - Book.
/// </summary>
public sealed class Book : Entity
{
    /// <summary>
    /// Book identifier.
    /// </summary>
    public BookId Id { get; private set; }

    /// <summary>
    /// Book title.
    /// </summary>
    public BookTitle Title { get; private set; }

    /// <summary>
    /// ISBN.
    /// </summary>
    public Isbn Isbn { get; private set; }

    /// <summary>
    /// Year when book was published.
    /// </summary>
    public BookPublicationDate PublicationDate { get; private set; }

    /// <summary>
    /// Book authors.
    /// </summary>
    public IReadOnlyCollection<Author> Authors { get; private set; }

    /// <summary>
    /// Borrowing info.
    /// </summary>
    public BorrowInfo? BorrowInfo { get; private set; }

    /// <summary>
    /// When book added to library.
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Creates new instance of <see cref="Book"/>.
    /// </summary>
    /// <param name="bookId">Book identifier.</param>
    /// <param name="bookTitle">Book title.</param>
    /// <param name="isbn">ISBN.</param>
    /// <param name="publicationDate">Book publication date.</param>
    /// <param name="authors">Book authors.</param>
    /// <param name="createdAt">When book added to library.</param>
    /// <exception cref="BookLibraryException">
    /// When <paramref name="bookId"/> was invalid.
    /// </exception>
    /// <exception cref="BookLibraryException">
    /// When <paramref name="authors"/> were empty.
    /// </exception>
    public Book(
        BookId bookId,
        BookTitle bookTitle,
        Isbn isbn,
        BookPublicationDate publicationDate,
        IReadOnlyCollection<Author> authors,
        DateTimeOffset createdAt
    )
    {
        ArgumentNullException.ThrowIfNull(bookTitle);
        ArgumentNullException.ThrowIfNull(isbn);
        ArgumentNullException.ThrowIfNull(publicationDate);
        ArgumentNullException.ThrowIfNull(authors);

        Id = bookId != default
            ? bookId
            : throw ErrorCodes.InvalidBookId.ToException();
        Title = bookTitle;
        Isbn = isbn;
        PublicationDate = publicationDate;
        Authors = authors.Count > 0
            ? new List<Author>(authors)
            : throw ErrorCodes.BookMustHaveAnAuthors.ToException();

        CreatedAt = createdAt;

        AddDomainEvent(new BookCreatedEvent(Title, Isbn, PublicationDate, Count: 1));
    }

    /// <summary>
    /// Borrow book by abonent.
    /// </summary>
    /// <param name="abonement">Borrower.</param>
    /// <param name="borrowedAt">DateTime when book was borrowed.</param>
    /// <param name="returnBefore">DateTime when book must be returned.</param>
    /// <exception cref="ArgumentNullException">When book not provided.</exception>
    /// <exception cref="ArgumentException">
    /// When <paramref name="borrowedAt"/> is later than <paramref name="returnBefore"/>.
    /// </exception>
    /// <exception cref="BookLibraryException">
    /// When a lot of books have already been taken by abonent.
    /// </exception>
    public Result Borrow(Abonement abonement, DateTimeOffset borrowedAt, DateOnly? returnBefore)
    {
        ArgumentNullException.ThrowIfNull(abonement);

        // business rule: applying book library default policy - book borrowing for 30 days
        var returnBeforeDate = returnBefore ?? DateOnly.FromDateTime(borrowedAt.AddDays(30).Date);

        // business rule: return date must be in the future
        if (DateOnly.FromDateTime(borrowedAt.Date) >= returnBeforeDate)
        {
            return ErrorCodes.InvalidBookBorrowingPeriod.ToDomainError();
        }

        // business rule: this book must be available to borrow
        if (BorrowInfo is not null)
        {
            return BorrowInfo.AbonentId != abonement.AbonentId
                ? ErrorCodes.BookAlreadyBorrowed.ToDomainError()
                : Result.Ok();
        }

        // business rule: applying book library policy: no more than 3 books per abonent at the same time
        if (abonement.BorrowedBooksCount >= 3)
        {
            return ErrorCodes.TooManyBooksBorrowedAlready.ToDomainError();
        }

        // changing book internal state (invariant)
        BorrowInfo = new BorrowInfo(abonement.AbonentId, borrowedAt, returnBeforeDate);

        // registering domain event
        AddDomainEvent(new BookBorrowedEvent(Id, BorrowInfo.AbonentId, BorrowInfo.BorrowedAt, BorrowInfo.ReturnBefore));

        return Result.Ok();
    }

    /// <summary>
    /// Return the book.
    /// </summary>
    /// <param name="abonentId">Abonent identifier.</param>
    /// <param name="returnedAt">DateTime when book was returned.</param>
    public Result Return(AbonentId abonentId, DateTimeOffset returnedAt)
    {
        if (abonentId == default)
        {
            return ErrorCodes.InvalidBookReturnAbonentId.ToDomainError();
        }

        if (BorrowInfo is null)
        {
            return ErrorCodes.BookNotBorrowedByAnyone.ToDomainError();
        }

        if (BorrowInfo.AbonentId != abonentId)
        {
            return ErrorCodes.BookNotBorrowedByAbonent.ToDomainError();
        }

        // TODO: we need history of borrowed books (like activity)

        BorrowInfo = null;

        // hint: there we can check amortization percent of book after return
        AddDomainEvent(new BookReturnedEvent(Id, abonentId, returnedAt));

        return Result.Ok();
    }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    /// <summary>
    /// Constructor for EF.
    /// </summary>
    [Obsolete("Constructor only for EFCore, because it needs to ctor without parameters", error: true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [ExcludeFromCodeCoverage]
    private Book()
    {
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
}