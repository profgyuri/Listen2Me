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
}