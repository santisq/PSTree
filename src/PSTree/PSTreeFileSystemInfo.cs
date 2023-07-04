namespace PSTree;

public abstract class PSTreeFileSystemInfo
{
    internal string Source { get; set; }

    internal int Depth { get; set; }

    public string Hierarchy { get; internal set; }

    public long Length { get; internal set; }

    protected PSTreeFileSystemInfo(string hierarchy, string source)
    {
        Hierarchy = hierarchy;
        Source = source;
    }
}
