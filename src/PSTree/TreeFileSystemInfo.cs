namespace PSTree;

public abstract class TreeFileSystemInfo(string source)
    : TreeBase<TreeDirectory>(source)
{
    public abstract string Name { get; }

    public long Length { get; internal set; }

    public string GetFormattedLength() =>
        Internal._FormattingInternals.GetFormattedLength(Length);
}
