using BookLibrary.Infrastructure.Books;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Infrastructure.Configurations;

/// <summary>
/// Configuration of <see cref="BookStatChange"/> entity.
/// </summary>
[UsedImplicitly]
[ExcludeFromCodeCoverage]
internal sealed class BookStatChangesConfiguration : IEntityTypeConfiguration<BookStatChange>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<BookStatChange> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasKey(x => x.Id);
    }
}