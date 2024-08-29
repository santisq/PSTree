using System.Collections.Generic;
using System.IO;

namespace PSTree;

internal sealed class PSTreeComparer : IComparer<FileSystemInfo>
{
    public int Compare(FileSystemInfo x, FileSystemInfo y) =>
        x is DirectoryInfo && y is DirectoryInfo
            ? y.Name.CompareTo(x.Name)  // Directories in descending order
            : x.Name.CompareTo(y.Name); // Files in ascending order
}
