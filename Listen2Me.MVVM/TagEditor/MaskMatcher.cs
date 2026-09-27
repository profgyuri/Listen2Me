namespace Listen2Me.MVVM.TagEditor;

/// <inheritdoc/>
public class MaskMatcher : IMaskMatcher
{
    /// <inheritdoc/>
    public IEnumerable<MatchState> MatchSequence(
        IReadOnlyList<MaskToken> tokens, string input, MatchState state)
    {
        if (tokens.Count == 0)
        {
            yield return state;
            yield break;
        }

        var head = tokens[0];
        var rest = tokens.Skip(1).ToArray();

        foreach (var afterHead in MatchOne(head, rest, input, state))
        foreach (var final in MatchSequence(rest, input, afterHead))
            yield return final;
    }

    private IEnumerable<MatchState> MatchOne(
        MaskToken token, IReadOnlyList<MaskToken> rest, string input, MatchState state)
    {
        switch (token)
        {
            case LiteralToken lit:
                if (input.AsSpan(state.Position).StartsWith(lit.Text))
                    yield return state with { Position = state.Position + lit.Text.Length };
                yield break;

            case FieldToken field:
            {
                if (rest.Count == 0)
                {
                    if (state.Position >= input.Length)
                        yield break; // nothing left to capture

                    var value = input[state.Position..];
                    if (field.Validator?.Invoke(value) ?? true)
                        yield return new MatchState(Position: input.Length, Captures: state.Captures.SetItem(field.Name, value));
                    yield break;
                }

                if (rest[0] is not LiteralToken terminator)
                {
                    for (var end = state.Position + 1; end <= input.Length; end++)
                    {
                        var value = input[state.Position..end];
                        if (field.Validator?.Invoke(value) ?? true)
                            yield return new MatchState(Position: end, Captures: state.Captures.SetItem(field.Name, value));
                    }
                    yield break;
                }

                var idx = input.IndexOf(terminator.Text, state.Position + 1, StringComparison.Ordinal);
                while (idx >= 0)
                {
                    var value = input[state.Position..idx];
                    if (field.Validator?.Invoke(value) ?? true)
                        yield return state with
                        {
                            Position = idx,
                            Captures = state.Captures.SetItem(field.Name, value)
                        };
                    idx = input.IndexOf(terminator.Text, idx + 1, StringComparison.Ordinal);
                }
                yield break;
            }

            case OptionalToken opt:
                // Prefer matching the optional block; fall back to skipping it.
                var any = false;
                foreach (var s in MatchSequence(opt.Inner, input, state))
                {
                    any = true;
                    yield return s;
                }
                yield return state; // always also offer the "skip" branch
                if (!any) yield break;
                yield break;
        }
    }
}