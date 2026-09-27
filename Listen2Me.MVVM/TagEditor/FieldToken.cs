namespace Listen2Me.MVVM.TagEditor;

internal sealed record FieldToken(string Name, Func<string, bool>? Validator = null) : MaskToken;