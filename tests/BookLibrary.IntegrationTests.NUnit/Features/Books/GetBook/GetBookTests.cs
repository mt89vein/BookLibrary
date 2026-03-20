using BookLibrary.Api.Features;
using BookLibrary.Application.Features.Books.GetBook;
using BookLibrary.Domain.Exceptions;
using BookLibrary.IntegrationTests.NUnit.Assertions;
using BookLibrary.IntegrationTests.NUnit.Fixtures;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace BookLibrary.IntegrationTests.NUnit.Features.Books.GetBook;

[TestOf(typeof(GetBookController))]
[TestOf(typeof(GetBookUseCase))]
public sealed class GetBookTests : ApiTestBase
{
    [Test]
    public async Task Returns_existing_book_by_id()
    {
        var book = await SeedBookAsync();

        using var response = await HttpClient.GetAsync(GetUri(book.Id.Value));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(response, Response.IsNotProblemDetails);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
        await response.VerifyResponseAsync();
    }

    [Test]
    public async Task Return_not_found_when_book_not_exists()
    {
        var notExistingBookId = Guid.NewGuid();

        using var response = await HttpClient.GetAsync(GetUri(notExistingBookId));

        Assert.That(response, ErrorCodes.BookNotFound.AsProblemDetails(HttpStatusCode.OK));
    }

    private static Uri GetUri(Guid bookId)
    {
        var qs = QueryString.Create("bookId", bookId.ToString());

        return new Uri($"api/v1/books{qs}", UriKind.Relative);
    }
}