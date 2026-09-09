using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Extensions;
using Listen2Me.MVVM.Messages;
using Listen2Me.MVVM.Messages.Queuing;
using Listen2Me.MVVM.Persistence.Entities;
using Listen2Me.MVVM.TagEditor;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Shells;

public partial class TagEditorFormulaViewModel : DialogViewModelBase<bool>
{
    private readonly IMessageQueue _messageQueue;
    private readonly IFilenameToTagsParser _filenameToTagsParser;
    private readonly ITagsToFilenameParser _tagsToFilenameParser;
    
    [ObservableProperty] private string _fileName = string.Empty;
    [ObservableProperty] private string _formula = string.Empty;
    [ObservableProperty] private Dictionary<string, string> _readTags = new();

    private bool _isFilenameToTags;
    private Song _song;
    
    public TagEditorFormulaViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger, 
        IMessageQueue messageQueue, IFilenameToTagsParser filenameToTagsParser, 
        ITagsToFilenameParser tagsToFilenameParser) 
        : base(errorHandler, logger, messenger)
    {
        _messageQueue = messageQueue;
        _filenameToTagsParser = filenameToTagsParser;
        _tagsToFilenameParser = tagsToFilenameParser;
    }

    public override Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        var firstSongMessage = _messageQueue.Dequeue<ForwardFirstSelectedSongMessage>();
        if (firstSongMessage is null) throw new InvalidOperationException("No song was sent as a template.");
        
        var formulaDialogTypeMessage = _messageQueue.Dequeue<FormulaDialogTypeMessage>();
        if (formulaDialogTypeMessage is null) throw new InvalidOperationException("Type of formula dialog was not sent.");

        _song = firstSongMessage.Song;
        _isFilenameToTags = formulaDialogTypeMessage.IsFilenameToTags;

        if (_isFilenameToTags)
        {
            FileName = _song.FileName;
        }
        else
        {
            ReadTags = _song.MapToDictionary();
        }
        
        return base.InitializeAsync(cancellationToken);
    }

    partial void OnFormulaChanged(string value)
    {
        if (_isFilenameToTags)
        {
            ReadTags = _filenameToTagsParser.Parse(FileName, value)?.ToDictionary() ?? new Dictionary<string, string>();
            return;
        }
        
        FileName = _tagsToFilenameParser.Generate(ReadTags, Formula) ?? string.Empty;
    }

    [RelayCommand]
    private async Task Ok()
    {
        Result = true;
    }
    
    [RelayCommand]
    private async Task Cancel()
    {
        Result = false;
    }
}