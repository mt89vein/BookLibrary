using BookLibrary.Application.Infrastructure;
using UUIDNext;

namespace BookLibrary.Infrastructure;

/// <summary>
/// Uuid Generator.
/// </summary>
internal sealed class UuidGenerator : IUuidGenerator
{
    /// <summary>
    /// Get new one.
    /// </summary>
    /// <returns>Generated uuid.</returns>
    public Guid GenerateNew()
    {
        return Uuid.NewSequential();
    }
}