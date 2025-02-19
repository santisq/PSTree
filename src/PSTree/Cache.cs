using System.Collections.Generic;

namespace PSTree;

internal sealed class Cache<TBase, TLeaf>
    where TLeaf : TBase
{
    internal List<TBase> Items { get; } = [];

    private readonly List<TLeaf> _leaves = [];

    internal void Add(TLeaf leaf) => _leaves.Add(leaf);

    internal void Add(TBase container) => Items.Add(container);

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
