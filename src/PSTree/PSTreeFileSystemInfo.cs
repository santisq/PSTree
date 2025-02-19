namespace PSTree;

public abstract class PSTreeFileSystemInfo(string hierarchy, string source)
    : PSTreeBase(hierarchy)
{
    internal PSTreeDirectory? ParentNode { get; set; }

    internal bool ShouldInclude { get; set; }

    internal string Source { get; set; } = source;

    public long Length { get; internal set; }

    public string GetFormattedLength() =>
        Internal._FormattingInternals.GetFormattedLength(Length);
}
