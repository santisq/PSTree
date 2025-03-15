namespace PSTree;

internal interface ITree
{
    string Source { get; }

    bool Include { get; }

    string Hierarchy { get; }

    int Depth { get; }
}
