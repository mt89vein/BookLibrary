using BookLibrary.Domain.Aggregates.Books;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Infrastructure.ValueConverters;

/// <summary>
/// Converts from <see cref="BookTitle"/> to <see cref="string"/>.
/// </summary>
[UsedImplicitly]
[ExcludeFromCodeCoverage]
internal sealed class BookTitleEfValueConverter : ValueConverter<BookTitle, string>
{
    public BookTitleEfValueConverter(ConverterMappingHints? mappingHints = null)
        : base(id => id.Value, value => new BookTitle(value), mappingHints)
    {
    }
}