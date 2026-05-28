using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Serilog;
using Listen2Me.MVVM.ErrorHandling;

namespace Listen2Me.MVVM.ViewModels;

/// <summary>
/// Provides a default shell landing view model.
/// </summary>
public sealed partial class DefaultHomeViewModel : ViewModelBase
{
    [ObservableProperty] private string _title = "Listen2Me";

    [ObservableProperty] private string _description = "MVVM scaffold initialized.";

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultHomeViewModel"/> class.
    /// </summary>
    public DefaultHomeViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger)
        : base(errorHandler, logger, messenger)
    {
    }
}