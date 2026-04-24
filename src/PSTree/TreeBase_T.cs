using System.Collections.Generic;

namespace PSTree;

public abstract class TreeBase<TContainer, TBase>(string source, int depth = 0) : ITree
    where TContainer : TBase
    where TBase : TreeBase<TContainer, TBase>
{
    internal TContainer? Container { get; set; }

    internal List<TBase>? Children { get; set; }

    internal string Source { get; } = source;

    internal bool Include { get; set; }

    public string? Hierarchy { get; internal set; }

    public int Depth { get; } = depth;

    ITree? ITree.Container { get => Container; }

    string ITree.Source { get => Source; }

    bool ITree.Include { get => Include; set => Include = value; }

    string? ITree.Hierarchy
    {
        get => Hierarchy;
        set => Hierarchy = value;
    }

    int ITree.Depth { get => Depth; }

    public IEnumerable<ITree> Enumerate()
    {
        Stack<TreeBase<TContainer, TBase>> stack = [];
        stack.Push(this);

        while (stack.Count > 0)
        {
            TreeBase<TContainer, TBase> current = stack.Pop();

            yield return current;
            if (current.Children is null) continue;

            foreach (TBase child in current.Children)
            {
                if (child is TContainer container)
                {
                    stack.Push(container);
                    continue;
                }

                yield return child;
            }
        }
    }

    internal void AddChild(TBase child)
    {
        Children ??= [];
        Children.Add(child);
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
