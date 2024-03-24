using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace BookLibrary.Infrastructure.Configurations;

/// <summary>
/// Helper methods for EF.
/// </summary>
public static class EntityFrameworkUtils
{
    /// <summary>
    /// JSON serialization.
    /// </summary>
    /// <typeparam name="T">Type of property.</typeparam>
    /// <param name="propertyBuilder">Property configurator.</param>
    /// <returns>Property configurator.</returns>
    public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder)
    {
        ArgumentNullException.ThrowIfNull(propertyBuilder);

        var converter = new ValueConverter<T, string>(
            input => Serialize(input),
            output => Deserialize<T>(output));

        var comparer = new ValueComparer<T>(
            (l, r) => Serialize(l) == Serialize(r),
            v => v == null ? 0 : Serialize(v).GetHashCode(),
            v => Deserialize<T>(Serialize(v)));

        propertyBuilder.HasConversion(converter);
        propertyBuilder.Metadata.SetValueConverter(converter);
        propertyBuilder.Metadata.SetValueComparer(comparer);
        propertyBuilder.HasColumnType("jsonb");

        return propertyBuilder;
    }

    private static string Serialize<T>(T t)
    {
        return JsonSerializer.Serialize(t, JsonSerializerOptions.Default);
    }

    private static T Deserialize<T>(string t)
    {
        return JsonSerializer.Deserialize<T>(t, JsonSerializerOptions.Default)!;
    }
}