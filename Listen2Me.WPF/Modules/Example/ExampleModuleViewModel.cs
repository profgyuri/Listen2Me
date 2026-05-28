using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Serilog;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Threading;
using Listen2Me.MVVM.ViewModels;

namespace Listen2Me.WPF.Modules.Example;

/// <summary>
/// Provides a concrete feature ViewModel for module scaffolding examples.
/// </summary>
public sealed partial class ExampleModuleViewModel : ViewModelBase
{
    private readonly IExampleStatusService _statusService;
    private readonly IUiDispatcher _uiDispatcher;

    [ObservableProperty] private string _title = "Example Module";

    [ObservableProperty] private string _status = "Initializing...";

    [ObservableProperty] private int _broadcastCount;

    [ObservableProperty] private string _lastReceivedMessage = "No messages yet.";

    /// <summary>
    /// Initializes a new instance of the <see cref="ExampleModuleViewModel"/> class.
    /// </summary>
    public ExampleModuleViewModel(
        IExampleStatusService statusService,
        IUiDispatcher uiDispatcher,
        IErrorHandler errorHandler,
        ILogger logger,
        IMessenger messenger)
        : base(errorHandler, logger, messenger)
    {
        _statusService = statusService;
        _uiDispatcher = uiDispatcher;
        RegisterMessage<ExamplePingMessage>(message =>
            _uiDispatcher.InvokeAsync(() => LastReceivedMessage = message.Value));
    }

    /// <inheritdoc />
    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        Status = await _statusService.GetStatusAsync(cancellationToken);
    }

    [RelayCommand]
    private Task RefreshStatusAsync() =>
        ExecuteSafeAsync(
            async ct => { Status = await _statusService.GetStatusAsync(ct); },
            "ExampleModule.RefreshStatus");

    [RelayCommand]
    private void BroadcastMessage()
    {
        BroadcastCount++;
        Messenger.Send(new ExamplePingMessage($"Ping {BroadcastCount} at {DateTimeOffset.Now:T}"));
    }
}