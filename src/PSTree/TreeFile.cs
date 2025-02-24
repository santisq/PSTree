using System.IO;
using PSTree.Extensions;
using PSTree.Style;

namespace PSTree;

public sealed class TreeFile : TreeFileSystemInfo<FileInfo>
{
    public DirectoryInfo? Directory => Instance.Directory;

    public string? DirectoryName => Instance.DirectoryName;

    private TreeFile(
        FileInfo file, string hierarchy, string source)
        : base(file, hierarchy, source)
    {
        Length = file.Length;
        ShouldInclude = true;
    }

    private TreeFile(
        FileInfo file, string hierarchy, string source, int depth)
        : base(file, hierarchy, source, depth)
    {
        Length = file.Length;
        ShouldInclude = true;
    }

    internal static TreeFile Create(FileInfo file, string source)
    {
        string styled = TreeStyle.Instance.FileSystem.GetColoredName(file);
        return new TreeFile(file, styled, source);
    }

    internal static TreeFile Create(FileInfo file, string source, int depth)
    {
        string styled = TreeStyle.Instance.FileSystem.GetColoredName(file).Indent(depth);
        return new TreeFile(file, styled, source, depth);
    }

    internal TreeFile AddParent(TreeDirectory parent)
    {
        ParentNode = parent;
        return this;
    }

    internal TreeFile SetIncludeFlagIf(bool condition)
    {
        if (condition)
        {
            ParentNode?.SetIncludeFlag();
        }

        return this;
    }
}
