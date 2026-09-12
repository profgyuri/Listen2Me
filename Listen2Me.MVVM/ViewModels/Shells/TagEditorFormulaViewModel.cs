using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.Extensions;
using Listen2Me.MVVM.Messages;
using Listen2Me.MVVM.Messages.Queuing;
using Listen2Me.MVVM.Persistence.Entities;
using Listen2Me.MVVM.Settings.TagEditor;
using Listen2Me.MVVM.TagEditor;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Shells;

public partial class TagEditorFormulaViewModel : DialogViewModelBase<bool>
{
    private readonly IMessageQueue _messageQueue;
    private readonly IFilenameToTagsParser _filenameToTagsParser;
    private readonly ITagsToFilenameParser _tagsToFilenameParser;
    private readonly TagEditorSettings _settings;
    
    [ObservableProperty] private string _fileName = string.Empty;
    [ObservableProperty] private string _formula = string.Empty;
    [ObservableProperty] private Dictionary<string, string> _readTags = new();

    private bool _isFilenameToTags;
    private Song _song;
    
    public TagEditorFormulaViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger, 
        IMessageQueue messageQueue, IFilenameToTagsParser filenameToTagsParser, 
        ITagsToFilenameParser tagsToFilenameParser, TagEditorSettings settings) 
        : base(errorHandler, logger, messenger)
    {
        _messageQueue = messageQueue;
        _filenameToTagsParser = filenameToTagsParser;
        _tagsToFilenameParser = tagsToFilenameParser;
        _settings = settings;
    }

    /// <inheritdoc/>
    public override Task OnOpenAsync(CancellationToken ct = default)
    {
        if (ct.IsCancellationRequested) return Task.CompletedTask;
        Result = false;
        
        Logger.Debug("TagEditor ID: {Id}", Id);
        
        var firstSongMessage = _messageQueue.Dequeue<ForwardFirstSelectedSongMessage>();
        if (firstSongMessage is null) throw new InvalidOperationException("No song was sent as a template.");
        Logger.Debug("First song sent: {Song}", firstSongMessage.Song);
        
        var formulaDialogTypeMessage = _messageQueue.Dequeue<FormulaDialogTypeMessage>();
        if (formulaDialogTypeMessage is null) throw new InvalidOperationException("Type of formula dialog was not sent.");
        Logger.Debug("Formula dialog type is filename to tags: {IsFilenameToTags}", formulaDialogTypeMessage.IsFilenameToTags);

        _song = firstSongMessage.Song;
        _isFilenameToTags = formulaDialogTypeMessage.IsFilenameToTags;

        if (_isFilenameToTags)
        {
            FileName = _song.FileName;
            Formula = _settings.FilenameToTagsFormula;
        }
        else
        {
            ReadTags = _song.MapToDictionary();
            Formula = _settings.TagsToFilenameFormula;
        }
        
        Logger.Debug("TagEditor OnOpen method handled.");
        return base.OnOpenAsync(ct);
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
        Logger.Debug("TagEditor Ok method invoked with formula: {0}", Formula);
        
        if (_isFilenameToTags)
            _settings.FilenameToTagsFormula = Formula;
        else
            _settings.TagsToFilenameFormula = Formula;
        
        try
        {
            await _settings.SaveAsync();
            Logger.Debug("Formula saved to settings.");
        }
        catch (Exception e)
        {
            await ErrorHandler.HandleAsync(e, "Failed to save formula settings.");
        }
        
        Result = true;
        Logger.Debug("Formula dialog closing with result: {0}.", Result);
    }
    
    [RelayCommand]
    private async Task Cancel()
    {
        Result = false;
        Logger.Debug("Formula dialog cancelled.");
    }
}