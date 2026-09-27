using System.Text.RegularExpressions;

namespace Listen2Me.MVVM.TagEditor;

/// <summary>
/// Converts a filename into a dictionary of tags.
/// </summary>
public interface IFilenameToTagsParser
{
    /// <summary>
    /// Get the tags from the filename.
    /// </summary>
    /// <param name="filename">The filename to parse, without the extension.</param>
    /// <param name="formula">The string that determines how the filename is parsed.</param>
    /// <returns>A dictionary of tags and their values.</returns>
    IReadOnlyDictionary<string, string>? Parse(string filename, string formula);
}