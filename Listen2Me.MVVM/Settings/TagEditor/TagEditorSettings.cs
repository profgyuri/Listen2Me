namespace Listen2Me.MVVM.Settings.TagEditor;

/// <summary>
/// Holds settings for the tag editor.
/// </summary>
public class TagEditorSettings : JsonSettingsMemory
{
    /// <summary>
    /// The formula to generate the filename from the tags.
    /// </summary>
    public string TagsToFilenameFormula { get; set; } = string.Empty;

    /// <summary>
    /// The formula to generate the tags from the filename.
    /// </summary>
    public string FilenameToTagsFormula { get; set; } = string.Empty;
    
    protected override string FileName => "tagEditor.json";
}