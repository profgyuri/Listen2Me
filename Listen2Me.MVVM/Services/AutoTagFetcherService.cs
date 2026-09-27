using System.Collections.ObjectModel;
using Listen2Me.MVVM.Extensions;
using Listen2Me.MVVM.Messages;
using Listen2Me.MVVM.Messages.Queuing;
using Listen2Me.MVVM.Navigation;
using Listen2Me.MVVM.Persistence.Entities;
using Listen2Me.MVVM.Settings;
using Listen2Me.MVVM.TagEditor;
using Listen2Me.MVVM.ViewModels.Shells;
using Serilog;

namespace Listen2Me.MVVM.Services;

/// <inheritdoc cref="IAutoTagFetcherService"/>
public class AutoTagFetcherService : IAutoTagFetcherService
{
    private readonly IMessageQueue _messageQueue;
    private readonly ILogger _logger;
    private readonly IDialogManager _dialogManager;
    private readonly IFilenameToTagsParser _filenameToTagsParser;
    private readonly ITagsToFilenameParser _tagsToFilenameParser;
    private readonly ISettings _settings;

    public AutoTagFetcherService(IMessageQueue messageQueue, ILogger logger, IDialogManager dialogManager, 
        IFilenameToTagsParser filenameToTagsParser, ITagsToFilenameParser tagsToFilenameParser, ISettings settings)
    {
        _messageQueue = messageQueue;
        _logger = logger;
        _dialogManager = dialogManager;
        _filenameToTagsParser = filenameToTagsParser;
        _tagsToFilenameParser = tagsToFilenameParser;
        _settings = settings;
    }

    /// <inheritdoc cref="IAutoTagFetcherService.FetchTagsFromFilenameAsync"/>
    public async Task FetchTagsFromFilenameAsync(ObservableCollection<Song> songs)
    {
        _messageQueue.Enqueue(new ForwardFirstSelectedSongMessage(songs[0]));
        _messageQueue.Enqueue(new FormulaDialogTypeMessage(true));
        
        _logger.Debug("Showing formula dialog to extract tags from filename");
        var result = _dialogManager.ShowDialogAsync<TagEditorFormulaViewModel, bool>();

        if (!await result)
        {
            _logger.Debug("Dialog returned false, aborting");
            return;
        }
        
        _logger.Debug("Editing tags for {0} songs", songs.Count);
        foreach (var song in songs)
        {
            var tags = _filenameToTagsParser.Parse(song.FileName, _settings.TagEditor.FilenameToTagsFormula);
            song.MapFromDictionary(tags);
        }
    }

    /// <inheritdoc cref="IAutoTagFetcherService.FetchFilenameFromTagsAsync"/>
    public async Task FetchFilenameFromTagsAsync(ObservableCollection<Song> songs)
    {
        _messageQueue.Enqueue(new ForwardFirstSelectedSongMessage(songs[0]));
        _messageQueue.Enqueue(new FormulaDialogTypeMessage(false));
        var result = _dialogManager.ShowDialogAsync<TagEditorFormulaViewModel, bool>();

        if (!await result)
        {
            return;
        }

        foreach (var song in songs)
        {
            var newFilename =
                _tagsToFilenameParser.Generate(song.MapToDictionary(), _settings.TagEditor.TagsToFilenameFormula);
            if (string.IsNullOrEmpty(newFilename)) continue;
            
            song.FileName = newFilename;
        }
    }
}