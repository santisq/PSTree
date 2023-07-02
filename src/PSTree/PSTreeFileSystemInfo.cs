namespace PSTree;

public abstract class PSTreeFileSystemInfo
{
    internal int Depth { get; set; }

    public string Hierarchy { get; internal set; }

    protected PSTreeFileSystemInfo(string hierarchy) =>
        Hierarchy = hierarchy;

    public long Length { get; internal set; }
}
