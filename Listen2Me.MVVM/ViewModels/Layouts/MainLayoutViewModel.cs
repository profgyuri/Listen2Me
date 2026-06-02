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

    public void DragOver(IDropInfo dropInfo)
    {
        if (dropInfo.Data is WidgetViewModel)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
            dropInfo.Effects = DragDropEffects.Move;
        }
    }

    public void Drop(IDropInfo dropInfo)
    {
        if (dropInfo.Data is not WidgetViewModel source) return;

        // VisualTarget is the ContentControl being dropped onto
        if (dropInfo.VisualTarget is not ContentControl { Content: WidgetViewModel target }) return;

        var sourceIndex = Widgets.IndexOf(source);
        var targetIndex = Widgets.IndexOf(target);

        if (sourceIndex < 0 || targetIndex < 0 || sourceIndex == targetIndex) return;

        Widgets[sourceIndex] = target;
        Widgets[targetIndex] = source;
    }
}