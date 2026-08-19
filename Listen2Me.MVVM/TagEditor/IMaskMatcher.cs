namespace Listen2Me.MVVM.TagEditor;

/// <summary>
/// Matches tokens against input strings.
/// </summary>
public interface IMaskMatcher
{
    /// <summary>
    /// Matches a sequence of tokens against an input string.
    /// </summary>
    /// <param name="tokens">List of tokens to match.</param>
    /// <param name="input">String to match against.</param>
    /// <param name="state">Data structure used to track the match state.</param>
    /// <returns></returns>
    IEnumerable<MatchState> MatchSequence(IReadOnlyList<MaskToken> tokens, string input, MatchState state);
}