using BookLibrary.Domain.Aggregates.Abonents;
using BookLibrary.Domain.Aggregates.Books;
using BookLibrary.Domain.ValueObjects;

namespace BookLibrary.UnitTests.Aggregates.Books;

/// <summary>
/// Book aggregate unit tests.
/// </summary>
[TestOf(typeof(Book))]
internal sealed partial class BookTests
{
    [Test]
    public void Should_raise_domain_event_when_book_created()
    {
        // act
        var book = CreateBook();

        // assert
        Assert.That(book, Have.SingleDomainEvent<BookCreatedEvent>());
    }

    [Test]
    public void Should_remove_all_domain_events_when_clear_called()
    {
        // arrange
        var book = CreateBook();

        // act
        book.ClearDomainEvents();

        // assert
        Assert.That(book, Have.NoDomainEvents());
    }

    /// <summary>
    /// Creates instance of <see cref="Book"/>.
    /// </summary>
    /// <param name="clearDomainEvents">Is need to clear domain events?</param>
    private static Book CreateBook(bool clearDomainEvents = false)
    {
        var book = new Book(new BookId(Guid.NewGuid()),
            GetDummyBookTitle(),
            GetDummyIsbn(),
            new BookPublicationDate(DateOnly.FromDateTime(DateTimeOffset.UtcNow.UtcDateTime)),
            [GetDummyAuthor()],
            DateTimeOffset.UtcNow
        );

        if (clearDomainEvents)
        {
            book.ClearDomainEvents();
        }

        return book;
    }

    private static BookTitle GetDummyBookTitle()
    {
        return new BookTitle("Dummy book title");
    }

    private static Isbn GetDummyIsbn()
    {
        return new Isbn("9780134434421");
    }

    private static Author GetDummyAuthor()
    {
        return new Author("Dummy", "Author");
    }

    private static AbonentId GetNewDummyAbonentId()
    {
        return new AbonentId(Guid.NewGuid());
    }
}