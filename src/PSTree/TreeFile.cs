using System.IO;

namespace PSTree;

public sealed class TreeFile : TreeFileSystemInfo<FileInfo>
{
    internal override bool Include { get; set; } = true;

    public DirectoryInfo? Directory => Instance.Directory;

    public string? DirectoryName => Instance.DirectoryName;

    internal TreeFile(
        FileInfo file, string source)
        : base(file, source)
    {
        Length = file.Length;
    }

    internal TreeFile(
        FileInfo file, string source, int depth)
        : base(file, source, depth)
    {
        Length = file.Length;
    }

    internal TreeFile SetIncludeFlagIf(bool condition)
    {
        if (condition) ParentNode!.SetIncludeFlag();
        return this;
    }
}
