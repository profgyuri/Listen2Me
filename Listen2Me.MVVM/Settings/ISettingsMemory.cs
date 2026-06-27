namespace Listen2Me.MVVM.Settings;

/// <summary>
/// Provides contract for saving settings.
/// </summary>
public interface ISettingsMemory
{
    /// <summary>
    /// Saves all theme-specific settings asynchronously.
    /// </summary>
    Task SaveAsync(CancellationToken ct = default);

    /// <summary>
    /// Loads all theme-specific settings asynchronously.
    /// </summary>
    Task LoadAsync(CancellationToken ct = default);
}