using System.Windows;
using System.Windows.Controls;

namespace Listen2Me.WPF.Views.Components;

public partial class TitleBar : UserControl
{
    public TitleBar()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty HideButtonsProperty = DependencyProperty.Register(
        nameof(HideButtons), typeof(bool), typeof(TitleBar), new PropertyMetadata(default(bool)));

    public bool HideButtons
    {
        get => (bool)GetValue(HideButtonsProperty);
        set => SetValue(HideButtonsProperty, value);
    }
}