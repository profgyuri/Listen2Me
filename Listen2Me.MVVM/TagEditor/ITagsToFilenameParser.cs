namespace Listen2Me.MVVM.TagEditor;

/// <summary>
/// Generates a filename from a dictionary of tags.
/// </summary>
public interface ITagsToFilenameParser
{
    /// <summary>
    /// Generate the filename from the tags.
    /// </summary>
    /// <param name="tags">The tag values to substitute into the formula.</param>
    /// <param name="formula">The string that determines how the filename is built.</param>
    /// <returns>The generated filename (without extension), or null if the formula is invalid
    /// or a required (non-optional) field has no value.</returns>
    string? Generate(IReadOnlyDictionary<string, string> tags, string formula);
}