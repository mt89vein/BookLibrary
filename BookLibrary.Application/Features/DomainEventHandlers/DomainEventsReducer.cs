using BookLibrary.Domain.Aggregates.Books;
using JetBrains.Annotations;
using Seedwork;

namespace BookLibrary.Application.Features.DomainEventHandlers;

/// <summary>
/// Reduces domain events.
/// </summary>
public interface IDomainEventsReducer
{
    /// <summary>
    /// Reduce domain events.
    /// </summary>
    /// <param name="domainEvents">Domain events.</param>
    /// <returns>Reduced domain events.</returns>
    IEnumerable<IDomainEvent> Reduce(IReadOnlyCollection<IDomainEvent> domainEvents);
}

/// <summary>
/// Reduces domain events.
/// </summary>
[UsedImplicitly]
internal sealed class DomainEventsReducer : IDomainEventsReducer
{
    /// <summary>
    /// Reduce domain events.
    /// </summary>
    /// <param name="domainEvents">Domain events.</param>
    /// <returns>Reduced domain events.</returns>
    public IEnumerable<IDomainEvent> Reduce(IReadOnlyCollection<IDomainEvent> domainEvents)
    {
        // nothing to reduce
        if (domainEvents.Count == 1)
        {
            return domainEvents;
        }

        return ReduceImpl(domainEvents);

        static IEnumerable<IDomainEvent> ReduceImpl(IReadOnlyCollection<IDomainEvent> domainEvents)
        {
            var bookCreatedEvents = new List<BookCreatedEvent>(domainEvents.Count);
            foreach (var domainEvent in domainEvents)
            {
                if (domainEvent is BookCreatedEvent bookCreatedEvent)
                {
                    bookCreatedEvents.Add(bookCreatedEvent);
                }
                else
                {
                    yield return domainEvent;
                }
            }

            var groups = bookCreatedEvents.GroupBy(x => new { x.Title, x.Isbn, x.PublicationDate });

            foreach (var group in groups)
            {
                yield return new BookCreatedEvent(group.Key.Title, group.Key.Isbn, group.Key.PublicationDate, group.Count());
            }
        }
    }

}