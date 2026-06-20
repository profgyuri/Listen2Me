using Listen2Me.MVVM.Settings.Appearance;
using Listen2Me.MVVM.Settings.Storage;

namespace Listen2Me.MVVM.Settings;

/// <inheritdoc cref="ISettings"/>
public class Settings : ISettings
{
    public Settings(IAppearanceSettings appearance, IStorageSettings storage)
    {
        Appearance = appearance;
        Storage = storage;
    }

    /// <inheritdoc cref="IAppearanceSettings"/>
    public IAppearanceSettings Appearance { get; set; }
    
    /// <inheritdoc cref="IStorageSettings"/>
    public IStorageSettings Storage { get; set; }
}