using Seedwork;

namespace BookLibrary.Domain.Aggregates.Abonents.Events;

/// <summary>
/// Event occurs when abonent registered.
/// </summary>
/// <param name="AbonentId">Abonent identifier.</param>
public sealed record AbonentRegistredEvent(AbonentId AbonentId) : IDomainEvent;