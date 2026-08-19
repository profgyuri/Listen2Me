namespace Listen2Me.MVVM.TagEditor;

internal sealed record OptionalToken(IReadOnlyList<MaskToken> Inner) : MaskToken;