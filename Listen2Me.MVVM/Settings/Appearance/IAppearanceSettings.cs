using Listen2Me.MVVM.Settings.Appearance.Themes;

namespace Listen2Me.MVVM.Settings.Appearance;

/// <summary>
/// Entry point for settings related to appearance.
/// </summary>
public interface IAppearanceSettings
{
    /// <inheritdoc cref="IThemeManager"/>
    public IThemeManager ThemeManager { get; set; }
}