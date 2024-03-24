using BookLibrary.Domain.Exceptions;
using Seedwork;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace BookLibrary.Domain.Aggregates.Abonents;

/// <summary>
/// Abonent name.
/// </summary>
public sealed class AbonentName : ValueObject
{
    /// <summary>
    /// First name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Last name.
    /// </summary>
    public string Surname { get; private set; }

    /// <summary>
    /// Middle name.
    /// </summary>
    public string? Patronymic { get; private set; }

    /// <summary>
    /// Creates new instance of <see cref="AbonentName"/>.
    /// </summary>
    /// <param name="name">First name.</param>
    /// <param name="surname">Last name.</param>
    /// <param name="patronymic">Middle name.</param>
    /// <exception cref="BookLibraryException">
    /// When <paramref name="name"/> or <paramref name="surname"/> not set.
    /// </exception>
    public AbonentName(string name, string surname, string? patronymic = null)
    {
        Name = !string.IsNullOrWhiteSpace(name)
            ? name
            : throw ErrorCodes.InvalidAbonentName.ToException();

        Surname = !string.IsNullOrWhiteSpace(surname)
            ? surname
            : throw ErrorCodes.InvalidAbonentSurname.ToException();

        Patronymic = patronymic;
    }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    /// <summary>
    /// Constructor for EF.
    /// </summary>
    [Obsolete("Constructor only for EFCore, because it needs to ctor without parameters", error: true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [ExcludeFromCodeCoverage]
    private AbonentName()
    {
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

    public override string ToString()
    {
        var p = Patronymic is null
            ? string.Empty
            : $" {Patronymic}";

        return $"{Surname} {Name}" + p;

    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Surname;
        yield return Name;
        yield return Patronymic;
    }
}