using System.Collections.ObjectModel;
using System.IO;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.Persistence.Entities;
using Listen2Me.MVVM.ViewModels.Shells;

namespace Listen2Me.MVVM.Services;

/// <inheritdoc cref="IFileMoverService"/>
public class FileMoverService : IFileMoverService
{
    private readonly IDialogManager _dialogManager;

    public FileMoverService(IDialogManager dialogManager)
    {
        _dialogManager = dialogManager;
    }

    /// <inheritdoc cref="IFileMoverService.MoveFilesAsync"/>
    public async Task MoveFilesAsync(ObservableCollection<Song> songs)
    {
        var path = await _dialogManager.ShowDialogAsync<FolderBrowserDialogViewModel, string>();
        if (string.IsNullOrEmpty(path)) return;

        foreach (var song in songs)
        {
            var fileName = Path.GetFileName(song.Path);
            var newPath = Path.Combine(path, fileName);
            
            // the custom property changed event will trigger the file rename or move
            song.Path = newPath;
        }
    }
}