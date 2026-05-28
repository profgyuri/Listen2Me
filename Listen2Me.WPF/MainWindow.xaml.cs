using System.Windows;
using Listen2Me.MVVM.ViewModels;

namespace Listen2Me.WPF;

/// <summary>
/// Hosts the shell navigation surface.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    /// <param name="shellViewModel">The shell view model instance.</param>
    public MainWindow(ShellViewModel shellViewModel)
    {
        InitializeComponent();
        DataContext = shellViewModel;
    }
}