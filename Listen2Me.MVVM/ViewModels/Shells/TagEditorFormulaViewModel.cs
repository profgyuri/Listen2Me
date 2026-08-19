using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
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
    
    [ObservableProperty] private string _fileName = string.Empty;
    [ObservableProperty] private string _formula = string.Empty;
    [ObservableProperty] private Dictionary<string, string> _readTags = new();

    private Song _song;
    
    public TagEditorFormulaViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger, 
        IMessageQueue messageQueue, IFilenameToTagsParser filenameToTagsParser) 
        : base(errorHandler, logger, messenger)
    {
        _messageQueue = messageQueue;
        _filenameToTagsParser = filenameToTagsParser;
    }

    public override Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        var message = _messageQueue.Dequeue<ForwardFirstSelectedSongMessage>();
        if (message is null) throw new InvalidOperationException("No matching message found in the queue.");
        
        _song = message.Song;
        FileName = _song.FileName;
        
        return base.InitializeAsync(cancellationToken);
    }

    partial void OnFormulaChanged(string value)
    {
        ReadTags = _filenameToTagsParser.Parse(FileName, value)?.ToDictionary() ?? new Dictionary<string, string>();
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