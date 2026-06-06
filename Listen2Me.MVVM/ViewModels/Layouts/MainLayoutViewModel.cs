using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GongSolutions.Wpf.DragDrop;
using Listen2Me.MVVM.ErrorHandling;
using Listen2Me.MVVM.ViewModels.Widgets;
using Serilog;

namespace Listen2Me.MVVM.ViewModels.Layouts;

public sealed partial class MainLayoutViewModel : ViewModelBase, IDropTarget
{
    [ObservableProperty] private bool _isEditMode = true;

    public ObservableCollection<ViewModelBase> Widgets { get; }
    
    public MainLayoutViewModel(IErrorHandler errorHandler, ILogger logger, IMessenger messenger, 
        TrackInfoWidgetViewModel trackInfoWidgetViewModel, PlayerControlsWidgetViewModel playerControlsWidgetViewModel,
        SearchResultsWidgetViewModel searchResultsWidgetViewModel, PlaylistWidgetViewModel playlistWidgetViewModel) 
        : base(errorHandler, logger, messenger)
    {
        var orderedWidgets = new List<WidgetViewModel>([
            trackInfoWidgetViewModel, 
            playerControlsWidgetViewModel, 
            searchResultsWidgetViewModel, 
            playlistWidgetViewModel
        ]).OrderBy(w => w.Order).ToList();
        Widgets = new(orderedWidgets);
    }

    #region GongSolutions.Wpf.DragDrop.IDropTarget
    /// <inheritdoc />
    public void DragOver(IDropInfo dropInfo)
    {
        if (dropInfo.Data is WidgetViewModel)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
            dropInfo.Effects = DragDropEffects.Move;
        }
    }

    /// <inheritdoc />
    public void Drop(IDropInfo dropInfo)
    {
        if (dropInfo.Data is not WidgetViewModel source) return;

        // VisualTarget is the ContentControl being dropped onto
        if (dropInfo.VisualTarget is not ContentControl { Content: WidgetViewModel target }) return;
        if (source == target) return;
        
        // Reorder only in the same row or switch rows
        if (source.Order < 2 && target.Order >= 2 ||
            target.Order < 2 && source.Order >= 2)
        {
            (Widgets[0], Widgets[2]) = (Widgets[2], Widgets[0]);
            (Widgets[1], Widgets[3]) = (Widgets[3], Widgets[1]);
            
            return;
        }

        var sourceIndex = Widgets.IndexOf(source);
        var targetIndex = Widgets.IndexOf(target);

        if (sourceIndex < 0 || targetIndex < 0 || sourceIndex == targetIndex) return;

        Widgets[sourceIndex] = target;
        Widgets[targetIndex] = source;
    }
    #endregion
}