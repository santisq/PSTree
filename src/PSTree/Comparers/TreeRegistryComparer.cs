#if WINDOWS
using System;
using System.Collections.Generic;
using PSTree.Nodes;

namespace PSTree.Comparers;

#pragma warning disable CS8767

internal sealed class TreeRegistryComparer : IComparer<TreeRegistryBase>
{
    private readonly RegistrySortMode _mode;

    internal static TreeRegistryComparer ByValue { get; } = new(RegistrySortMode.ValuesFirst);
    internal static TreeRegistryComparer ByKey { get; } = new(RegistrySortMode.KeysFirst);

    private TreeRegistryComparer(RegistrySortMode mode) => _mode = mode;

    public int Compare(TreeRegistryBase x, TreeRegistryBase y) => _mode switch
    {
        RegistrySortMode.ValuesFirst => TreeComparers.ByLeavesFirst(x, y),
        RegistrySortMode.KeysFirst => TreeComparers.ByContainersFirst(x, y),
        _ => throw new ArgumentOutOfRangeException(nameof(_mode)) // Unreachable
    };
}
#endif
