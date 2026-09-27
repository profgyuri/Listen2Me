using System.Windows;
using System.Windows.Controls;
using Listen2Me.MVVM.Extensions;
using Listen2Me.MVVM.Persistence.Entities;
using Listen2Me.MVVM.ViewModels.Layouts;

namespace Listen2Me.WPF.Views.Layouts;

public partial class TagEditorLayout : UserControl
{
    public TagEditorLayout()
    {
        InitializeComponent();
    }

    private void SongGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var vm = (TagEditorLayoutViewModel)DataContext;
        vm.SelectedSongs?.AddRange(e.AddedItems.Cast<Song>());
        vm.SelectedSongs?.RemoveRange(e.RemovedItems.Cast<Song>());
    }

    private void DataGrid_OnDragEnter(object sender, DragEventArgs e) => UpdateDragEffect(e);

    private void DataGrid_OnDragOver(object sender, DragEventArgs e) => UpdateDragEffect(e);
    
    private static void UpdateDragEffect(DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop)
            ? DragDropEffects.Copy
            : DragDropEffects.None;
        e.Handled = true;
    }

    private async void DataGrid_OnDrop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            return;

        e.Handled = true;
        var paths = (string[]?)e.Data.GetData(DataFormats.FileDrop);
        
        if (paths is null) return;
        
        if (DataContext is TagEditorLayoutViewModel vm)
        {
            await vm.HandleDroppedPaths(paths);
        }
    }
}