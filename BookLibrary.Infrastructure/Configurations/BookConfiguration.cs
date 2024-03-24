using BookLibrary.Domain.Aggregates.Abonents;
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

/// <summary>
/// Configuration of <see cref="Abonent"/> entity.
/// </summary>
[UsedImplicitly]
[ExcludeFromCodeCoverage]
internal sealed class AbonentConfiguration : IEntityTypeConfiguration<Abonent>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Abonent> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Email).IsUnique();
        builder.OwnsOne(x => x.Name, y =>
        {
            y.Property(x => x.Name).HasColumnName("name");
            y.Property(x => x.Surname).HasColumnName("surname");
            y.Property(x => x.Patronymic).HasColumnName("patronymic");
        });
    }
}