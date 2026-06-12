using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace Listen2Me.WPF.Views.Components;

public partial class NavBarItem : UserControl
{
    public NavBarItem()
    {
        InitializeComponent();
    }
    
    public PackIconKind Icon
    {
        get => (PackIconKind)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
    
    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }
    
    #region Icon Attached Property
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.RegisterAttached(
            "Icon",
            typeof(PackIconKind),
            typeof(NavBarItem),
            new PropertyMetadata(default(PackIconKind)));
    
    public static PackIconKind GetIcon(DependencyObject obj)
        => (PackIconKind)obj.GetValue(IconProperty);
    
    public static void SetIcon(DependencyObject obj, PackIconKind value)
        => obj.SetValue(IconProperty, value);
    #endregion
    
    #region Text Attached Property
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.RegisterAttached(
            "Text",
            typeof(string),
            typeof(NavBarItem),
            new PropertyMetadata(default(string)));
    
    public static string GetText(DependencyObject obj)
        => (string)obj.GetValue(TextProperty);
    
    public static void SetText(DependencyObject obj, string value)
        => obj.SetValue(TextProperty, value);
    #endregion
    
    #region Command Attached Property
    public static readonly DependencyProperty CommandParameterProperty =
        DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(NavBarItem),
            new PropertyMetadata(null));

    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }
    
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(NavBarItem),
            new PropertyMetadata(null));
    #endregion
    
    #region IsActive Attached Property
    public static readonly DependencyProperty IsActiveProperty =
        DependencyProperty.RegisterAttached(
            "IsActive",
            typeof(bool),
            typeof(NavBarItem),
            new PropertyMetadata(default(bool)));
    #endregion
}