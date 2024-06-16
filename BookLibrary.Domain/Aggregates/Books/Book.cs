using BookLibrary.Domain.Aggregates.Abonents;
using BookLibrary.Domain.Exceptions;
using BookLibrary.Domain.ValueObjects;
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
    public void Borrow(Abonement abonement, DateTimeOffset borrowedAt, DateOnly? returnBefore)
    {
        ArgumentNullException.ThrowIfNull(abonement);

        // by default 30 days
        var returnBeforeDate = returnBefore ?? DateOnly.FromDateTime(borrowedAt.AddDays(30).Date);

        if (DateOnly.FromDateTime(borrowedAt.Date) >= returnBeforeDate)
        {
            throw new ArgumentException("Book return date must be later than borrowing time");
        }

        if (BorrowInfo is not null)
        {
            if (BorrowInfo.AbonentId != abonement.AbonentId)
            {
                throw ErrorCodes.BookAlreadyBorrowed.ToException();
            }

            return;
        }

        if (abonement.BorrowedBooksCount >= 3)
        {
            throw ErrorCodes.TooManyBooksBorrowedAlready.ToException();
        }

        BorrowInfo = new BorrowInfo(abonement.AbonentId, borrowedAt, returnBeforeDate);

        // hint: there we can check amortization percent of book before borrow
        AddDomainEvent(new BookBorrowedEvent(Id, BorrowInfo.AbonentId, BorrowInfo.BorrowedAt, BorrowInfo.ReturnBefore));
    }

    /// <summary>
    /// Return the book.
    /// </summary>
    /// <param name="abonentId">Abonent identifier.</param>
    /// <param name="returnedAt">DateTime when book was returned.</param>
    public void Return(AbonentId abonentId, DateTimeOffset returnedAt)
    {
        if (abonentId == default)
        {
            throw ErrorCodes.InvalidBookReturnAbonentId.ToException();
        }

        if (BorrowInfo is null)
        {
            throw ErrorCodes.BookNotBorrowedByAnyone.ToException();
        }

        if (BorrowInfo.AbonentId != abonentId)
        {
            throw ErrorCodes.BookNotBorrowedByAbonent.ToException();
        }

        // TODO: we need history of borrowed books (like activity)

        BorrowInfo = null;

        // hint: there we can check amortization percent of book after return
        AddDomainEvent(new BookReturnedEvent(Id, abonentId, returnedAt));
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