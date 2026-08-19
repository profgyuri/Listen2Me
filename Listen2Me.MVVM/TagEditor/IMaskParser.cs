namespace Listen2Me.MVVM.TagEditor;

/// <summary>
/// Parses formulas into tokens.
/// </summary>
public interface IMaskParser
{
    /// <summary>
    /// Parses a mask formula into a list of tokens.
    /// </summary>
    /// <param name="formula">The formula to parse.</param>
    List<MaskToken> Parse(string formula);
}