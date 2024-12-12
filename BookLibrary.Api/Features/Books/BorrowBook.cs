using BookLibrary.Api.Extensions;
using BookLibrary.Application.Features.Books.BorrowBook;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Sstv.DomainExceptions.Extensions.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using UUIDNext;

namespace BookLibrary.Api.Features;

/// <summary>
/// Borrow book controller.
/// </summary>
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/v1/books")]
[Tags("Books")]
public sealed class BorrowBookController : ApiController
{
    /// <summary>
    /// Borrow book from library.
    /// </summary>
    /// <param name="model">Borrow book model.</param>
    /// <param name="useCase">UseCase - borrow book.</param>
    /// <param name="ct">Token for cancel operation.</param>
    [HttpPost("borrow")]
    [SwaggerRequestExample(typeof(BorrowBookModel), typeof(BorrowBookModelExample))]
    [SwaggerResponse(StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "In case bad request parameters", typeof(ErrorCodeProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "In case server error", typeof(ErrorCodeProblemDetails))]
    public async Task<IActionResult> BorrowBookAsync(
        [FromForm] BorrowBookModel model,
        [FromServices] BorrowBookUseCase useCase,
        CancellationToken ct = default
    )
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(useCase);

        var command = new BorrowBookCommand(
            AbonentId: HttpContext.GetUserId(),
            model.BookId,
            model.Isbn,
            model.PublicationDate,
            model.ReturnDate
        );

        var result = await useCase.ExecuteAsync(command, ct);

        return Ok(result);
    }
}

/// <summary>
/// Borrow book model.
/// </summary>
/// <param name="BookId">Book identifier.</param>
/// <param name="Isbn">ISBN.</param>
/// <param name="PublicationDate">Book publication date.</param>
/// <param name="ReturnDate">Date when abonent what to return book.</param>
public sealed record BorrowBookModel(
    Guid? BookId,
    string? Isbn,
    DateOnly? PublicationDate,
    DateOnly? ReturnDate
);

/// <summary>
/// Validator for <see cref="BorrowBookModel"/>.
/// </summary>
[UsedImplicitly]
internal sealed class BorrowBookModelValidator : AbstractValidator<BorrowBookModel>
{
    public BorrowBookModelValidator()
    {
        RuleFor(x => x.Isbn)
            .NotEmpty()
            .WithMessage("Invalid ISBN")
            .When(x => x.BookId.GetValueOrDefault() == Guid.Empty);

        RuleFor(x => x.BookId)
            .NotEmpty()
            .WithMessage("Invalid Book identifier")
            .When(x => string.IsNullOrWhiteSpace(x.Isbn));
    }
}

/// <summary>
/// Example of request for borrow book.
/// </summary>
[UsedImplicitly]
internal sealed class BorrowBookModelExample : IMultipleExamplesProvider<BorrowBookModel>
{
    public IEnumerable<SwaggerExample<BorrowBookModel>> GetExamples()
    {
        yield return new SwaggerExample<BorrowBookModel>
        {
            Name = "By BookId",
            Description = "Example of borrowing book by it's unique identifier",
            Value = new BorrowBookModel(BookId: Uuid.NewSequential(), null, null, null)
        };

        yield return new SwaggerExample<BorrowBookModel>
        {
            Name = "By ISBN",
            Description = "Example of borrowing book by ISBN",
            Value = new BorrowBookModel(
                BookId: null,
                Isbn: "9780134434421",
                PublicationDate: new DateOnly(2024, 01, 24),
                ReturnDate: null
            )
        };
    }
}