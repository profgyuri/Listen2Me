using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Listen2Me.WPF.Views.Components;

[ContentProperty(nameof(Items))]
public partial class NavBar : UserControl
{
    public ObservableCollection<NavBarItem> Items { get; } = [];
    
    public NavBar()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty CurrentPathProperty =
        DependencyProperty.RegisterAttached(
            "CurrentPath",
            typeof(string),
            typeof(NavBar),
            new PropertyMetadata(null));
}