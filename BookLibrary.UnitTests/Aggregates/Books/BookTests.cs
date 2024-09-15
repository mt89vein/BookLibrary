using BookLibrary.Domain.Aggregates.Abonents;
using BookLibrary.Domain.Aggregates.Books;
using static BookLibrary.UnitTests.Aggregates.Books.BookBuilder;

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

    private static AbonentId GetNewDummyAbonentId()
    {
        return new AbonentId(Guid.NewGuid());
    }
}