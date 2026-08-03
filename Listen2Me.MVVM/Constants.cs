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
    
    public static readonly string SqLiteConnectionString =
        $"Data Source={Environment.ExpandEnvironmentVariables(@"%AppData%\Listen2Me\Listen2Me.db")}";

    public static string[] SupportedAudioExtensions = [".mp3", ".wav", ".flac", ".aiff", ".ogg", ".aac", ".m4a"];
}