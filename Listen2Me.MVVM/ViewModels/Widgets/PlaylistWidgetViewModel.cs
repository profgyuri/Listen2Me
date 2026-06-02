using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Widgets;

public class PlaylistWidgetViewModel : WidgetViewModel
{
    public PlaylistWidgetViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger) 
        : base(errorHandler, logger, messenger)
    {
        Order = 3;
        Name = "Playlist";
    }
}