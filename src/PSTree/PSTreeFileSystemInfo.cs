namespace PSTree;

public abstract class PSTreeFileSystemInfo(string hierarchy, string source)
{
    internal string Source { get; set; } = source;

    public int Depth { get; set; }

    public string Hierarchy { get; internal set; } = hierarchy;

    public long Length { get; internal set; }

    public string GetFormattedLength() =>
        Internal._FormattingInternals.GetFormattedLength(Length);
}
