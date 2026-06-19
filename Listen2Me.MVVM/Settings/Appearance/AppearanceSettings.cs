using Listen2Me.MVVM.Settings.Appearance.Themes;

namespace Listen2Me.MVVM.Settings.Appearance;

/// <inheritdoc cref="IAppearanceSettings"/>
public class AppearanceSettings : IAppearanceSettings
{
    public AppearanceSettings(IThemeManager themeManager)
    {
        ThemeManager = themeManager;
    }

    /// <inheritdoc cref="IThemeManager"/>
    public IThemeManager ThemeManager { get; set; }
}