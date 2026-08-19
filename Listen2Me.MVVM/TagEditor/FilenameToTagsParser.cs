using System.Collections.Immutable;

namespace Listen2Me.MVVM.TagEditor;

/// <inheritdoc/>
public sealed class FilenameToTagsParser : IFilenameToTagsParser
{
    private readonly IMaskParser _maskParser;
    private readonly IMaskMatcher _maskMatcher;

    public FilenameToTagsParser(IMaskParser maskParser, IMaskMatcher maskMatcher)
    {
        _maskParser = maskParser;
        _maskMatcher = maskMatcher;
    }

    /// <inheritdoc/>
    public IReadOnlyDictionary<string, string>? Parse(string filename, string formula)
    {
        List<MaskToken> tokens;
        try
        {
            tokens = _maskParser.Parse(formula);
        }
        catch (MaskParseException)
        {
            // Malformed formula (unbalanced brackets, adjacent fields, etc).
            return null;
        }

        var initialState = new MatchState(0, ImmutableDictionary<string, string>.Empty);

        var completeMatch = _maskMatcher
            .MatchSequence(tokens, filename, initialState)
            .FirstOrDefault(s => s.Position == filename.Length);

        return completeMatch?.Captures;
    }
}