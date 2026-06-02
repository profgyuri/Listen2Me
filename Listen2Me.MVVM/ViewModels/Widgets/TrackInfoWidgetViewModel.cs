using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Widgets;

public class TrackInfoWidgetViewModel : WidgetViewModel
{
    public TrackInfoWidgetViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger) 
        : base(errorHandler, logger, messenger)
    {
        Order = 0;
        Name = "Track Info";
    }
}