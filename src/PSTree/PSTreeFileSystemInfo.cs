namespace PSTree;

public abstract class PSTreeFileSystemInfo(string hierarchy, string source)
{
    protected PSTreeDirectory? _parent;

    internal bool _shouldInclude;

    internal string Source { get; set; } = source;

    public int Depth { get; protected set; }

    public string Hierarchy { get; internal set; } = hierarchy;

    public long Length { get; internal set; }

    public string GetFormattedLength() =>
        Internal._FormattingInternals.GetFormattedLength(Length);
}
