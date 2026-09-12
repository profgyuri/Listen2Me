using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Serilog;

namespace Listen2Me.MVVM.ViewModels;

public partial class DialogViewModelBase<TResult> : ViewModelBase
{
    [ObservableProperty] private TResult? _result;
    
    public DialogViewModelBase(IErrorHandler errorHandler, ILogger logger, IMessenger messenger) 
        : base(errorHandler, logger, messenger)
    { }

    /// <summary>
    /// Intended to be called every time the dialog is opened regardless of initialization status,
    /// opposed to <see cref="ViewModelBase.InitializeAsync"/> which is called once per lifetime scope.
    /// </summary>
    public virtual async Task OnOpenAsync(CancellationToken ct = default)
    {
        await Task.CompletedTask;
    }
}