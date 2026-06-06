using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Listen2Me.MVVM.Messages;

/// <summary>
/// Message to navigate to a new route via the NavBar.
/// </summary>
public class NavBarNavigationMessage(string path) : ValueChangedMessage<string>(path)
{ }