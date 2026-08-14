using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Shells;

public partial class TagEditorFormulaViewModel : DialogViewModelBase<bool>
{
    [ObservableProperty] private string _fileName = string.Empty;
    [ObservableProperty] private string _formula = string.Empty;
    [ObservableProperty] private Dictionary<string, string> _readTags = new();
    
    public TagEditorFormulaViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger) 
        : base(errorHandler, logger, messenger)
    {
    }

    [RelayCommand]
    private async Task Ok()
    {
        Result = true;
    }
    
    [RelayCommand]
    private async Task Cancel()
    {
        Result = false;
    }
}