namespace BookLibrary.Domain.Aggregates.Books;

/// <summary>
/// Book statistics.
/// </summary>
public sealed class BookStat
{
    /// <summary>
    /// ISBN.
    /// </summary>
    public string Isbn { get; set; } = null!;

    /// <summary>
    /// Year when book was published.
    /// </summary>
    public DateOnly PublicationDate { get; set; }

    /// <summary>
    /// Book title.
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Book authors.
    /// </summary>
    public string Authors { get; set; } = null!;

    /// <summary>
    /// How many books available for borrow.
    /// </summary>
    public int AvailableCount { get; set; }

    /// <summary>
    /// How many books borrowed.
    /// </summary>
    public int BorrowedCount { get; set; }
}