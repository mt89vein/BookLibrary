using BookLibrary.Application.Features.Abonents.RegisterAbonent;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Sstv.DomainExceptions.Extensions.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace BookLibrary.Api.Features.Abonents;

/// <summary>
/// Abonent registration controller.
/// </summary>
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/v1/abonents")]
[Tags("Abonents")]
public sealed class RegisterAbonentController : ControllerBase
{
    /// <summary>
    /// Registeres abonent.
    /// </summary>
    /// <param name="model">Add book model.</param>
    /// <param name="useCase">UseCase - add new books.</param>
    /// <param name="ct">Token for cancel operation.</param>
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status200OK)]
    [SwaggerRequestExample(typeof(RegisterAbonentModel), typeof(RegisterAbonentModelExample))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "In case bad request parameters", typeof(ErrorCodeProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "In case server error", typeof(ErrorCodeProblemDetails))]
    public async Task<IActionResult> RegisterAbonentAsync(
        RegisterAbonentModel model,
        [FromServices] RegisterAbonentUseCase useCase,
        CancellationToken ct = default
    )
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(useCase);

        var command = new RegisterAbonentCommand(
            model.Email,
            model.Surname,
            model.Name,
            model.Patronymic
        );

        await useCase.ExecuteAsync(command, ct);

        return Ok();
    }
}

/// <summary>
/// Register new abonent model.
/// </summary>
/// <param name="Email">Email.</param>
/// <param name="Surname">Last name.</param>
/// <param name="Name">First name.</param>
/// <param name="Patronymic">Middle name.</param>
public sealed record RegisterAbonentModel(
    string Email,
    string Surname,
    string Name,
    string? Patronymic
);

/// <summary>
/// Validator for <see cref="AddNewBooksModel"/>.
/// </summary>
[UsedImplicitly]
internal sealed class RegisterAbonentModelValidator : AbstractValidator<RegisterAbonentModel>
{
    public RegisterAbonentModelValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .Must(x => x.Contains('@'))
            .WithMessage("Invalid Email");

        RuleFor(x => x.Surname)
            .NotEmpty()
            .WithMessage("Surname cannot be empty");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name cannot be empty");
    }
}

/// <summary>
/// Example of request to register abonent.
/// </summary>
[UsedImplicitly]
internal sealed class RegisterAbonentModelExample : IExamplesProvider<RegisterAbonentModel>
{
    public RegisterAbonentModel GetExamples()
    {
        return new RegisterAbonentModel(
            Email: "my-email@gmail.com",
            Surname: "Marblemaw",
            Name: "Solace",
            Patronymic: null
        );
    }
}