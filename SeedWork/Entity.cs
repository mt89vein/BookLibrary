namespace Seedwork;

/// <summary>
/// Entity interface.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Collection of domain events.
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Removes all domain events.
    /// </summary>
    void ClearDomainEvents();

    /// <summary>
    /// Moves all domain events from <paramref name="entity"/> to current.
    /// </summary>
    /// <param name="entity">Entity.</param>
    void MoveDomainEventsFrom(IEntity entity);
}

/// <summary>
/// Entity base class.
/// </summary>
public abstract class Entity : IEntity
{
    /// <summary>
    /// Collection of domain events.
    /// </summary>
    private List<IDomainEvent>? _domainEvents;

    /// <summary>
    /// Collection of domain events.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents
    {
        get
        {
            if (_domainEvents is not null)
            {
                return _domainEvents.AsReadOnly();
            }

            return Array.Empty<IDomainEvent>();
        }
    }

    /// <summary>
    /// Adds domain event to collection.
    /// </summary>
    /// <param name="domainEvent">Domain event to add.</param>
    /// <exception cref="ArgumentNullException">
    /// Throws, when <paramref name="domainEvent"/> was null.
    /// </exception>
    protected virtual void AddDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        _domainEvents ??= [];
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Replace domain event in collection.
    /// </summary>
    /// <param name="domainEvent">Domain event to add.</param>
    /// <exception cref="ArgumentNullException">
    /// Throws, when <paramref name="domainEvent"/> was null.
    /// </exception>
    protected virtual void ReplaceDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        _domainEvents ??= [];
        _domainEvents.RemoveAll(x => x.GetType() == domainEvent.GetType());
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Removes all domain events.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }

    /// <summary>
    /// Moves all domain events from <paramref name="entity"/> to current.
    /// </summary>
    /// <param name="entity">Entity.</param>
    public void MoveDomainEventsFrom(IEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _domainEvents ??= [];
        _domainEvents.AddRange(entity.DomainEvents);
        entity.ClearDomainEvents();
    }
}