namespace Listen2Me.MVVM.Settings.Appearance.Themes;

/// <summary>
/// Provides settings for themes and accents.
/// </summary>
public interface IThemeManager
{
    void SetTheme(Themes theme);
    void SetAccent(Accents accent);
}