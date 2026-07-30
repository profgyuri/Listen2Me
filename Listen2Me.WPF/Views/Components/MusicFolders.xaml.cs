using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Listen2Me.MVVM.Persistence.Entities;

namespace Listen2Me.WPF.Views.Components;

public partial class MusicFolders : UserControl
{
    public MusicFolders()
    {
        InitializeComponent();
    }

    //ItemSource DP
    public IEnumerable<MusicFolder> ItemSource
    {
        get => (IEnumerable<MusicFolder>)GetValue(ItemSourceProperty);
        set => SetValue(ItemSourceProperty, value);
    }

    public static readonly DependencyProperty ItemSourceProperty =
        DependencyProperty.Register(nameof(ItemSource), typeof(IEnumerable<MusicFolder>), typeof(MusicFolders));
    
    
    //SelectedItems DP
    public ObservableCollection<MusicFolder> SelectedItems
    {
        get => (ObservableCollection<MusicFolder>)GetValue(SelectedItemsProperty);
        set => SetValue(SelectedItemsProperty, value);
    }
    
    public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.RegisterAttached(
        nameof(SelectedItems),  
        typeof(ObservableCollection<MusicFolder>), 
        typeof(MusicFolders), 
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    //AddFolder Command
    public ICommand AddFolderCommand
    {
        get => (ICommand)GetValue(AddFolderCommandProperty);
        set => SetValue(AddFolderCommandProperty, value);
    }

    public static readonly DependencyProperty AddFolderCommandProperty =
        DependencyProperty.Register(nameof(AddFolderCommand), typeof(ICommand), typeof(MusicFolders));
    
    //RemoveFolder Command
    public ICommand RemoveFolderCommand
    {
        get => (ICommand)GetValue(RemoveFolderCommandProperty);
        set => SetValue(RemoveFolderCommandProperty, value);
    }
    
    public static readonly DependencyProperty RemoveFolderCommandProperty =
        DependencyProperty.Register(nameof(RemoveFolderCommand), typeof(ICommand), typeof(MusicFolders));

    private void ListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListView listView)
        {
            SelectedItems = new ObservableCollection<MusicFolder>(listView.SelectedItems.Cast<MusicFolder>());
        }
    }
}