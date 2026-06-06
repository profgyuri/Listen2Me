using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Messages;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Components;

public partial class NavBarViewModel : ViewModelBase
{
    public NavBarViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger) 
        : base(errorHandler, logger, messenger)
    { }

    [RelayCommand]
    private void NavigateTo(string path)
    {
        Messenger.Send(new NavBarNavigationMessage(path));
        Logger.Information("Navigating to {Path}...", path);
    }
}