using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PSTree;

public sealed class TreeDirectory : TreeFileSystemInfo<DirectoryInfo>
{
    public DirectoryInfo? Parent { get => Instance.Parent; }

    public int ItemCount { get; internal set; }

    public int TotalItemCount { get; internal set; }

    internal TreeDirectory(DirectoryInfo dir, string source, int depth)
        : base(dir, source, depth)
    { }

    internal TreeDirectory(string path)
        : base(new DirectoryInfo(path), path)
    {
        Include = true;
    }

    public IEnumerable<FileInfo> EnumerateFiles() =>
        Instance.EnumerateFiles();

    public IEnumerable<DirectoryInfo> EnumerateDirectories() =>
        Instance.EnumerateDirectories();

    public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos() =>
        Instance.EnumerateFileSystemInfos();

    internal IOrderedEnumerable<FileSystemInfo> GetSortedEnumerable() =>
        Instance
            .EnumerateFileSystemInfos()
            .OrderBy(static e => e is DirectoryInfo)
            .ThenBy(static e => e, TreeComparer.Value);

    internal void AggregateUp(long length, bool recursive, bool propagateInclude)
    {
        TotalItemCount = ItemCount;
        Length = length;
        if (propagateInclude) Include = true;

        for (TreeDirectory? i = Container; i is not null; i = i.Container)
        {
            if (recursive) i.Length += length;
            if (propagateInclude) i.Include = true;
            i.TotalItemCount += ItemCount;
        }
    }
}
