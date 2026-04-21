namespace PSTree;

public interface ITree
{
    string Source { get; }

    bool Include { get; }

    string? Hierarchy { get; internal set; }

    int Depth { get; }
}
