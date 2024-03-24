using BookLibrary.Domain.Aggregates.Books;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Infrastructure.ValueConverters;

/// <summary>
/// Converts from <see cref="BookPublicationDate"/> to <see cref="DateOnly"/>.
/// </summary>
[UsedImplicitly]
[ExcludeFromCodeCoverage]
internal sealed class BookPublicationDateEfValueConverter : ValueConverter<BookPublicationDate, DateOnly>
{
    public BookPublicationDateEfValueConverter(ConverterMappingHints? mappingHints = null)
        : base(id => id.Value, value => new BookPublicationDate(value), mappingHints)
    {
    }
}