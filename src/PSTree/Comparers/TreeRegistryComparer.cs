#if WINDOWS
using System;
using System.Collections.Generic;
using PSTree.Nodes;

namespace PSTree.Comparers;

#pragma warning disable CS8767

internal sealed class TreeRegistryComparer : IComparer<TreeRegistryBase>
{
    private readonly RegistrySortMode _mode;
    private static readonly TreeRegistryComparer s_byKey = new(RegistrySortMode.KeysFirst);
    private static readonly TreeRegistryComparer s_byValue = new(RegistrySortMode.ValuesFirst);

    private TreeRegistryComparer(RegistrySortMode mode) => _mode = mode;

    public static TreeRegistryComparer For(RegistrySortMode mode) => mode switch
    {
        RegistrySortMode.KeysFirst => s_byKey,
        RegistrySortMode.ValuesFirst => s_byValue,
        _ => throw new ArgumentOutOfRangeException(nameof(mode))
    };

    public int Compare(TreeRegistryBase x, TreeRegistryBase y)
    {
        return _mode switch
        {
            RegistrySortMode.KeysFirst => TreeComparers.ByContainersFirst(x, y),
            RegistrySortMode.ValuesFirst => TreeComparers.ByLeavesFirst(x, y),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
#endif
