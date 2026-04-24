namespace PSTree;

public interface ITree
{
    internal ITree? Container { get; }
    internal string Source { get; }
    internal bool Include { get; set; }
    public string? Hierarchy { get; internal set; }
    public int Depth { get; }
}
