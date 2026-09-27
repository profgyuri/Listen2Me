using System.Text;

namespace Listen2Me.MVVM.TagEditor;

/// <inheritdoc/>
public class MaskParser : IMaskParser
{
    /// <inheritdoc/>
    public List<MaskToken> Parse(string formula)
    {
        int pos = 0;
        var tokens = ParseSequence(formula, ref pos, insideOptional: false);

        if (pos != formula.Length)
            throw new MaskParseException($"Unexpected ']' at position {pos}.");

        ValidateNoAdjacentFields(tokens);
        return tokens;
    }

    private List<MaskToken> ParseSequence(string formula, ref int pos, bool insideOptional)
    {
        var tokens = new List<MaskToken>();
        var literal = new StringBuilder();

        void FlushLiteral()
        {
            if (literal.Length > 0)
            {
                tokens.Add(new LiteralToken(literal.ToString()));
                literal.Clear();
            }
        }

        while (pos < formula.Length)
        {
            char c = formula[pos];

            if (c == ']')
            {
                if (!insideOptional)
                    throw new MaskParseException($"Unmatched ']' at position {pos}.");
                break; // let the caller consume it
            }

            if (c == '[')
            {
                FlushLiteral();
                pos++; // consume '['
                var inner = ParseSequence(formula, ref pos, insideOptional: true);

                if (pos >= formula.Length || formula[pos] != ']')
                    throw new MaskParseException($"Unclosed '[' starting before position {pos}.");
                pos++; // consume ']'

                tokens.Add(new OptionalToken(inner));
                continue;
            }

            if (c == '%')
            {
                FlushLiteral();
                int end = formula.IndexOf('%', pos + 1);
                if (end < 0)
                    throw new MaskParseException($"Unclosed '%' starting at position {pos}.");

                string name = formula.Substring(pos + 1, end - pos - 1);
                if (name.Length == 0)
                    throw new MaskParseException($"Empty field name at position {pos}.");

                tokens.Add(new FieldToken(name));
                pos = end + 1;
                continue;
            }

            literal.Append(c);
            pos++;
        }

        FlushLiteral();
        return tokens;
    }

    private void ValidateNoAdjacentFields(IReadOnlyList<MaskToken> tokens)
    {
        for (int i = 0; i < tokens.Count - 1; i++)
        {
            if (tokens[i] is FieldToken && tokens[i + 1] is FieldToken)
                throw new MaskParseException(
                    "Two fields cannot be adjacent without a literal separator between them.");
        }

        foreach (var opt in tokens.OfType<OptionalToken>())
            ValidateNoAdjacentFields(opt.Inner);
    }
}