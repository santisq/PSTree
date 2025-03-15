using System.Collections.Generic;
using System.Linq;

namespace PSTree;

internal sealed class Cache<TBase, TLeaf>
    where TLeaf : TBase
    where TBase : ITree
{
    private readonly List<TBase> _items = [];

    private readonly List<TLeaf> _leaves = [];

    internal void Add(TLeaf leaf) => _leaves.Add(leaf);

    internal void Add(TBase container) => _items.Add(container);

    internal TBase[] GetResult(bool includeCondition) =>
        includeCondition
            ? [.. _items.Where(static e => e.Include)]
            : [.. _items];

    internal void Flush()
    {
        if (_leaves.Count > 0)
        {
            _items.AddRange([.. _leaves]);
            _leaves.Clear();
        }
    }

    internal void Clear()
    {
        _leaves.Clear();
        _items.Clear();
    }
}
