using BookLibrary.Domain.ValueObjects;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Infrastructure.ValueConverters;

/// <summary>
/// Converts from <see cref="Email"/> to <see cref="string"/>.
/// </summary>
[UsedImplicitly]
[ExcludeFromCodeCoverage]
internal sealed class EmailEfValueConverter : ValueConverter<Email, string>
{
    public EmailEfValueConverter(ConverterMappingHints? mappingHints = null)
        : base(id => id.Value, value => new Email(value), mappingHints)
    {
    }
}