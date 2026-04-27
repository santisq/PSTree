using System.Collections.Generic;
using System.Text;
using PSTree.Extensions;
using PSTree.Interfaces;
using PSTree.Style;

namespace PSTree.Nodes;

public abstract class TreeBase<TContainer, TBase>(string source, int depth = 0) : ITree
    where TContainer : TBase
    where TBase : TreeBase<TContainer, TBase>
{
    internal abstract bool IsContainer { get; }

    internal TContainer? Container { get; set; }

    internal List<TBase>? Children { get; set; }

    internal string Source { get; } = source;

    internal bool Include { get; set; }

    public abstract string Name { get; }

    public string? Hierarchy { get; internal set; }

    public int Depth { get; } = depth;

    bool ITree.IsContainer { get => IsContainer; }
    ITree? ITree.Container { get => Container; }
    string ITree.Source { get => Source; }
    // bool ITree.Include { get => Include; set => Include = value; }
    string? ITree.Hierarchy { get => Hierarchy; set => Hierarchy = value; }
    int ITree.Depth { get => Depth; }
    string ITree.Name { get => Name; }

    private bool IsLast()
        => Container?.Children is { Count: > 0 } children
#if NET8_0_OR_GREATER
            && ReferenceEquals(this, children[^1]);
#else
            && ReferenceEquals(this, children[children.Count - 1]);
#endif

    internal IEnumerable<ITree> Render(
        int maxDepth,
        bool withInclude,
        IComparer<TBase>? comparer)
    {
        RenderingSet set = TreeStyle.Instance.RenderingSet;
        bool[] continues = new bool[maxDepth + 1];
        StringBuilder builder = new(256);
        Stack<TreeBase<TContainer, TBase>> stack = new(32);

        stack.Push(this);
        while (stack.Count > 0)
        {
            TreeBase<TContainer, TBase> current = stack.Pop();
            if (withInclude && !current.Include) continue;

            int dp = current.Depth;
            builder.Clear();

            if (dp > 0)
            {
                bool isLast = current.IsLast();
                for (int lev = 1; lev < dp; lev++)
                    builder.Append(continues[lev] ? set.Vertical : set.Space);

                builder.Append(isLast ? set.LastBranch : set.Branch);
                continues[dp] = !isLast;
            }

            builder.SetStyledName(current);
            yield return current;

            if (current.Children is not { Count: > 0 } children)
                continue;

            if (comparer is not null)
                children.Sort(comparer);

            for (int i = children.Count - 1; i >= 0; i--)
                stack.Push(children[i]);
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
