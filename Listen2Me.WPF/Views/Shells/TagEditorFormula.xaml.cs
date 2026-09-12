using System.Windows;
using Listen2Me.MVVM.ViewModels.Shells;

namespace Listen2Me.WPF.Views.Shells;

public partial class TagEditorFormula : Window
{
    public TagEditorFormula(TagEditorFormulaViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }

    private async void OkButton_OnClick(object sender, RoutedEventArgs e)
    {
        await ((TagEditorFormulaViewModel)DataContext).OkCommand.ExecuteAsync(null);
        DialogResult = true;
        Close();
    }
    
    private async void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        await ((TagEditorFormulaViewModel)DataContext).CancelCommand.ExecuteAsync(null);
        DialogResult = false;
        Close();
    }
}