namespace PSTree;

public abstract class PSTreeBase(string hierarchy)
{
    public string Hierarchy { get; internal set; } = hierarchy;

    public int Depth { get; protected set; }
}
