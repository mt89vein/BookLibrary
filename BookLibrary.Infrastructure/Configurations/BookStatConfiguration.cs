using BookLibrary.Domain.Aggregates.Books;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Infrastructure.Configurations;

/// <summary>
/// Configuration of <see cref="BookStat"/> entity.
/// </summary>
[UsedImplicitly]
[ExcludeFromCodeCoverage]
internal sealed class BookStatConfiguration : IEntityTypeConfiguration<BookStat>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<BookStat> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasKey(x => new { x.Isbn, x.PublicationDate });

        // index for double wildcard search using like/ilike "%{filter}%"
        builder.HasIndex(x => x.Authors)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");
        builder.HasIndex(x => x.Title)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");
    }
}