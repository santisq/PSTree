using System.Collections.Generic;
using System.IO;
using System.Linq;
using PSTree.Extensions;
using PSTree.Style;

namespace PSTree;

public sealed class TreeDirectory : TreeFileSystemInfo<DirectoryInfo>
{
    public DirectoryInfo? Parent => Instance.Parent;

    public int ItemCount { get; internal set; }

    public int TotalItemCount { get; internal set; }

    private TreeDirectory(
        DirectoryInfo dir, string hierarchy, string source, int depth)
        : base(dir, hierarchy, source, depth)
    { }

    private TreeDirectory(
        DirectoryInfo dir, string hierarchy, string source)
        : base(dir, hierarchy, source)
    {
        Include = true;
    }

    public IEnumerable<FileInfo> EnumerateFiles() =>
        Instance.EnumerateFiles();

    public IEnumerable<DirectoryInfo> EnumerateDirectories() =>
        Instance.EnumerateDirectories();

    public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos() =>
        Instance.EnumerateFileSystemInfos();

    internal IOrderedEnumerable<FileSystemInfo> GetSortedEnumerable(TreeComparer comparer) =>
        Instance
            .EnumerateFileSystemInfos()
            .OrderBy(static e => e is DirectoryInfo)
            .ThenBy(static e => e, comparer);

    internal static TreeDirectory Create(string path) =>
        Create(new DirectoryInfo(path), path);

    internal static TreeDirectory Create(DirectoryInfo dir, string source)
    {
        string styled = TreeStyle.Instance.FileSystem.GetColoredName(dir);
        return new TreeDirectory(dir, styled, source);
    }

    internal static TreeDirectory Create(DirectoryInfo dir, string source, int depth)
    {
        string styled = TreeStyle.Instance.FileSystem.GetColoredName(dir).Indent(depth);
        return new TreeDirectory(dir, styled, source, depth);
    }

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
