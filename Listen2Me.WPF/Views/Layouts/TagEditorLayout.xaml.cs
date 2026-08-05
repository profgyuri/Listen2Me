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
        vm.SelectedSongs.AddRange(e.AddedItems.Cast<Song>());
        vm.SelectedSongs.RemoveRange(e.RemovedItems.Cast<Song>());
    }
}