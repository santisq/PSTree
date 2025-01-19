using System.Collections.Generic;
using System.IO;
using System.Linq;
using PSTree.Extensions;
using PSTree.Style;

namespace PSTree;

public sealed class PSTreeDirectory : PSTreeFileSystemInfo<DirectoryInfo>
{
    public DirectoryInfo Parent => Instance.Parent;

    public int ItemCount { get; internal set; }

    public int TotalItemCount { get; internal set; }

    private PSTreeDirectory(
        DirectoryInfo dir, string hierarchy, string source, int depth)
        : base(dir, hierarchy, source, depth)
    { }

    private PSTreeDirectory(
        DirectoryInfo dir, string hierarchy, string source)
        : base(dir, hierarchy, source)
    {
        ShouldInclude = true;
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

    internal static PSTreeDirectory Create(string path) =>
        Create(new DirectoryInfo(path), path);

    internal static PSTreeDirectory Create(DirectoryInfo dir, string source)
    {
        string styled = TreeStyle.Instance.GetColoredName(dir);
        return new PSTreeDirectory(dir, styled, source);
    }

    internal static PSTreeDirectory Create(DirectoryInfo dir, string source, int depth)
    {
        string styled = TreeStyle.Instance.GetColoredName(dir).Indent(depth);
        return new PSTreeDirectory(dir, styled, source, depth);
    }

    internal PSTreeDirectory AddParent(PSTreeDirectory parent)
    {
        ParentNode = parent;
        return this;
    }

    internal void IndexCount(int count)
    {
        ItemCount = count;
        TotalItemCount = count;

        for (PSTreeDirectory? i = ParentNode; i is not null; i = i.ParentNode)
        {
            i.TotalItemCount += count;
        }
    }

    internal void IndexLength(long length)
    {
        for (PSTreeDirectory? i = ParentNode; i is not null; i = i.ParentNode)
        {
            i.Length += length;
        }
    }

    internal void SetIncludeFlag()
    {
        ShouldInclude = true;

        for (PSTreeDirectory? i = ParentNode; i is not null; i = i.ParentNode)
        {
            if (i.ShouldInclude)
            {
                return;
            }

            i.ShouldInclude = true;
        }
    }
}
