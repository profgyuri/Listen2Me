using System.Windows;
using Listen2Me.MVVM.Settings.Appearance.Themes;
using Serilog;

namespace Listen2Me.WPF.Styles.Themes;

public class SimplifiedThemeManager : IThemeManager
{
    private readonly ILogger _logger;
    
    private static readonly Dictionary<MVVM.Settings.Appearance.Themes.Themes, Uri> ThemeUris = new()
    {
        [MVVM.Settings.Appearance.Themes.Themes.Light] = new Uri("pack://application:,,,/Styles/Themes/Light.xaml"),
        [MVVM.Settings.Appearance.Themes.Themes.Dark] = new Uri("pack://application:,,,/Styles/Themes/Dark.xaml"),
    };

    private static readonly Dictionary<Accents, Uri> AccentUris = new()
    {
        [Accents.Blue] = new Uri("pack://application:,,,/Styles/Themes/Accent.Blue.xaml"),
        [Accents.Green] = new Uri("pack://application:,,,/Styles/Themes/Accent.Green.xaml"),
        [Accents.Orange] = new Uri("pack://application:,,,/Styles/Themes/Accent.Orange.xaml"),
        [Accents.Purple] = new Uri("pack://application:,,,/Styles/Themes/Accent.Purple.xaml"),
        [Accents.Red] = new Uri("pack://application:,,,/Styles/Themes/Accent.Red.xaml"),
    };

    private readonly ResourceDictionary _appResources;

    public SimplifiedThemeManager(ILogger logger)
    {
        _logger = logger;
        _appResources = Application.Current.Resources;
    }

    public void SetTheme(MVVM.Settings.Appearance.Themes.Themes theme)
    {
        var newDict = new ResourceDictionary { Source = ThemeUris[theme] };
        _appResources.MergedDictionaries.RemoveAt(2);
        _appResources.MergedDictionaries.Insert(2, newDict);
    }

    public void SetAccent(Accents accent)
    {
        var newDict = new ResourceDictionary { Source = AccentUris[accent] };
        _appResources.MergedDictionaries.RemoveAt(3);
        _appResources.MergedDictionaries.Insert(3, newDict);
    }
}