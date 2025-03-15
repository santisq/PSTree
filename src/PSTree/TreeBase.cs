namespace PSTree;

public abstract class TreeBase<TContainer>(string hierarchy, string source) : ITree
    where TContainer : TreeBase<TContainer>
{
    internal TContainer? ParentNode { get; private set; }

    internal string Source { get; } = source;

    internal virtual bool Include { get; set; }

    public string Hierarchy { get; internal set; } = hierarchy;

    public int Depth { get; protected set; }

    string ITree.Source { get => Source; }

    bool ITree.Include { get => Include; }

    string ITree.Hierarchy { get => Hierarchy; }

    internal TSelf AddParent<TSelf>(TContainer parent)
        where TSelf : TreeBase<TContainer>
    {
        ParentNode = parent;
        return (TSelf)this;
    }

    internal void SetIncludeFlag()
    {
        Include = true;

        for (TContainer? i = ParentNode; i is not null; i = i.ParentNode)
        {
            if (i.Include)
            {
                return;
            }

            i.Include = true;
        }
    }
}
