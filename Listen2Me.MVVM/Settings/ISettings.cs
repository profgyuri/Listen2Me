using Listen2Me.MVVM.Settings.Appearance;
using Listen2Me.MVVM.Settings.Storage;

namespace Listen2Me.MVVM.Settings;

/// <summary>
/// God interface for all settings.
/// </summary>
public interface ISettings
{
    /// <inheritdoc cref="IAppearanceSettings"/>
    public IAppearanceSettings Appearance { get; set; }
    
    /// <inheritdoc cref="IStorageSettings"/>
    public IStorageSettings Storage { get; set; }
}