using BookLibrary.Application.Infrastructure;
using BookLibrary.Domain.Aggregates.Abonents;
using BookLibrary.Domain.Aggregates.Books;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Infrastructure;

/// <summary>
/// Application context.
/// </summary>
internal sealed class ApplicationContext : DbContext, IApplicationContext
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
    /// Abonents.
    /// </summary>
    public DbSet<Abonent> Abonents { get; set; } = null!;

    /// <summary>
    /// Books.
    /// </summary>
    public DbSet<Book> Books { get; set; } = null!;

    /// <summary>
    /// Creates new instance of <see cref="ApplicationContext"/>.
    /// </summary>
    /// <param name="options">Options.</param>
    /// <param name="domainEventDispatcher">Domain events dispatcher.</param>
    public ApplicationContext(
        DbContextOptions<ApplicationContext> options,
        IDomainEventDispatcher<ApplicationContext> domainEventDispatcher
    ) : base(options)
    {
        _domainEventDispatcher = domainEventDispatcher;
        _domainEventDispatcher.SetDbContext(this);
    }

    /// <summary>
    /// Configures model.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.HasDefaultSchema(DefaultScheme);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_domainEventDispatcher is null)
        {
            throw new InvalidOperationException("DomainEventDispatcher was null");
        }

        await _domainEventDispatcher.DispatchDomainEventsAsync(cancellationToken);

        return await base.SaveChangesAsync(cancellationToken);
    }
}