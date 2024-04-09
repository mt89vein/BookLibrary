namespace BookLibrary.Application.Infrastructure;

/// <summary>
/// Metric collector.
/// </summary>
public interface IMetricCollector
{
    /// <summary>
    /// Increment counter of created books.
    /// </summary>
    /// <param name="count">Count of created books.</param>
    void BooksCreated(int count);

    /// <summary>
    /// Increment counter of borrowed books.
    /// </summary>
    void BookBorrowed();

    /// <summary>
    /// Increment counter of returned books.
    /// </summary>
    void BookReturned();

    /// <summary>
    /// Increment counter of registered abonents.
    /// </summary>
    void AbonentRegistered();
}