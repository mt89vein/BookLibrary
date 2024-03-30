using BookLibrary.Application.Features.Books.AddNewBook;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Sstv.DomainExceptions.Extensions.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace BookLibrary.Api.Features;

/// <summary>
/// Add new books controller.
/// </summary>
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/v1/books")]
[Tags("Books")]
public sealed class AddNewBooksController : ControllerBase
{
    /// <summary>
    /// Adds book to library.
    /// </summary>
    /// <param name="model">Add book model.</param>
    /// <param name="useCase">UseCase - add new books.</param>
    /// <param name="ct">Token for cancel operation.</param>
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status200OK)]
    [SwaggerRequestExample(typeof(AddNewBooksModel), typeof(AddNewBooksModelExample))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "In case bad request parameters", typeof(ErrorCodeProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "In case server error", typeof(ErrorCodeProblemDetails))]
    public async Task<IActionResult> AddBooksAsync(
        AddNewBooksModel model,
        [FromServices] AddNewBookUseCase useCase,
        CancellationToken ct = default
    )
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(useCase);

        var command = new AddNewBookCommand(
            model.Isbn,
            model.Title,
            model.PublicationDate,
            model.Authors.Select(x => new BookAuthor(x.Name, x.Surname, x.Patronymic)).ToArray(),
            model.Count,
            UserId: Guid.NewGuid() // imagine that we have authentication
        );

        await useCase.ExecuteAsync(command, ct);

        return Ok();
    }
}

/// <summary>
/// Add new books model.
/// </summary>
/// <param name="Isbn">ISBN.</param>
/// <param name="Title">Book title.</param>
/// <param name="PublicationDate">Book publication date.</param>
/// <param name="Authors">Book authors.</param>
/// <param name="Count">Count of books to create.</param>
public sealed record AddNewBooksModel(
    string Isbn,
    string Title,
    DateOnly PublicationDate,
    IReadOnlyCollection<AddNewBookAuthorModel> Authors,
    int Count
);

/// <summary>
/// Book author.
/// </summary>
/// <param name="Name">First name.</param>
/// <param name="Surname">Last name.</param>
/// <param name="Patronymic">Middle name.</param>
public sealed record AddNewBookAuthorModel(string Name, string Surname, string? Patronymic = null);

/// <summary>
/// Validator for <see cref="AddNewBooksModel"/>.
/// </summary>
[UsedImplicitly]
internal sealed class AddNewBooksModelValidator : AbstractValidator<AddNewBooksModel>
{
    public AddNewBooksModelValidator()
    {
        RuleFor(x => x.Isbn)
            .NotEmpty()
            .WithMessage("Invalid ISBN");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Invalid book title");

        RuleFor(x => x.PublicationDate)
            .NotEmpty()
            .WithMessage("Invalid book publication date");

        RuleFor(x => x.Count)
            .InclusiveBetween(1, 1000)
            .WithMessage("Books count must be in range [1, 1000]");

        RuleFor(x => x.Authors)
            .NotEmpty()
            .WithMessage("Book authors not set");

        RuleForEach(x => x.Authors)
            .SetValidator(new AddNewBookAuthorModelValidator());
    }
}

/// <summary>
/// Validator for <see cref="AddNewBookAuthorModel"/>.
/// </summary>
[UsedImplicitly]
internal sealed class AddNewBookAuthorModelValidator : AbstractValidator<AddNewBookAuthorModel>
{
    public AddNewBookAuthorModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Invalid book author name");

        RuleFor(x => x.Surname)
            .NotEmpty()
            .WithMessage("Invalid book author surname");
    }
}

/// <summary>
/// Example of request for adding new books.
/// </summary>
[UsedImplicitly]
internal sealed class AddNewBooksModelExample : IExamplesProvider<AddNewBooksModel>
{
    public AddNewBooksModel GetExamples()
    {
        return new AddNewBooksModel(
            Isbn: "9780134434421",
            Title: "Domain-Driven Design distilled",
            PublicationDate: new DateOnly(2024, 01, 24),
            Authors: [new AddNewBookAuthorModel("Vaughn", "Vernon")],
            Count: 10
        );
    }
}