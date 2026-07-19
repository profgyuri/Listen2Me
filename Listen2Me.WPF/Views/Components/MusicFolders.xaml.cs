using System.Windows;
using System.Windows.Controls;
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
}