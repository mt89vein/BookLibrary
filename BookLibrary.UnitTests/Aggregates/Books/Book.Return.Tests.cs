using BookLibrary.Domain.Aggregates.Abonents;
using BookLibrary.Domain.Aggregates.Books;
using BookLibrary.Domain.Exceptions;

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
        var book = CreateBook();
        var abonentId = new AbonentId(Guid.NewGuid());
        book.Borrow(
            abonement: new Abonement(abonentId, 0),
            borrowedAt: DateTimeOffset.UtcNow,
            returnBefore: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3))
        );
        book.ClearDomainEvents();

        // act
        book.Return(abonentId, DateTimeOffset.UtcNow);

        // assert
        Assert.That(book, Have.SingleDomainEvent<BookReturnedEvent>());
    }

    [Test]
    public void Should_throw_exception_when_try_to_return_without_abonent_id()
    {
        // arrange
        var book = CreateBook(clearDomainEvents: true);

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
        var book = CreateBook(clearDomainEvents: true);

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
        var book = CreateBook();
        var firstAbonentId = new AbonentId(Guid.NewGuid());
        var secondAbonentId = new AbonentId(Guid.NewGuid());

        book.Borrow(
            abonement: new Abonement(firstAbonentId, 0),
            borrowedAt: DateTimeOffset.UtcNow,
            returnBefore: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3))
        );
        book.ClearDomainEvents();

        // act
        void Act()
        {
            book.Return(secondAbonentId, DateTimeOffset.UtcNow);
        }

        // assert
        Assert.That(Act, ErrorCodes.BookNotBorrowedByAbonent.Expect());
    }
}