using BookLibrary.Domain.Aggregates.Abonents;
using BookLibrary.Domain.Aggregates.Books;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Application.Infrastructure;

/// <summary>
/// Application context.
/// </summary>
public interface IApplicationContext : IDbSets
{
    /// <summary>
    /// Save changes.
    /// </summary>
    /// <param name="ct">Token for cancel operation.</param>
    /// <returns>Count of changes.</returns>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

public interface IDbSets : IDisposable
{
    /// <summary>
    /// Abonents.
    /// </summary>
    DbSet<Abonent> Abonents { get; }

    /// <summary>
    /// Books.
    /// </summary>
    DbSet<Book> Books { get; }

    /// <summary>
    /// Books statistics.
    /// </summary>
    DbSet<BookStat> BookStats { get; }
}