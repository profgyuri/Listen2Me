using System.Windows;
using System.Windows.Controls;
using Listen2Me.MVVM.ViewModels.Tabs.Settings;

namespace Listen2Me.WPF.Views.Tabs.Settings;

public partial class StorageTab : UserControl
{
    public StorageTab()
    {
        InitializeComponent();
    }
    
    private void AppDataButton_OnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException("Open listen2me folder in app data folder");
    }

    private async void TestConnButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is not StorageTabViewModel vm)
        {
            throw new InvalidOperationException("DataContext is not of type GeneralTabViewModel");
        }
        
        await vm.TestConnectionCommand.ExecuteAsync(PasswordBox.Password);
    }
}