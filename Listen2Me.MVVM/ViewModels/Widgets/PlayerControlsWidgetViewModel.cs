using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Widgets;

public class PlayerControlsWidgetViewModel : WidgetViewModel
{
    public PlayerControlsWidgetViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger) 
        : base(errorHandler, logger, messenger)
    {
        Order = 1;
        Name = "Player Controls";
    }
}