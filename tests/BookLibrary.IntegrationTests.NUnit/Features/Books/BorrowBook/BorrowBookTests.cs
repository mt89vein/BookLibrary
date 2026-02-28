using BookLibrary.Api.Features;
using BookLibrary.Application.Features.Books.BorrowBook;
using BookLibrary.Domain.Exceptions;
using BookLibrary.IntegrationTests.NUnit.Assertions;
using BookLibrary.IntegrationTests.NUnit.Fixtures;
using System.Net;

namespace BookLibrary.IntegrationTests.NUnit.Features.Books.BorrowBook;

[TestOf(typeof(BorrowBookController))]
[TestOf(typeof(BorrowBookUseCase))]
public sealed class BorrowBookTests : ApiTestBase
{
    [Test]
    public async Task Borrows_existing_free_book_by_id()
    {
        var book = await SeedBookAsync();

        using var response = await SendBorrowRequestAsync(BorrowByBookId(book.Id.Value));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response, Response.IsNotProblemDetails);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        await response.VerifyResponseAsync();
    }

    [Test]
    public async Task Borrows_existing_free_book_by_isbn()
    {
        var book = await SeedBookAsync();

        using var response = await SendBorrowRequestAsync(BorrowByIsbn(book.Isbn));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response, Response.IsNotProblemDetails);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        await response.VerifyResponseAsync();
    }

    [Test]
    public async Task Return_not_found_when_book_not_exists_by_id()
    {
        var notExistingBookId = Guid.NewGuid();

        using var response = await SendBorrowRequestAsync(BorrowByBookId(notExistingBookId));

        Assert.That(response, ErrorCodes.BookNotFound.AsProblemDetails(HttpStatusCode.OK));
    }

    [Test]
    public async Task Return_not_found_when_book_not_exists_by_isbn()
    {
        const string NOT_EXISTING_ISBN = "9781449373320";

        using var response = await SendBorrowRequestAsync(BorrowByIsbn(NOT_EXISTING_ISBN));

        Assert.That(response, ErrorCodes.ThereNoBookThatCanBeBorrowed.AsProblemDetails(HttpStatusCode.OK));
    }

    // TODO: other test cases such as:
    // - book (by id) already borrowed by someone
    // - abonement borrow limit exceeded
    // - return date too late (cause no more 30 days)
    // - and so on

    private static Dictionary<string, string?> BorrowByBookId(Guid bookId, DateOnly? returnDate = null)
    {
        return new Dictionary<string, string?>
        {
            ["BookId"] = bookId.ToString(),
            ["ReturnDate"] = returnDate?.ToString("O")
        };
    }

    private static Dictionary<string, string?> BorrowByIsbn(string isbn, DateOnly? returnDate = null)
    {
        return new Dictionary<string, string?>
        {
            ["Isbn"] = isbn,
            ["ReturnDate"] = returnDate?.ToString("O")
        };
    }

    private async Task<HttpResponseMessage> SendBorrowRequestAsync(Dictionary<string, string?> formData)
    {
        var message = new HttpRequestMessage(HttpMethod.Post, "api/v1/books/borrow")
        {
            Content = new FormUrlEncodedContent(formData)
        };

        return await HttpClient.SendAsync(message);
    }
}