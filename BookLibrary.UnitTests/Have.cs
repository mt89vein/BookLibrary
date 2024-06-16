using NUnit.Framework.Constraints;
using Seedwork;

namespace BookLibrary.UnitTests;

/// <summary>
/// Assert constraints.
/// </summary>
public static class Have
{
    /// <summary>
    /// Entity should have only one domain event. And it's type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Domain event type.</typeparam>
    public static Constraint SingleDomainEvent<T>()
        where T : IDomainEvent
    {
        return DomainEvent<T>()
            .With.Property(nameof(IEntity.DomainEvents))
            .Count.EqualTo(1);
    }

    /// <summary>
    /// Entity should have domain event with <typeparamref name="T"/>. type.
    /// </summary>
    /// <typeparam name="T">Domain event type.</typeparam>
    public static Constraint DomainEvent<T>()
        where T : IDomainEvent
    {
        return Is.AssignableTo<IEntity>()
            .With.Property(nameof(IEntity.DomainEvents))
            .Matches<IReadOnlyCollection<IDomainEvent>>(events =>
            {
                return events.Any(de => de.GetType() == typeof(T));
            });
    }

    /// <summary>
    /// Entity should not have any domain events.
    /// </summary>
    public static Constraint NoDomainEvents()
    {
        return Is.AssignableTo<IEntity>()
            .With.Property(nameof(IEntity.DomainEvents))
            .Empty;
    }
}