using Listen2Me.MVVM.Settings.Appearance.Themes;

namespace Listen2Me.MVVM.Settings.Appearance;

/// <summary>
/// Holds settings for the appearance of the application.
/// </summary>
public class AppearanceSettings : JsonSettingsMemory
{
    public AppearanceSettings(IThemeManager themeManager)
    {
        ThemeManager = themeManager;
    }

    /// <inheritdoc cref="IThemeManager"/>
    public IThemeManager ThemeManager { get; set; }

    protected override string FileName => "appearance.json";
}