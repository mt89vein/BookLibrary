using Mediator;
using System.Diagnostics.CodeAnalysis;

namespace Seedwork;

/// <summary>
/// Domain event.
/// </summary>
[SuppressMessage("Design", "CA1040:Do not use marker interfaces")]
public interface IDomainEvent : INotification;