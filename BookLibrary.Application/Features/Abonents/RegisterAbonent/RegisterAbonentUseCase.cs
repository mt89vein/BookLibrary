using BookLibrary.Application.Infrastructure;
using BookLibrary.Domain.Aggregates.Abonents;
using BookLibrary.Domain.Exceptions;
using BookLibrary.Domain.ValueObjects;
using EntityFramework.Exceptions.Common;
using Microsoft.Extensions.Logging;

namespace BookLibrary.Application.Features.Abonents.RegisterAbonent;

/// <summary>
/// Register abonent command.
/// </summary>
/// <param name="Email">Email.</param>
/// <param name="Surname">Last name.</param>
/// <param name="Name">First name.</param>
/// <param name="Patronymic">Middle name.</param>
public sealed record RegisterAbonentCommand(
    string Email,
    string Surname,
    string Name,
    string? Patronymic
);

/// <summary>
/// UseCase - register new abonent.
/// </summary>
public sealed class RegisterAbonentUseCase
{
    private readonly IApplicationContext _ctx;
    private readonly IUuidGenerator _uuidGenerator;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<RegisterAbonentUseCase> _logger;

    public RegisterAbonentUseCase(
        IApplicationContext ctx,
        IUuidGenerator uuidGenerator,
        TimeProvider timeProvider,
        ILogger<RegisterAbonentUseCase> logger
    )
    {
        _ctx = ctx;
        _logger = logger;
        _uuidGenerator = uuidGenerator;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Execute the UseCase.
    /// </summary>
    /// <param name="command">Command to register abonent.</param>
    /// <param name="ct">Token to cancel operation.</param>
    /// <exception cref="BookLibraryException">
    /// When there are problem with registering abonent.
    /// </exception>
    public async Task ExecuteAsync(RegisterAbonentCommand command, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var abonentId = new AbonentId(_uuidGenerator.GenerateNew());
        var email = new Email(command.Email);

        using var _ = _logger.BeginScope(new Dictionary<string, object?>
        {
            ["AbonentId"] = abonentId.ToString(),
            ["Email"] = email.ToString(),
        });

        try
        {
            _logger.LogInformation("Registering new abonent");

            var abonent = new Abonent(
                abonentId,
                new AbonentName(
                    name: command.Name,
                    surname: command.Surname,
                    patronymic: command.Patronymic
                ),
                email,
                createdAt: _timeProvider.GetUtcNow()
            );

            _ctx.Abonents.Add(abonent);

            await _ctx.SaveChangesAsync(ct);

            _logger.LogInformation("Abonent registered");
        }
        catch (Exception e) when (e is not BookLibraryException)
        {
            if (e is UniqueConstraintException c && c.ConstraintProperties.Contains(nameof(Abonent.Email)))
            {
                throw ErrorCodes.EmailAlreadyExists.ToException(e);
            }

            throw ErrorCodes.AbonentRegisteringFailed.ToException(e);
        }
    }
}