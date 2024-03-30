using BookLibrary.Api.Extensions;
using BookLibrary.Application.Features.Books.GetBorrowedBooks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Sstv.DomainExceptions.Extensions.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;
using UUIDNext;

namespace BookLibrary.Api.Features;

/// <summary>
/// Get borrowed books controller.
/// </summary>
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/v1/books")]
[Tags("Books")]
public sealed class GetBorrowedBooksController : ControllerBase
{
    /// <summary>
    /// Returns borrowed books.
    /// </summary>
    /// <param name="useCase">UseCase - get borrowed books.</param>
    /// <param name="ct">Token for cancel operation.</param>
    [HttpGet("borrowed")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(BorrowedBooksDto))]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(BorrowedBooksDtoExample))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "In case bad request parameters", typeof(ErrorCodeProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "In case server error", typeof(ErrorCodeProblemDetails))]
    public async Task<IActionResult> GetBorrowedBooksAsync(
        [FromServices] GetBorrowedBooksUseCase useCase,
        CancellationToken ct = default
    )
    {
        ArgumentNullException.ThrowIfNull(useCase);

        var borrowedBooks = await useCase.ExecuteAsync(
            new GetBorrowedBooksQuery(HttpContext.GetUserId()),
            ct
        );

        return Ok(borrowedBooks);
    }
}

/// <summary>
/// Example of response with borrowed books.
/// </summary>
[UsedImplicitly]
[ExcludeFromCodeCoverage]
internal sealed class BorrowedBooksDtoExample : IExamplesProvider<BorrowedBooksDto>
{
    public BorrowedBooksDto GetExamples()
    {
        return new BorrowedBooksDto
        {
            Books = new[]
            {
                new BorrowedBookDto
                {
                    Id = Uuid.NewSequential(),
                    BorrowedAt = DateTimeOffset.UtcNow.AddMinutes(3),
                    ReturnBefore = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
                    Title = "Domain-Driven Design distilled",
                    Isbn = "9780134434421",
                    PublicationDate = new DateOnly(2024, 01, 24),
                }
            }
        };
    }
}