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

    private void SelectButton_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        ((FolderBrowserDialogViewModel)DataContext).SelectCommand.Execute(null);
        Close();
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        ((FolderBrowserDialogViewModel)DataContext).CancelCommand.Execute(null);
        Close();
    }
}