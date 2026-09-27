using System.Text;

namespace Listen2Me.MVVM.TagEditor;

/// <inheritdoc/>
public sealed class TagsToFilenameParser : ITagsToFilenameParser
{
    private readonly IMaskParser _maskParser;
    
    public TagsToFilenameParser(IMaskParser maskParser)
    {
        _maskParser = maskParser;
    }
    
    /// <inheritdoc/>
    public string? Generate(IReadOnlyDictionary<string, string> tags, string formula)
    {
        List<MaskToken> tokens;
        try
        {
            tokens = _maskParser.Parse(formula);
        }
        catch (MaskParseException)
        {
            return null;
        }

        var sb = new StringBuilder();
        return TryBuildSequence(tokens, tags, sb) ? sb.ToString() : null;
    }
    
    /// <summary>
    /// Appends tokens into <paramref name="sb"/>. Returns false if a *required*
    /// field (i.e. one outside any optional block, or the current recursion level
    /// treats as required) has no value — the caller decides what to do with that.
    /// </summary>
    private static bool TryBuildSequence(
        IReadOnlyList<MaskToken> tokens, IReadOnlyDictionary<string, string> tags, StringBuilder sb)
    {
        foreach (var token in tokens)
        {
            switch (token)
            {
                case LiteralToken lit:
                    sb.Append(lit.Text);
                    break;

                case FieldToken field:
                    if (!tags.TryGetValue(field.Name, out var value) || string.IsNullOrEmpty(value))
                        return false; // invalid token

                    if (field.Validator?.Invoke(value) == false)
                        return false; // invalid value

                    sb.Append(value);
                    break;

                case OptionalToken opt:
                {
                    var scratch = new StringBuilder();
                    if (TryBuildSequence(opt.Inner, tags, scratch))
                        sb.Append(scratch);
                    break;
                }
            }
        }

        return true;
    }
}