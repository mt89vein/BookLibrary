using BookLibrary.Application.Features.Books.GetBook;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Sstv.DomainExceptions.Extensions.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using UUIDNext;

namespace BookLibrary.Api.Features;

/// <summary>
/// Get Book controller.
/// </summary>
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/v1/books")]
[Tags("Books")]
public sealed class GetBookController : ControllerBase
{
    /// <summary>
    /// Returns book by id.
    /// </summary>
    /// <param name="bookId">Book identifier.</param>
    /// <param name="useCase">UseCase - get book by id.</param>
    /// <param name="ct">Token for cancel operation.</param>
    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(BookDtoExample))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "In case bad request parameters", typeof(ErrorCodeProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "In case server error", typeof(ErrorCodeProblemDetails))]
    public async Task<IActionResult> GetBookByIdAsync(
        [FromQuery] Guid bookId,
        [FromServices] GetBookUseCase useCase,
        CancellationToken ct = default
    )
    {
        ArgumentNullException.ThrowIfNull(useCase);

        var bookDto = await useCase.ExecuteAsync(bookId, ct);

        return Ok(bookDto);
    }
}

/// <summary>
/// Example of request for adding new books.
/// </summary>
[UsedImplicitly]
internal sealed class BookDtoExample : IExamplesProvider<BookDto>
{
    public BookDto GetExamples()
    {
        return new BookDto
        {
            Id = Uuid.NewSequential(),
            Title = "Domain-Driven Design distilled",
            Isbn = "9780134434421",
            PublicationDate = new DateOnly(2024, 01, 24),
            Authors = [new AuthorDto { Name = "Vaughn", Surname = "Vernon" }],
            BorrowInfo = new BorrowInfoDto
            {
                AbonentId = Uuid.NewSequential(),
                BorrowedAt = DateTimeOffset.UtcNow.Subtract(TimeSpan.FromMinutes(5)),
                ReturnBefore = DateTimeOffset.UtcNow.AddDays(30),
            }
        };
    }
}