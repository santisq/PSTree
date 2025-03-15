namespace PSTree;

public abstract class TreeFileSystemInfo(string hierarchy, string source)
    : TreeBase<TreeDirectory>(hierarchy, source)
{
    public long Length { get; internal set; }

    public string GetFormattedLength() =>
        Internal._FormattingInternals.GetFormattedLength(Length);
}
