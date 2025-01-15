using System.Collections.Generic;
using System.IO;
using System.Linq;
using PSTree.Extensions;
using PSTree.Style;

namespace PSTree;

public sealed class PSTreeDirectory : PSTreeFileSystemInfo<DirectoryInfo>
{
    private string[]? _parents;

    internal string[] Parents { get => _parents ??= GetParents(FullName); }

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
    { }

    public IEnumerable<FileInfo> EnumerateFiles() =>
        Instance.EnumerateFiles();

    public IEnumerable<DirectoryInfo> EnumerateDirectories() =>
        Instance.EnumerateDirectories();

    public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos() =>
        Instance.EnumerateFileSystemInfos();

    private static string[] GetParents(string path)
    {
        int index = -1;
        List<string> parents = [];

        while ((index = path.IndexOf(Path.DirectorySeparatorChar, index + 1)) != -1)
        {
            parents.Add(path.Substring(0, index));
        }

        return [.. parents];
    }

    internal IOrderedEnumerable<FileSystemInfo> GetSortedEnumerable(PSTreeComparer comparer) =>
        Instance
            .EnumerateFileSystemInfos()
            .OrderBy(static e => e is DirectoryInfo)
            .ThenBy(static e => e, comparer);

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
}
