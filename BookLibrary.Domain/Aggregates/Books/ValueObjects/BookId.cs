using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Domain.Aggregates.Books;

[StronglyTypedId(generateJsonConverter: false)]
[SuppressMessage("Design", "CA1036")]
public partial struct BookId;