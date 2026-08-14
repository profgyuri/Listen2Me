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

    private void OkButton_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        ((TagEditorFormulaViewModel)DataContext).OkCommand.Execute(null);
        Close();
    }
    
    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        ((TagEditorFormulaViewModel)DataContext).CancelCommand.Execute(null);
        Close();
    }
}