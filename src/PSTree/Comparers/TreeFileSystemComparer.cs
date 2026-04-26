using System;
using System.Collections.Generic;
using PSTree.Nodes;

namespace PSTree.Comparers;

#pragma warning disable CS8767

internal sealed class TreeFileSystemComparer : IComparer<TreeFileSystemInfo>
{
    private readonly FileSystemSortMode _mode;

    internal static TreeFileSystemComparer ByFile { get; } = new(FileSystemSortMode.FilesFirst);
    internal static TreeFileSystemComparer ByDirectory { get; } = new(FileSystemSortMode.DirectoriesFirst);
    internal static TreeFileSystemComparer BySize { get; } = new(FileSystemSortMode.Size);

    private TreeFileSystemComparer(FileSystemSortMode mode) => _mode = mode;

    public int Compare(TreeFileSystemInfo x, TreeFileSystemInfo y) => _mode switch
    {
        FileSystemSortMode.FilesFirst => TreeComparers.ByLeavesFirst(x, y),
        FileSystemSortMode.DirectoriesFirst => TreeComparers.ByContainersFirst(x, y),
        FileSystemSortMode.Size => TreeComparers.BySize(x, y),
        _ => throw new ArgumentOutOfRangeException(nameof(_mode)) // Unreachable
    };
}
