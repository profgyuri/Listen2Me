using Listen2Me.MVVM.Settings;
using Listen2Me.MVVM.ViewModels.Tabs.Settings;
using Microsoft.Extensions.Hosting;

namespace Listen2Me.MVVM.Initialization;

public class StartupScanService : IHostedService
{
    private readonly LibraryTabViewModel _libraryTabViewModel;
    private readonly ISettings _settings;

    public StartupScanService(LibraryTabViewModel libraryTabViewModel, ISettings settings)
    {
        _libraryTabViewModel = libraryTabViewModel;
        _settings = settings;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_settings.Library.ScanAutomatically)
        {
            _ = _libraryTabViewModel.ScanCommand.ExecuteAsync(null);
        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}