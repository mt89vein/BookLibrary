using BookLibrary.Domain.Aggregates.Abonents;
using Seedwork;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Domain.Aggregates.Books;

/// <summary>
/// Book borrowing info.
/// </summary>
public sealed class BorrowInfo : ValueObject
{
    /// <summary>
    /// Abonent identifier.
    /// </summary>
    public AbonentId AbonentId { get; private set; }

    /// <summary>
    /// DateTime when book was borrowed.
    /// </summary>
    public DateTimeOffset BorrowedAt { get; private set; }

    /// <summary>
    /// DateTime when book must be returned.
    /// </summary>
    public DateTimeOffset ReturnBefore { get; private set; }

    /// <summary>
    /// Borrowed books by abonent.
    /// </summary>
    /// <param name="abonentId">Abonent identifier.</param>
    /// <param name="borrowedAt">DateTime when book was borrowed.</param>
    /// <param name="returnBefore">DateTime when book must be returned.</param>
    internal BorrowInfo(
        AbonentId abonentId,
        DateTimeOffset borrowedAt,
        DateTimeOffset returnBefore
    )
    {
        AbonentId = abonentId;
        BorrowedAt = borrowedAt;
        ReturnBefore = returnBefore;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return AbonentId.ToString();
        yield return BorrowedAt.ToString();
        yield return ReturnBefore.ToString();
    }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    /// <summary>
    /// Constructor for EF.
    /// </summary>
    [Obsolete("Constructor only for EFCore, because it needs to ctor without parameters", error: true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [ExcludeFromCodeCoverage]
    private BorrowInfo()
    {
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
}