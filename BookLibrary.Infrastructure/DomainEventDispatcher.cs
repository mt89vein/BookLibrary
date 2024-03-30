using Mediator;
using Microsoft.EntityFrameworkCore;
using Seedwork;

namespace BookLibrary.Infrastructure;

/// <summary>
/// Domain events dispatcher.
/// </summary>
/// <typeparam name="TDbContext">DbContext type.</typeparam>
internal sealed class DomainEventDispatcher<TDbContext> : IDomainEventDispatcher<TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    /// Mediator.
    /// </summary>
    private readonly IMediator _mediator;

    /// <summary>
    /// DbContext.
    /// </summary>
    private TDbContext? _dbContext;

    /// <summary>
    /// Creates new instance of <see cref="DomainEventDispatcher{T}"/>.
    /// </summary>
    /// <param name="mediator">Mediator.</param>
    public DomainEventDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Sets DbContext.
    /// </summary>
    /// <param name="dbContext">DbContext.</param>
    public void SetDbContext(TDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        _dbContext = dbContext;
    }

    /// <summary>
    /// Dispatches domain events.
    /// </summary>
    /// <param name="ct">Token for cancel operation.</param>
    public async Task DispatchDomainEventsAsync(CancellationToken ct = default)
    {
        if (_dbContext is null)
        {
            throw new InvalidOperationException("DbContext was null");
        }

        while (HasUnpublishedDomainEvents())
        {
            var entities = _dbContext.ChangeTracker
                .Entries<IEntity>()
                .Where(a => a.Entity.DomainEvents.Count > 0)
                .Select(a => a.Entity)
                .ToArray();

            foreach (var entity in entities)
            {
                var domainEvents = entity.DomainEvents.ToArray();
                entity.ClearDomainEvents();

                foreach (var domainEvent in domainEvents)
                {
                    await _mediator.Publish(domainEvent, ct).ConfigureAwait(false);
                }
            }
        }
    }

    /// <summary>
    /// Check, is there any domain event that needs to be published.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// When DbContext was null.
    /// </exception>
    /// <returns>True, when has domain event for publishing.</returns>
    private bool HasUnpublishedDomainEvents()
    {
        if (_dbContext is null)
        {
            throw new InvalidOperationException("DbContext was null");
        }

        return _dbContext.ChangeTracker
            .Entries<IEntity>()
            .Any(a => a.Entity.DomainEvents.Count > 0);
    }
}