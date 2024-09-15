using BookLibrary.Domain.Aggregates.Abonents;
using BookLibrary.Domain.Aggregates.Books;
using BookLibrary.Domain.Exceptions;
using static BookLibrary.UnitTests.Aggregates.Books.BookBuilder;

namespace BookLibrary.UnitTests.Aggregates.Books;

/// <summary>
/// Book aggregate unit tests.
/// </summary>
[TestOf(typeof(Book))]
internal partial class BookTests
{
    [Test]
    public void Should_raise_domain_event_when_book_returned()
    {
        // arrange
        var abonentId = new AbonentId(Guid.NewGuid());
        var book = new BookBuilder().SetBorrowedBy(abonentId).Build();

        // act
        book.Return(abonentId, DateTimeOffset.UtcNow);

        // assert
        Assert.That(book, Have.SingleDomainEvent<BookReturnedEvent>());
    }

    [Test]
    public void Should_throw_exception_when_try_to_return_without_abonent_id()
    {
        // arrange
        var book = CreateBook();

        // act
        void Act()
        {
            book.Return(default, DateTimeOffset.UtcNow);
        }

        // assert
        Assert.That(Act, ErrorCodes.InvalidBookReturnAbonentId.Expect());
    }

    [Test]
    public void Should_throw_exception_when_try_to_return_not_borrowed_book()
    {
        // arrange
        var book = CreateBook();

        // act
        void Act()
        {
            book.Return(GetNewDummyAbonentId(), DateTimeOffset.UtcNow);
        }

        // assert
        Assert.That(Act, ErrorCodes.BookNotBorrowedByAnyone.Expect());
    }

    [Test]
    public void Should_throw_exception_when_try_to_return_book_borrowed_by_another_abonent()
    {
        // arrange
        var firstAbonentId = new AbonentId(Guid.NewGuid());
        var secondAbonentId = new AbonentId(Guid.NewGuid());

        var book = new BookBuilder().SetBorrowedBy(firstAbonentId).Build();

        // act
        void Act()
        {
            book.Return(secondAbonentId, DateTimeOffset.UtcNow);
        }

        // assert
        Assert.That(Act, ErrorCodes.BookNotBorrowedByAbonent.Expect());
    }
}