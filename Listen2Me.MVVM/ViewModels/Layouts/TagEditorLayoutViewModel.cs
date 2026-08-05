using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Persistence.Entities;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Layouts;

public partial class TagEditorLayoutViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<Song> _songs = new();
    
    public TagEditorLayoutViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger) 
        : base(errorHandler, logger, messenger)
    {
    }

    public override Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        Songs.Add(new Song(Guid.NewGuid(), "Song 1", "Artist 1", "Genre 1", 128, 320, new TimeSpan(0, 0, 10), @"C:\song1.mp3", 5300, new DateTime(2022, 1, 1)));
        Songs.Add(new Song(Guid.NewGuid(), "Song 2", "Artist 2", "Genre 2", 128, 320, new TimeSpan(0, 0, 10), @"C:\song2.mp3", 5300, new DateTime(2022, 1, 1)));
        
        return base.InitializeAsync(cancellationToken);
    }
}