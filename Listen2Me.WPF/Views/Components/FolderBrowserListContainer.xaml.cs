using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Listen2Me.WPF.Views.Components;

public partial class FolderBrowserListContainer : UserControl
{
    public FolderBrowserListContainer()
    {
        InitializeComponent();
    }
    
    #region Header attached property
    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }
    
    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.RegisterAttached(
            "Header",
            typeof(string),
            typeof(FolderBrowserListContainer));
    #endregion

    #region ItemsSource attached property
    public IEnumerable<string> ItemsSource
    {
        get => (IEnumerable<string>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }
    
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.RegisterAttached(
            "ItemsSource",
            typeof(IEnumerable<string>),
            typeof(FolderBrowserListContainer));
    #endregion

    #region SelectedItem attached property
    public string SelectedItem
    {
        get => (string)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }
    
    public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.RegisterAttached(
            "SelectedItem",
            typeof(string),
            typeof(FolderBrowserListContainer), 
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    #endregion

    #region ItemDoubleClickCommand property and handler
    private void ListViewItem_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is ListViewItem { DataContext: string item } && ItemDoubleClickCommand?.CanExecute(item) == true)
        {
            ItemDoubleClickCommand.Execute(item);
        }
    }
    
    public ICommand ItemDoubleClickCommand
    {
        get => (ICommand)GetValue(ItemDoubleClickCommandProperty);
        set => SetValue(ItemDoubleClickCommandProperty, value);
    }

    public static readonly DependencyProperty ItemDoubleClickCommandProperty =
        DependencyProperty.RegisterAttached(
            nameof(ItemDoubleClickCommand),
            typeof(ICommand),
            typeof(FolderBrowserListContainer));
    #endregion

    #region IsBookmarkable property
    public static readonly DependencyProperty IsBookmarkableProperty = DependencyProperty.Register(
        nameof(IsBookmarkable), 
        typeof(bool), 
        typeof(FolderBrowserListContainer), 
        new PropertyMetadata(false));

    public bool IsBookmarkable
    {
        get => (bool)GetValue(IsBookmarkableProperty);
        set => SetValue(IsBookmarkableProperty, value);
    }
    #endregion

    #region AddBookmarkCommand property
    public static readonly DependencyProperty AddBookmarkCommandProperty = DependencyProperty.Register(
        nameof(AddBookmarkCommand), 
        typeof(ICommand), 
        typeof(FolderBrowserListContainer), 
        new PropertyMetadata(default(ICommand)));

    public ICommand AddBookmarkCommand
    {
        get => (ICommand)GetValue(AddBookmarkCommandProperty);
        set => SetValue(AddBookmarkCommandProperty, value);
    }
    #endregion
}