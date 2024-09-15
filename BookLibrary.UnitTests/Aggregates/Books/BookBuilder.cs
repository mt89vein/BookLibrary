using BookLibrary.Domain.Aggregates.Abonents;
using BookLibrary.Domain.Aggregates.Books;
using BookLibrary.Domain.ValueObjects;
using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.UnitTests.Aggregates.Books;

/// <summary>
/// Builder that simplifies creation of book for tests.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class BookBuilder
{
    private BookId _bookId;
    private BookTitle _bookTitle;
    private Isbn _isbn;
    private BookPublicationDate _publicationDate;
    private Author[] _authors;
    private DateTimeOffset _createdAt;

    private AbonentId? _borrowedBy;
    private DateTimeOffset? _borrowedAt;
    private DateTimeOffset? _returnBefore;

    public BookBuilder()
    {
        _bookId = new BookId(Guid.NewGuid());
        _bookTitle = new BookTitle("Dummy book title");
        _isbn = new Isbn("9780134434421");
        _publicationDate = new BookPublicationDate(DateOnly.FromDateTime(DateTimeOffset.UtcNow.UtcDateTime));
        _authors = [new Author("Dummy Author Name", "Dummy Author Surname")];
        _createdAt = DateTimeOffset.UtcNow;
    }

    public static Book CreateBook(bool clearDomainEvents = false)
    {
        return new BookBuilder().Build(clearDomainEvents);
    }

    public BookBuilder WithBookId(BookId bookId)
    {
        _bookId = bookId;

        return this;
    }

    public BookBuilder WithBookTitle(BookTitle bookTitle)
    {
        _bookTitle = bookTitle;

        return this;
    }

    public BookBuilder WithIsbn(Isbn isbn)
    {
        _isbn = isbn;

        return this;
    }

    public BookBuilder WithPublicationDate(BookPublicationDate publicationDate)
    {
        _publicationDate = publicationDate;

        return this;
    }

    public BookBuilder WithAuthors(params Author[] authors)
    {
        _authors = authors;

        return this;
    }

    public BookBuilder WithCreatedAt(DateTimeOffset createdAt)
    {
        _createdAt = createdAt;

        return this;
    }

    public BookBuilder SetBorrowedBy(AbonentId borrowedBy, DateTimeOffset? borrowedAt = null, DateTimeOffset? returnBefore = null)
    {
        _borrowedBy = borrowedBy;
        _borrowedAt = borrowedAt ?? DateTimeOffset.UtcNow;
        _returnBefore = returnBefore ?? _borrowedAt.Value.AddDays(5);

        return this;
    }

    public Book Build(bool clearDomainEvents = true)
    {
        var book = new Book(_bookId, _bookTitle, _isbn, _publicationDate, _authors, _createdAt);

        if (_borrowedBy.HasValue && _borrowedAt.HasValue && _returnBefore.HasValue)
        {
            book.Borrow(
                abonement: new Abonement(_borrowedBy.Value, 0),
                borrowedAt: _borrowedAt ?? DateTimeOffset.UtcNow,
                returnBefore: DateOnly.FromDateTime(_returnBefore.Value.DateTime)
            );
        }

        if (clearDomainEvents)
        {
            book.ClearDomainEvents();
        }

        return book;
    }
}