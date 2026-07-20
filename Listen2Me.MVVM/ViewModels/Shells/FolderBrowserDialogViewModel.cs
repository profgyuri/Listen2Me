using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Navigation;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Shells;

public partial class FolderBrowserDialogViewModel : DialogViewModelBase<string>
{
    public FolderBrowserDialogViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger, 
        IDialogManager dialogManager)
        : base(errorHandler, logger, messenger)
    {
    }
}