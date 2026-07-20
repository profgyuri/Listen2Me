using System.Windows;
using Listen2Me.MVVM.ViewModels.Shells;

namespace Listen2Me.WPF.Views.Shells;

public partial class FolderBrowserDialog : Window
{
    public FolderBrowserDialog(FolderBrowserDialogViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
    
    private void OnCancel(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
    
    private void OnOk(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}