using System;
using System.Collections.Generic;
using System.Text;
using PSTree.Extensions;
#if NET8_0_OR_GREATER
using System.Runtime.InteropServices;
#endif

namespace PSTree;

internal sealed class TreeBuilder<TBase, TLeaf>
    where TLeaf : TBase
    where TBase : ITree
{
    private const string Vertical = "│   ";
    private const string Space = "    ";
    private const string Branch = "├── ";
    private const string LastBranch = "└── ";

    private readonly List<TLeaf> _leaves = [];

    internal List<TBase> Items { get; } = [];

    internal void Add(TLeaf leaf) => _leaves.Add(leaf);

    internal void Add(TBase container) => Items.Add(container);

    internal bool HasLeaf() => _leaves.Count > 0;

    internal TBase[] GetTree(bool withInclude, int maxDepth)
    {
        if (withInclude) Items.RemoveAll(e => !e.Include);

        StringBuilder sb = new(256);
        bool[] continues = new bool[maxDepth + 1];

#if NET8_0_OR_GREATER
        Span<TBase> tree = CollectionsMarshal.AsSpan(Items);
#else
        TBase[] tree = [.. Items];
#endif

        for (int i = 0; i < tree.Length; i++)
        {
            sb.Clear();

            ITree current = tree[i];
            int depth = current.Depth;

            int j = i + 1;
            while (j < tree.Length && tree[j].Depth > depth) j++;
            bool last = j == tree.Length || tree[j].Depth != depth;

            if (depth > 0)
            {
                for (int lev = 1; lev < depth; lev++)
                    sb.Append(continues[lev] ? Vertical : Space);

                sb.Append(last ? LastBranch : Branch);
            }

            current.Hierarchy = sb.GetStyledName(current);
            continues[depth] = !last;

            if (continues.Length > depth + 1)
                Array.Clear(continues, depth + 1, continues.Length - depth - 1);
        }

#if NET8_0_OR_GREATER
        return tree.ToArray();
#else
        return tree;
#endif
    }

    internal void Flush()
    {
        if (_leaves.Count > 0)
        {
            Items.AddRange([.. _leaves]);
            _leaves.Clear();
        }
    }

    internal void Clear()
    {
        _leaves.Clear();
        Items.Clear();
    }
}
