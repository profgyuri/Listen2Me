using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Listen2Me.MVVM.Persistence.Entities;
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

    private void ListViewItem_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        var vm = (FolderBrowserDialogViewModel)DataContext;
        if (sender is ListViewItem { DataContext: Bookmark item } && vm.NavigateToCommand?.CanExecute(item.Path) == true)
        {
            vm.NavigateToCommand.Execute(item.Path);
        }
    }
}