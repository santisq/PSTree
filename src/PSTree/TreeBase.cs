namespace PSTree;

public abstract class TreeBase(string hierarchy, string source)
{
    public string Hierarchy { get; internal set; } = hierarchy;

    public int Depth { get; protected set; }

    internal string Source { get; } = source;
}
