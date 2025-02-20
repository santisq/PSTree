namespace PSTree;

public abstract class PSTreeFileSystemInfo(string hierarchy, string source)
    : PSTreeBase(hierarchy, source)
{
    internal PSTreeDirectory? ParentNode { get; set; }

    internal bool ShouldInclude { get; set; }

    public long Length { get; internal set; }

    public string GetFormattedLength() =>
        Internal._FormattingInternals.GetFormattedLength(Length);
}
