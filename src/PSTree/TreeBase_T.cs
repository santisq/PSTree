using System.Collections.Generic;
using System.Text;
using PSTree.Extensions;

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

    private bool IsLast()
        => Container?.Children is { Count: > 0 } children
            && ReferenceEquals(this, children[children.Count - 1]);

    public IEnumerable<ITree> Enumerate(int maxDepth)
    {
        const string Vertical = "│   ";
        const string Space = "    ";
        const string Branch = "├── ";
        const string LastBranch = "└── ";

        bool[] continues = new bool[maxDepth + 1];
        StringBuilder sb = new(256);
        Stack<TreeBase<TContainer, TBase>> stack = [];
        stack.Push(this);

        while (stack.Count > 0)
        {
            TreeBase<TContainer, TBase> current = stack.Pop();
            int depth = current.Depth;
            sb.Clear();

            bool isLast = false;
            if (depth > 0)
            {
                isLast = current.IsLast();
                for (int lev = 1; lev < depth; lev++)
                    sb.Append(continues[lev] ? Vertical : Space);

                sb.Append(isLast ? LastBranch : Branch);
            }

            current.Hierarchy = sb.GetStyledName(current);
            continues[depth] = !isLast;
            yield return current;

            if (current.Children is { Count: > 0 } children)
            {
                for (int i = children.Count - 1; i >= 0; i--)
                {
                    TBase child = children[i];
                    stack.Push(child);
                }
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
