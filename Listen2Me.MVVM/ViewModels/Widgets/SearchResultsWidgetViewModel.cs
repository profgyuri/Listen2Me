using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Widgets;

public class SearchResultsWidgetViewModel : WidgetViewModel
{
    public SearchResultsWidgetViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger) 
        : base(errorHandler, logger, messenger)
    {
        Order = 2;
        Name = "Search Results";
    }
}