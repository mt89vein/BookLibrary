using BookLibrary.Api.Extensions;
using BookLibrary.Application.Features.Books.ReturnBook;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Sstv.DomainExceptions.Extensions.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using UUIDNext;

namespace BookLibrary.Api.Features;

/// <summary>
/// Return book controller.
/// </summary>
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/v1/books")]
[Tags("Books")]
public sealed class ReturnBookController : ControllerBase
{
    /// <summary>
    /// Return book to library.
    /// </summary>
    /// <param name="model">Return book model.</param>
    /// <param name="useCase">UseCase - return book.</param>
    /// <param name="ct">Token for cancel operation.</param>
    [HttpPost("return")]
    [SwaggerRequestExample(typeof(ReturnBookModel), typeof(ReturnBookModelExample))]
    [SwaggerResponse(StatusCodes.Status200OK)]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "In case bad request parameters", typeof(ErrorCodeProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "In case server error", typeof(ErrorCodeProblemDetails))]
    public async Task<IActionResult> ReturnBookAsync(
        [FromForm] ReturnBookModel model,
        [FromServices] ReturnBookUseCase useCase,
        CancellationToken ct = default
    )
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(useCase);

        var command = new ReturnBookCommand(
            BookId: model.BookId,
            AbonentId: HttpContext.GetUserId()
        );

        await useCase.ExecuteAsync(command, ct);

        return Ok();
    }
}

/// <summary>
/// Return book model.
/// </summary>
/// <param name="BookId">Book identifier.</param>
public sealed record ReturnBookModel(Guid BookId);

/// <summary>
/// Validator for <see cref="ReturnBookModel"/>.
/// </summary>
[UsedImplicitly]
internal sealed class ReturnBookModelValidator : AbstractValidator<ReturnBookModel>
{
    public ReturnBookModelValidator()
    {
        RuleFor(x => x.BookId)
            .NotEmpty()
            .WithMessage("Invalid book identifier");
    }
}

/// <summary>
/// Example of request for return book.
/// </summary>
[UsedImplicitly]
internal sealed class ReturnBookModelExample : IExamplesProvider<ReturnBookModel>
{
    public ReturnBookModel GetExamples()
    {
        return new ReturnBookModel(Uuid.NewSequential());
    }
}