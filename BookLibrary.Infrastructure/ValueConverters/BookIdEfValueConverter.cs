using BookLibrary.Domain.Aggregates.Books;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Infrastructure.ValueConverters;

/// <summary>
/// Converts from <see cref="BookId"/> to <see cref="Guid"/>.
/// </summary>
[UsedImplicitly]
[ExcludeFromCodeCoverage]
internal sealed class BookIdEfValueConverter : ValueConverter<BookId, Guid>
{
    public BookIdEfValueConverter(ConverterMappingHints? mappingHints = null)
        : base(id => id.Value, value => new BookId(value), mappingHints)
    {
    }
}