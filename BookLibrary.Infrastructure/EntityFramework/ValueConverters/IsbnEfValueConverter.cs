using BookLibrary.Domain.ValueObjects;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Infrastructure.ValueConverters;

/// <summary>
/// Converts from <see cref="Isbn"/> to <see cref="string"/>.
/// </summary>
[UsedImplicitly]
[ExcludeFromCodeCoverage]
internal sealed class IsbnEfValueConverter : ValueConverter<Isbn, string>
{
    public IsbnEfValueConverter(ConverterMappingHints? mappingHints = null)
        : base(id => id.Value, value => new Isbn(value), mappingHints)
    {
    }
}