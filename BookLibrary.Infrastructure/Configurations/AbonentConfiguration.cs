﻿using BookLibrary.Domain.Aggregates.Abonents;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Infrastructure.Configurations;

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