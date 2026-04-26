using System;
using System.Collections.Generic;
using PSTree.Nodes;

namespace PSTree.Comparers;

#pragma warning disable CS8767

internal sealed class TreeFileSystemComparer : IComparer<TreeFileSystemInfo>
{
    private readonly FileSystemSortMode _mode;
    private static readonly TreeFileSystemComparer s_byDir = new(FileSystemSortMode.DirectoriesFirst);
    private static readonly TreeFileSystemComparer s_byFile = new(FileSystemSortMode.FilesFirst);
    private static readonly TreeFileSystemComparer s_bySize = new(FileSystemSortMode.Size);

    private TreeFileSystemComparer(FileSystemSortMode mode) => _mode = mode;

    public static TreeFileSystemComparer For(FileSystemSortMode mode) => mode switch
    {
        FileSystemSortMode.DirectoriesFirst => s_byDir,
        FileSystemSortMode.FilesFirst => s_byFile,
        FileSystemSortMode.Size => s_bySize,
        _ => throw new ArgumentOutOfRangeException(nameof(mode))
    };

    public int Compare(TreeFileSystemInfo x, TreeFileSystemInfo y) => _mode switch
    {
        FileSystemSortMode.FilesFirst => TreeComparers.ByLeavesFirst(x, y),
        FileSystemSortMode.DirectoriesFirst => TreeComparers.ByContainersFirst(x, y),
        FileSystemSortMode.Size => TreeComparers.BySize(x, y),
        _ => throw new ArgumentOutOfRangeException(nameof(_mode)) // Unreachable
    };
}
