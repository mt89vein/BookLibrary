using BookLibrary.Domain.Aggregates.Books;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Infrastructure.Configurations;

/// <summary>
/// Configuration of <see cref="Book"/> entity.
/// </summary>
[UsedImplicitly]
[ExcludeFromCodeCoverage]
internal sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.Isbn, x.PublicationDate });

        builder.Property(x => x.Authors).HasJsonConversion();
        builder.OwnsOne(x => x.BorrowInfo, y =>
        {
            y.Property(x => x.AbonentId).HasColumnName("borrowed_by_abonent_id");
            y.Property(x => x.BorrowedAt).HasColumnName("borrowed_at");
            y.Property(x => x.ReturnBefore).HasColumnName("borrowed_return_before");
        });
    }
}