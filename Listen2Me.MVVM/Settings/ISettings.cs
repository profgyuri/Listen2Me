using Listen2Me.MVVM.Settings.Appearance;
using Listen2Me.MVVM.Settings.Storage;

namespace Listen2Me.MVVM.Settings;

/// <summary>
/// God interface for all settings.
/// </summary>
public interface ISettings
{
    /// <inheritdoc cref="AppearanceSettings"/>
    public AppearanceSettings Appearance { get; set; }
    
    /// <inheritdoc cref="StorageSettings"/>
    public StorageSettings Storage { get; set; }
    
    /// <summary>
    /// Saves all settings asynchronously.
    /// </summary>
    Task SaveAsync(CancellationToken ct = default);

    /// <summary>
    /// Loads all settings asynchronously.
    /// </summary>
    Task LoadAsync(CancellationToken ct = default);
}