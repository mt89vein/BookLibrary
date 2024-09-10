using BookLibrary.Application.Infrastructure;
using BookLibrary.Domain.Aggregates.Abonents;
using BookLibrary.Domain.Aggregates.Books;
using BookLibrary.Infrastructure.Books;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BookLibrary.Infrastructure;

/// <summary>
/// Application context.
/// </summary>
public sealed class ApplicationContext : DbContext, IApplicationContext
{
    /// <summary>
    /// Scheme name.
    /// </summary>
    public static string DefaultScheme => "book_library";

    /// <summary>
    /// Domain events dispatcher.
    /// </summary>
    private readonly IDomainEventDispatcher<ApplicationContext> _domainEventDispatcher;

    /// <summary>
    /// Mediator.
    /// </summary>
    private readonly IMediator _mediator;

    /// <summary>
    /// Abonents.
    /// </summary>
    public DbSet<Abonent> Abonents { get; set; } = null!;

    /// <summary>
    /// Books.
    /// </summary>
    public DbSet<Book> Books { get; set; } = null!;

    /// <summary>
    /// Books stats.
    /// </summary>
    public DbSet<BookStat> BookStats { get; set; } = null!;

    /// <summary>
    /// Book stat changes.
    /// </summary>
    public DbSet<BookStatChange> BookStatChanges { get; set; } = null!;

    /// <summary>
    /// Creates new instance of <see cref="ApplicationContext"/>.
    /// </summary>
    /// <param name="options">Options.</param>
    /// <param name="domainEventDispatcher">Domain events dispatcher.</param>
    /// <param name="mediator">Mediator.</param>
    public ApplicationContext(
        DbContextOptions<ApplicationContext> options,
        IDomainEventDispatcher<ApplicationContext> domainEventDispatcher,
        IMediator mediator
    ) : base(options)
    {
        _domainEventDispatcher = domainEventDispatcher;
        _mediator = mediator;
        _domainEventDispatcher.SetDbContext(this);
    }

    /// <summary>
    /// Configures model.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.HasPostgresExtension("pg_trgm");
        modelBuilder.HasDefaultSchema(DefaultScheme);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    /// <summary>
    /// Saves all changes that was made in this context to the database.
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_domainEventDispatcher is null)
        {
            throw new InvalidOperationException("DomainEventDispatcher was null");
        }

        await _domainEventDispatcher.DispatchDomainEventsAsync(cancellationToken);

        foreach (var entityEntry in ChangeTracker.Entries<Book>().ToArray())
        {
            await NotifyBookChangesAsync(entityEntry, cancellationToken);
        }

        foreach (var entityEntry in ChangeTracker.Entries<BorrowInfo>().ToArray())
        {
            await NotifyBookBorrowInfoChangesAsync(entityEntry, cancellationToken);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    private ValueTask NotifyBookChangesAsync(EntityEntry<Book> book, CancellationToken ct)
    {
        return book.State switch
        {
            EntityState.Added => _mediator.Publish(new BookCreated(book.Entity), ct),
            EntityState.Modified => _mediator.Publish(new BookUpdated(book.Entity), ct),
            EntityState.Deleted or EntityState.Detached or EntityState.Unchanged or _ => ValueTask.CompletedTask
        };
    }

    private ValueTask NotifyBookBorrowInfoChangesAsync(EntityEntry<BorrowInfo> borrowInfo, CancellationToken ct)
    {
        var bookId = borrowInfo.Property<BookId>("BookId");
        var book = Books.Local.FirstOrDefault(x => x.Id == bookId.CurrentValue)!;

        return borrowInfo.State switch
        {
            EntityState.Added => _mediator.Publish(new BookBorrowCreated(book), ct),
            EntityState.Deleted => _mediator.Publish(new BookBorrowDeleted(book), ct),
            EntityState.Detached or EntityState.Unchanged or EntityState.Modified or _ => ValueTask.CompletedTask
        };
    }
}