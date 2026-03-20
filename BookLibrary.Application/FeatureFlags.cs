namespace BookLibrary.Application;

/// <summary>
/// Application feature flags.
/// </summary>
public static class FeatureFlags
{
    /// <summary>
    /// Is allowed to automatically apply migration on application startup.
    /// </summary>
    public const string AUTO_MIGRATIONS_ENABLED = "AutoMigrationsEnabled";
}