using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Infrastructure.ValueConverters;

/// <summary>
/// ValueObject converter selector by convention.
/// </summary>
[UsedImplicitly]
[ExcludeFromCodeCoverage]
internal sealed class ValueObjectsConverterSelectorByConvention : ValueConverterSelector
{
    /// <summary>
    /// Converters cache.
    /// </summary>
    private readonly ConcurrentDictionary<(Type ModelClrType, Type ProviderClrType), ValueConverterInfo> _converters =
        new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="ValueObjectsConverterSelectorByConvention" /> class.
    /// </summary>
    /// <param name="dependencies">Parameter object containing dependencies for this service.</param>
    public ValueObjectsConverterSelectorByConvention(ValueConverterSelectorDependencies dependencies)
        : base(dependencies)
    {
    }

    /// <summary>
    ///     Returns the list of <see cref="Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter" /> instances that can be
    ///     used to convert the given model type. Converters nearer the front of
    ///     the list should be used in preference to converters nearer the end.
    /// </summary>
    /// <param name="modelClrType">The type for which a converter is needed.</param>
    /// <param name="providerClrType">The database provider type to target, or null for any.</param>
    /// <returns>The converters available.</returns>
    public override IEnumerable<ValueConverterInfo> Select(Type modelClrType, Type? providerClrType = null)
    {
        var baseConverters = base.Select(modelClrType, providerClrType);
        foreach (var converter in baseConverters)
        {
            yield return converter;
        }

        // get T from Nullable<T>
        var underlyingModelType = UnwrapNullableType(modelClrType);
        var underlyingProviderType = UnwrapNullableType(providerClrType);

        // 'null' means 'get any value converters for the modelClrType'
        if (underlyingProviderType is null && underlyingModelType is not null)
        {
            var converterType = Type.GetType("BookLibrary.Infrastructure.ValueConverters." + underlyingModelType.Name +
                                             "EfValueConverter");

            if (converterType is not null)
            {
                // get T2 from ValueConvert<T1, T2>
                var targetType = converterType.BaseType!.GetGenericArguments()[1];

                yield return _converters.GetOrAdd((underlyingModelType, targetType),
                    k =>
                    {
                        Func<ValueConverterInfo, ValueConverter> converterFactory =
                            info => (ValueConverter)Activator.CreateInstance(converterType, info.MappingHints)!;

                        return new ValueConverterInfo(modelClrType, targetType, converterFactory);
                    }
                );
            }
        }
    }

    private static Type? UnwrapNullableType(Type? type)
    {
        if (type is null)
        {
            return null;
        }

        return Nullable.GetUnderlyingType(type) ?? type;
    }
}