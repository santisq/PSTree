namespace PSTree;

public interface ITree
{
    internal string Source { get; }

    internal bool Include { get; }

    public string? Hierarchy { get; internal set; }

    public int Depth { get; }
}
