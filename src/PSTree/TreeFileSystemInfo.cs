namespace PSTree;

public abstract class TreeFileSystemInfo(string hierarchy, string source)
    : TreeBase(hierarchy, source)
{
    internal TreeDirectory? ParentNode { get; set; }

    internal bool ShouldInclude { get; set; }

    public long Length { get; internal set; }

    public string GetFormattedLength() =>
        Internal._FormattingInternals.GetFormattedLength(Length);
}
