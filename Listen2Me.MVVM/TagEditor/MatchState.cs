using System.Collections.Immutable;

namespace Listen2Me.MVVM.TagEditor;

public sealed record MatchState(int Position, ImmutableDictionary<string, string> Captures);