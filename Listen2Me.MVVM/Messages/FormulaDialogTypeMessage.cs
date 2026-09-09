namespace Listen2Me.MVVM.Messages;

public class FormulaDialogTypeMessage
{
    public bool IsFilenameToTags { get; private set; }

    /// <param name="isFilenameToTags">Specifies whether the tags should be read from the filename.</param>
    public FormulaDialogTypeMessage(bool isFilenameToTags)
    {
        IsFilenameToTags = isFilenameToTags;
    }
}