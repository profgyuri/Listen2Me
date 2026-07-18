using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Media;
using Listen2Me.MVVM.Settings.Appearance.Themes;
using Serilog;

namespace Listen2Me.MVVM.Settings.Appearance;

/// <summary>
/// Holds settings for the appearance of the application.
/// </summary>
public class AppearanceSettings : JsonSettingsMemory
{
    private readonly ILogger _logger;

    [JsonConstructor]
    public AppearanceSettings()
    { }
    
    public AppearanceSettings(IThemeManager themeManager, ILogger logger)
    {
        _logger = logger;
        ThemeManager = themeManager;
    }
    
    /// <summary>
    /// Wires up the theme manager after deserialization and applies the current theme/accent.
    /// Must be called once, immediately after the settings are loaded from disk.
    /// </summary>
    public void SetThemeManager(IThemeManager themeManager)
    {
        ThemeManager = themeManager;
        ThemeManager.SetTheme(Theme);
        ThemeManager.SetAccent(Accent);
    }

    /// <inheritdoc cref="IThemeManager"/>
    [JsonIgnore]
    public IThemeManager ThemeManager { get; private set; }
    
    /// <summary>
    /// Font family to use for the application.
    /// </summary>
    [JsonConverter(typeof(FontFamilyJsonConverter))]
    public FontFamily FontFamily { get; set; } = new("Segoe UI");

    /// <summary>
    /// Theme to use for the application.
    /// </summary>
    public Themes.Themes Theme
    {
        get;
        set
        {
            field = value;
            ThemeManager?.SetTheme(value);
        }
    } = Themes.Themes.Light;

    /// <summary>
    /// Accent color to use for the application.
    /// </summary>
    public Accents Accent
    {
        get;
        set
        {
            field = value;
            ThemeManager?.SetAccent(value);
        }
    } = Accents.Blue;

    /// <summary>
    /// Gets or sets a value indicating whether the main view's grid is editable.
    /// </summary>
    public bool IsGridEditable { get; set; }

    protected override string FileName => "appearance.json";
}

file class FontFamilyJsonConverter : JsonConverter<FontFamily>
{
    public override FontFamily Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var source = reader.GetString();
        return string.IsNullOrWhiteSpace(source) ? new FontFamily("Segoe UI") : new FontFamily(source);
    }

    public override void Write(Utf8JsonWriter writer, FontFamily value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Source);
    }
}