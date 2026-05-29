using System.Windows;
using Listen2Me.MVVM.ViewModels.Shells;

namespace Listen2Me.WPF.Views.Shells;

public partial class MainShell : Window
{
    public MainShell(MainShellViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}