using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Listen2Me.WPF.Views.Components;

public partial class FontSettings : UserControl
{
    public FontSettings()
    {
        InitializeComponent();
    }
    
    //Font Family
    public IEnumerable<FontFamily> FontFamilySource
    {
        get => (IEnumerable<FontFamily>)GetValue(FontFamilySourceProperty);
        set => SetValue(FontFamilySourceProperty, value);
    }
    public static readonly DependencyProperty FontFamilySourceProperty =
        DependencyProperty.Register(nameof(FontFamilySource), typeof(IEnumerable<FontFamily>), typeof(FontSettings),
            new FrameworkPropertyMetadata(Fonts.SystemFontFamilies, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public FontFamily SelectedFontFamily
    {
        get => (FontFamily)GetValue(SelectedFontFamilyProperty);
        set => SetValue(SelectedFontFamilyProperty, value);
    }
    public static readonly DependencyProperty SelectedFontFamilyProperty =
        DependencyProperty.Register(nameof(SelectedFontFamily), typeof(FontFamily), typeof(FontSettings),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    //Font Size
    public IEnumerable<double> FontSizeSource
    {
        get => (IEnumerable<double>)GetValue(FontSizeSourceProperty);
        set => SetValue(FontSizeSourceProperty, value);
    }
    public static readonly DependencyProperty FontSizeSourceProperty =
        DependencyProperty.Register(nameof(FontSizeSource), typeof(IEnumerable<double>), typeof(FontSettings));

    public double SelectedFontSize
    {
        get => (double)GetValue(SelectedFontSizeProperty);
        set => SetValue(SelectedFontSizeProperty, value);
    }
    public static readonly DependencyProperty SelectedFontSizeProperty =
        DependencyProperty.Register(nameof(SelectedFontSize), typeof(double), typeof(FontSettings),
            new FrameworkPropertyMetadata(12.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    //Bold, Italic
    public bool IsBold
    {
        get => (bool)GetValue(IsBoldProperty);
        set => SetValue(IsBoldProperty, value);
    }
    public static readonly DependencyProperty IsBoldProperty =
        DependencyProperty.Register(nameof(IsBold), typeof(bool), typeof(FontSettings),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsBoldChanged));

    public bool IsItalic
    {
        get => (bool)GetValue(IsItalicProperty);
        set => SetValue(IsItalicProperty, value);
    }
    public static readonly DependencyProperty IsItalicProperty =
        DependencyProperty.Register(nameof(IsItalic), typeof(bool), typeof(FontSettings),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsItalicChanged));

    private static void OnIsBoldChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (FontSettings)d;
        control.SyncListBoxItem(control.BoldItem, (bool)e.NewValue);
    }

    private static void OnIsItalicChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (FontSettings)d;
        control.SyncListBoxItem(control.ItalicItem, (bool)e.NewValue);
    }

    private void SyncListBoxItem(ListBoxItem item, bool isSelected)
    {
        if (item.IsSelected == isSelected) return;

        item.IsSelected = isSelected;
    }

    private void FontStyleListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        IsBold = BoldItem.IsSelected;
        IsItalic = ItalicItem.IsSelected;
    }
}