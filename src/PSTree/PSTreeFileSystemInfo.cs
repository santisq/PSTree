namespace PSTree;

public abstract class PSTreeFileSystemInfo(string hierarchy, string source)
{
    internal PSTreeDirectory? ParentNode { get; set; }

    internal bool ShouldInclude { get; set; }

    internal string Source { get; set; } = source;

    public int Depth { get; protected set; }

    public string Hierarchy { get; internal set; } = hierarchy;

    public long Length { get; internal set; }

    public string GetFormattedLength() =>
        Internal._FormattingInternals.GetFormattedLength(Length);
}
