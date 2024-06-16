using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Domain.Aggregates.Abonents;

/// <summary>
/// Abonent identifier.
/// </summary>
[StronglyTypedId(generateJsonConverter: false)]
[SuppressMessage("Design", "CA1036")]
public partial struct AbonentId;