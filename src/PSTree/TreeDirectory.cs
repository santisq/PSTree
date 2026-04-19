using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PSTree;

public sealed class TreeDirectory : TreeFileSystemInfo<DirectoryInfo>
{
    public DirectoryInfo? Parent => Instance.Parent;

    public int ItemCount { get; internal set; }

    public int TotalItemCount { get; internal set; }

    internal TreeDirectory(
        DirectoryInfo dir, string source, int depth)
        : base(dir, source, depth)
    { }

    internal TreeDirectory(
        DirectoryInfo dir, string source)
        : base(dir, source)
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

    internal void IndexCount(int count)
    {
        ItemCount = count;
        TotalItemCount = count;

        for (TreeDirectory? i = ParentNode; i is not null; i = i.ParentNode)
        {
            i.TotalItemCount += count;
        }
    }

    internal void IndexLength(long length)
    {
        for (TreeDirectory? i = ParentNode; i is not null; i = i.ParentNode)
        {
            i.Length += length;
        }
    }

    internal TreeDirectory SetIncludeFlagIf(bool condition)
    {
        if (condition)
        {
            Include = true;
        }

        return this;
    }
}
