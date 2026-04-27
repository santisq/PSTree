using System.Collections.Generic;
using System.IO;

namespace PSTree.Nodes;

public sealed class TreeDirectory : TreeFileSystemInfo<DirectoryInfo>
{
    internal override bool IsContainer { get; } = true;

    public DirectoryInfo? Parent { get => Instance.Parent; }

    public int ItemCount { get; internal set; }

    public int TotalItemCount { get; internal set; }

    private TreeDirectory(DirectoryInfo dir, string source, int depth)
        : base(dir, source, depth)
    { }

    internal TreeDirectory(string path)
        : base(new DirectoryInfo(path), path)
    { }

    public IEnumerable<FileInfo> EnumerateFiles() =>
        Instance.EnumerateFiles();

    public IEnumerable<DirectoryInfo> EnumerateDirectories() =>
        Instance.EnumerateDirectories();

    public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos() =>
        Instance.EnumerateFileSystemInfos();

    internal TreeDirectory CreateDirectory(DirectoryInfo dir, string source)
        => new(dir, source, Depth + 1) { Container = this };

    internal TreeFile CreateFile(FileInfo file, string source)
        => new(file, source, Depth + 1) { Container = this };

    internal void AggregateUp(long length, bool recursive, bool include)
    {
        if (include) Include = include;
        ItemCount = Children?.Count ?? 0;
        TotalItemCount = ItemCount;
        Length = length;

        for (TreeDirectory? i = Container; i is not null; i = i.Container)
        {
            if (recursive) i.Length += length;
            if (include) i.Include = include;
            i.TotalItemCount += ItemCount;
        }
    }
}
