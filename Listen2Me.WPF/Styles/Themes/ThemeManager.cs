using System.Windows;
using Listen2Me.MVVM.Settings.Appearance.Themes;

namespace Listen2Me.WPF.Styles.Themes;

public class ThemeManager : IThemeManager
{
    private static readonly Dictionary<MVVM.Settings.Appearance.Themes.Themes, Uri> ThemeUris = new()
    {
        [MVVM.Settings.Appearance.Themes.Themes.Light] = new Uri("pack://application:,,,/Styles/Themes/Light.xaml"),
        [MVVM.Settings.Appearance.Themes.Themes.Dark] = new Uri("pack://application:,,,/Styles/Themes/Dark.xaml"),
    };

    private static readonly Dictionary<Accents, Uri> AccentUris = new()
    {
        [Accents.Blue] = new Uri("pack://application:,,,/Styles/Themes/Accent.Blue.xaml"),
        [Accents.Green] = new Uri("pack://application:,,,/Styles/Themes/Accent.Green.xaml"),
        [Accents.Purple] = new Uri("pack://application:,,,/Styles/Themes/Accent.Purple.xaml"),
    };
    
    private MVVM.Settings.Appearance.Themes.Themes _currentTheme = MVVM.Settings.Appearance.Themes.Themes.Light;
    private Accents _currentAccent = Accents.Blue;
    
    public void SetTheme(MVVM.Settings.Appearance.Themes.Themes theme)
    {
        SwapDictionary(ThemeUris[_currentTheme], ThemeUris[theme]);
        _currentTheme = theme;
    }

    public void SetAccent(Accents accent)
    {
        SwapDictionary(AccentUris[_currentAccent], AccentUris[accent]);
        _currentAccent = accent;
    }
    
    private static void SwapDictionary(Uri currentUri, Uri newUri)
    {
        if (currentUri == newUri) return;

        var mergedDicts = Application.Current.Resources.MergedDictionaries;

        // get the theme dictionary's inner merged dicts
        var themeDictionary = mergedDicts.FirstOrDefault(d => d.Source != null &&
                                                              d.Source.OriginalString.Contains("ThemeDictionary"));

        var target = themeDictionary?.MergedDictionaries ?? mergedDicts;

        var existing = target.FirstOrDefault(
            d => currentUri.OriginalString.EndsWith(d.Source?.OriginalString!));
        if (existing is null) return;

        var index = target.IndexOf(existing);
        target[index] = new ResourceDictionary { Source = newUri };
    }
}