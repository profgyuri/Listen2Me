using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.System.Browsing;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Shells;

public partial class FolderBrowserDialogViewModel : DialogViewModelBase<string>
{
    [ObservableProperty] private ObservableCollection<string> _drives = new();
    [ObservableProperty] private ObservableCollection<string> _bookmarks = new();
    [ObservableProperty] private ObservableCollection<string> _folders = new();
    [ObservableProperty] private string _selectedDrive = string.Empty;
    [ObservableProperty] private string _selectedBookmark = string.Empty;
    [ObservableProperty] private string _selectedFolder = string.Empty;

    private readonly IFolderBrowser _folderBrowser;
    
    public FolderBrowserDialogViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger, 
        IFolderBrowser folderBrowser)
        : base(errorHandler, logger, messenger)
    {
        _folderBrowser = folderBrowser;
    }

    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        Drives = new ObservableCollection<string>(_folderBrowser.GetRoots());

        SelectedDrive = Drives[0];
        _folderBrowser.NavigateTo(SelectedDrive);
        await ActualizeSubFolders();
        
        await base.InitializeAsync(cancellationToken);
    }

    private async Task ActualizeSubFolders()
    {
        var subFolders = await _folderBrowser.GetSubFolders();
        Folders.Clear();
        foreach (var subFolder in subFolders)
        {
            Folders.Add(subFolder);
        }
    }

    [RelayCommand]
    private async Task NavigateTo(string path)
    {
        await ExecuteSafeAsync(async _ =>
        {
            _folderBrowser.NavigateTo(path);
            await ActualizeSubFolders();
        }, "Navigate to command");
    }

    [RelayCommand]
    private async Task NavigateToChild(string path)
    {
        await ExecuteSafeAsync(async _ =>
        {
            _folderBrowser.NavigateToChild(path);
            await ActualizeSubFolders();
        }, "Navigate to child command");   
    }
}