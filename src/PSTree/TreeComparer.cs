using System.Collections.Generic;

namespace PSTree;

#pragma warning disable CS8767

internal sealed class TreeComparer : IComparer<TreeFileSystemInfo>
{
    internal static TreeComparer Value { get; } = new();

    public int Compare(TreeFileSystemInfo x, TreeFileSystemInfo y) => CompareByName(x, y);

    private static int CompareByName(TreeFileSystemInfo x, TreeFileSystemInfo y)
    {
        return (x, y) switch
        {
            (TreeDirectory, TreeDirectory) => y.Name.CompareTo(x.Name), // Directories in descending order
            (TreeFile, TreeFile) => x.Name.CompareTo(y.Name),           // Files in ascending order
            (TreeDirectory, _) => -1,                                   // Directories first
            _ => 1                                                      // Then Files
        };
    }
}
