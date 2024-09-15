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
    public void Should_raise_domain_event_when_book_borrowed()
    {
        // arrange
        var book = CreateBook(clearDomainEvents: true);

        // act
        book.Borrow(
            abonement: new Abonement(GetNewDummyAbonentId(), 0),
            borrowedAt: DateTimeOffset.UtcNow,
            returnBefore: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3))
        );

        // assert
        Assert.That(book, Have.SingleDomainEvent<BookBorrowedEvent>());
    }

    [Test]
    public void Should_throw_exception_when_try_to_borrow_book_with_return_date_in_the_past()
    {
        // arrange
        var book = CreateBook();

        // act
        void Act()
        {
            book.Borrow(
                abonement: new Abonement(GetNewDummyAbonentId(), 0),
                borrowedAt: DateTimeOffset.UtcNow,
                returnBefore: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1))
            );
        }

        // assert
        Assert.That(Act, Throws.ArgumentException);
    }

    [Test]
    public void Should_set_return_before_to_default_thirty_days_when_not_specified()
    {
        // arrange
        var book = CreateBook(clearDomainEvents: true);

        // act
        book.Borrow(
            abonement: new Abonement(GetNewDummyAbonentId(), 0),
            borrowedAt: DateTimeOffset.UtcNow,
            returnBefore: null
        );

        // assert
        Assert.That(book, Have.SingleDomainEvent<BookBorrowedEvent>());
        var de = (BookBorrowedEvent)book.DomainEvents.Single();
        Assert.That(
            de.ReturnBefore,
            Is.EqualTo(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30))),
            "ReturnBefore not matched"
        );
    }

    [Test]
    public void Should_throw_exception_when_try_to_borrow_book_that_was_already_borrowed_by_another_abonent()
    {
        // arrange
        var firstAbonentId = new AbonentId(Guid.NewGuid());
        var secondAbonentId = new AbonentId(Guid.NewGuid());

        var book = new BookBuilder().SetBorrowedBy(firstAbonentId).Build();

        // act
        void Act()
        {
            book.Borrow(
                abonement: new Abonement(secondAbonentId, 0),
                borrowedAt: DateTimeOffset.UtcNow,
                returnBefore: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10))
            );
        }

        // assert
        Assert.That(Act, ErrorCodes.BookAlreadyBorrowed.Expect());
    }

    [Test]
    public void Should_do_nothing_when_try_to_borrow_book_that_was_already_borrowed_by_abonent()
    {
        // arrange
        var abonentId = new AbonentId(Guid.NewGuid());
        var book = new BookBuilder().SetBorrowedBy(abonentId).Build();

        // act
        book.Borrow(
            abonement: new Abonement(abonentId, 0),
            borrowedAt: DateTimeOffset.UtcNow,
            returnBefore: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10))
        );

        // assert
        Assert.That(book, Have.NoDomainEvents());
    }

    [Test]
    public void Should_throw_exception_when_try_to_borrow_book_with_exceeded_limit_of_books()
    {
        // arrange
        var book = CreateBook(clearDomainEvents: true);

        // act
        void Act()
        {
            book.Borrow(
                abonement: new Abonement(GetNewDummyAbonentId(), 3),
                borrowedAt: DateTimeOffset.UtcNow,
                returnBefore: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10))
            );
        }

        // assert
        Assert.That(Act, ErrorCodes.TooManyBooksBorrowedAlready.Expect());
    }
}