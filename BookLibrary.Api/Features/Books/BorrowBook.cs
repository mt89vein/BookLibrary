using BookLibrary.Api.Extensions;
using BookLibrary.Application.Features.Books.BorrowBook;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Sstv.DomainExceptions.Extensions.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace BookLibrary.Api.Features;

/// <summary>
/// Borrow book controller.
/// </summary>
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/v1/books")]
[Tags("Books")]
public sealed class BorrowBookController : ControllerBase
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
            model.Isbn,
            model.PublicationDate,
            model.ReturnDate
        );

        await useCase.ExecuteAsync(command, ct);

        return Ok();
    }
}

/// <summary>
/// Borrow book model.
/// </summary>
/// <param name="Isbn">ISBN.</param>
/// <param name="PublicationDate">Book publication date.</param>
/// <param name="ReturnDate">Date when abonent what to return book.</param>
public sealed record BorrowBookModel(
    string Isbn,
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
            .WithMessage("Invalid ISBN");
    }
}

/// <summary>
/// Example of request for borrow book.
/// </summary>
[UsedImplicitly]
internal sealed class BorrowBookModelExample : IExamplesProvider<BorrowBookModel>
{
    public BorrowBookModel GetExamples()
    {
        return new BorrowBookModel(
            Isbn: "9780134434421",
            PublicationDate: new DateOnly(2024, 01, 24),
            ReturnDate: null
        );
    }
}