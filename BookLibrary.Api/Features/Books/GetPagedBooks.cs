using BookLibrary.Application.Features.Books.GetPagedBooks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Sstv.DomainExceptions.Extensions.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Api.Features;

/// <summary>
/// Get Book controller.
/// </summary>
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/v1/books")]
[Tags("Books")]
public sealed class GetPagedBooksController : ControllerBase
{
    /// <summary>
    /// Returns books with paging.
    /// </summary>
    /// <param name="model">Books search model.</param>
    /// <param name="useCase">UseCase - get paged books.</param>
    /// <param name="ct">Token for cancel operation.</param>
    [HttpPost("search")]
    [SwaggerRequestExample(typeof(GetPagedBooksModel), typeof(GetPagedBooksModelExample))]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(BookPageDto))]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(BookPageDtoExample))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "In case bad request parameters", typeof(ErrorCodeProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "In case server error", typeof(ErrorCodeProblemDetails))]
    public async Task<IActionResult> GetPagedBooksAsync(
        [FromBody] GetPagedBooksModel model,
        [FromServices] GetPagedBooksUseCase useCase,
        CancellationToken ct = default
    )
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(useCase);

        var query = new GetPagedBooksQuery(model.Page, model.PageSize, model.Title, model.Isbn);

        var pageDto = await useCase.ExecuteAsync(query, ct);

        return Ok(pageDto);
    }
}

/// <summary>
/// Books search model.
/// </summary>
/// <param name="Page">Page number.</param>
/// <param name="PageSize">Page size.</param>
/// <param name="Isbn">Filter by ISBN.</param>
/// <param name="Title">Filter by title.</param>
public sealed record GetPagedBooksModel(
    int Page = 1,
    [Range(5, 25)] int PageSize = 25,
    string? Title = null,
    string? Isbn = null
);

/// <summary>
/// Example of book page search model.
/// </summary>
[UsedImplicitly]
[ExcludeFromCodeCoverage]
internal sealed class GetPagedBooksModelExample : IExamplesProvider<GetPagedBooksModel>
{
    public GetPagedBooksModel GetExamples()
    {
        return new GetPagedBooksModel
        {
            Isbn = null,
            Title = null,
            Page = 1,
            PageSize = 10
        };
    }
}

/// <summary>
/// Example of book page.
/// </summary>
[UsedImplicitly]
[ExcludeFromCodeCoverage]
internal sealed class BookPageDtoExample : IExamplesProvider<BookPageDto>
{
    public BookPageDto GetExamples()
    {
        return new BookPageDto
        {
            Page = 1,
            PageSize = 10,
            HasNextPage = false,
            Items = new[]
            {
                new BookPageItemDto
                {
                    Title = "Domain-Driven Design distilled",
                    Isbn = "9780134434421",
                    PublicationDate = new DateOnly(2024, 01, 24),
                    Authors = new[]
                    {
                        new BookPageItemAuthorDto
                        {
                            Name = "Vaughn",
                            Surname = "Vernon"
                        }
                    },
                    Available = true
                }
            }
        };
    }
}