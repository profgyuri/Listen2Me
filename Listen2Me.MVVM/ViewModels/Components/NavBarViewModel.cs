using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Messages;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Components;

public partial class NavBarViewModel : ViewModelBase
{
    [ObservableProperty] private string _currentPath = "home";
    
    public NavBarViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger) 
        : base(errorHandler, logger, messenger)
    { }

    [RelayCommand]
    private void NavigateTo(string path)
    {
        Logger.Information("Navigating to {Path}...", path);
        CurrentPath = path;
        Messenger.Send(new NavBarNavigationMessage(path));
    }

    [RelayCommand]
    private void Close()
    {
        Application.Current.Shutdown();
    }
}