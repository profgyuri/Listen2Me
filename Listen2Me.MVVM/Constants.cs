using System.IO;

namespace Listen2Me.MVVM;

/// <summary>
/// Contains constants used throughout the application.
/// </summary>
public static class Constants
{
    /// <summary>
    /// The path to the application settings folder. (%APPDATA%\Listen2Me\Settings)
    /// </summary>
    public static string SettingsFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Listen2Me", "Settings");
}