namespace PSTree;

public abstract class TreeBase<TContainer>(string source) : ITree
    where TContainer : TreeBase<TContainer>
{
    internal TContainer? Container { get; private set; }

    internal string Source { get; } = source;

    internal bool Include { get; set; }

    public string? Hierarchy { get; internal set; }

    public int Depth { get; protected set; }

    string ITree.Source { get => Source; }

    bool ITree.Include { get => Include; }

    string? ITree.Hierarchy
    {
        get => Hierarchy;
        set => Hierarchy = value;
    }

    internal TOut AddParent<TOut>(TContainer parent)
        where TOut : TreeBase<TContainer>
    {
        Container = parent;
        return (TOut)this;
    }

    internal void PropagateInclude()
    {
        Include = true;
        for (TContainer? i = Container; i is not null; i = i.Container)
        {
            if (i.Include) return;
            i.Include = true;
        }
    }
}
