using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Serilog;

namespace Listen2Me.MVVM.ViewModels;

public class WidgetViewModel : ViewModelBase
{
    public string Name { get; set; }
    
    /// <summary>
    /// Order of the widget in the UI.
    /// </summary>
    public int Order { get; set; }
    
    public WidgetViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger) 
        : base(errorHandler, logger, messenger)
    {
    }
}