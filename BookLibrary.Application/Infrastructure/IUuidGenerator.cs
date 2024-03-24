namespace BookLibrary.Application.Infrastructure;

/// <summary>
/// Uuid Generator.
/// </summary>
public interface IUuidGenerator
{
    /// <summary>
    /// Get new one.
    /// </summary>
    /// <returns>Generated uuid.</returns>
    Guid GenerateNew();
}