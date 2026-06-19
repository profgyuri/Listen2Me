using Listen2Me.MVVM.Settings.Appearance;

namespace Listen2Me.MVVM.Settings;

/// <inheritdoc cref="ISettings"/>
public class Settings : ISettings
{
    public Settings(IAppearanceSettings appearance)
    {
        Appearance = appearance;
    }

    /// <inheritdoc cref="IAppearanceSettings"/>
    public IAppearanceSettings Appearance { get; set; }
}