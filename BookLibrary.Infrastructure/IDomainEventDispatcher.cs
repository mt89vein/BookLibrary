using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Infrastructure;

/// <summary>
/// Domain events dispatcher.
/// </summary>
/// <typeparam name="TDbContext">DbContext type.</typeparam>
public interface IDomainEventDispatcher<TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    /// Sets DbContext.
    /// </summary>
    /// <param name="dbContext">DbContext.</param>
    void SetDbContext(TDbContext dbContext);

    /// <summary>
    /// Dispatches domain events.
    /// </summary>
    /// <param name="ct">Token for cancel operation.</param>
    Task DispatchDomainEventsAsync(CancellationToken ct = default);
}