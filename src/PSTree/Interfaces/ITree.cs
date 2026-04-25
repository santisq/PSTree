namespace PSTree.Interfaces;

public interface ITree
{
    internal bool IsContainer { get; }
    internal ITree? Container { get; }
    internal string Source { get; }
    // internal bool Include { get; set; }
    public string? Hierarchy { get; internal set; }
    public int Depth { get; }
    public string Name { get; }
}
