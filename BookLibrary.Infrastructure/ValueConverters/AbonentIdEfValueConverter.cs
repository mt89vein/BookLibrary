using BookLibrary.Domain.Aggregates.Abonents;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Infrastructure.ValueConverters;

/// <summary>
/// Converts from <see cref="AbonentId"/> to <see cref="Guid"/>.
/// </summary>
[UsedImplicitly]
[ExcludeFromCodeCoverage]
internal sealed class AbonentIdEfValueConverter : ValueConverter<AbonentId, Guid>
{
    public AbonentIdEfValueConverter(ConverterMappingHints? mappingHints = null)
        : base(id => id.Value, value => new AbonentId(value), mappingHints)
    {
    }
}