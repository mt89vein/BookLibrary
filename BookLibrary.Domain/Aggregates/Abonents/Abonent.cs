using BookLibrary.Domain.Aggregates.Abonents.Events;
using BookLibrary.Domain.Exceptions;
using BookLibrary.Domain.ValueObjects;
using Seedwork;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Domain.Aggregates.Abonents;

/// <summary>
/// Abonent entity, that can borrow books in library.
/// </summary>
public sealed class Abonent : Entity
{
    /// <summary>
    /// Abonent identifier.
    /// </summary>
    public AbonentId Id { get; private set; }

    /// <summary>
    /// Abonent name.
    /// </summary>
    public AbonentName Name { get; private set; }

    /// <summary>
    /// Email.
    /// </summary>
    public Email Email { get; private set; }

    /// <summary>
    /// Abonent creation date time.
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Abonent"></see> class.
    /// </summary>
    /// <param name="id">Abonent identifier.</param>
    /// <param name="name">Abonent name.</param>
    /// <param name="email">Email.</param>
    /// <param name="createdAt">When abonent created.</param>
    public Abonent(
        AbonentId id,
        AbonentName name,
        Email email,
        DateTimeOffset createdAt
    )
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(email);

        Id = id != default ? id : throw ErrorCodes.InvalidAbonentId.ToException();
        Name = name;
        Email = email;
        CreatedAt = createdAt;

        AddDomainEvent(new AbonentRegistredEvent(Id));
    }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    /// <summary>
    /// Constructor for EF.
    /// </summary>
    [Obsolete("Constructor only for EFCore, because it needs to ctor without parameters", error: true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [ExcludeFromCodeCoverage]
    private Abonent()
    {
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
}

// how can i implement subscriptions?

// abonent can set isbn and optionally year of the book
// when book returned, we eventually check is there any abonent that subscribed to the availability of this book or not
// if so, we mark that subscription done, and never notify again (until subscription created again)
// or we can set datetime when we notified, and we won't notify more than once a week (when book returned)