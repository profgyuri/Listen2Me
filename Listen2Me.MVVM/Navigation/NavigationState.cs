using CommunityToolkit.Mvvm.ComponentModel;

namespace Listen2Me.MVVM.Navigation;

/// <summary>
/// Stores the active navigation state for the shell.
/// </summary>
public sealed partial class NavigationState : ObservableObject
{
    [ObservableProperty] private string _currentRoute = string.Empty;

    [ObservableProperty] private object? _currentViewModel;
}