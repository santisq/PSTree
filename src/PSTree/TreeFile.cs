using System.IO;

namespace PSTree;

public sealed class TreeFile : TreeFileSystemInfo<FileInfo>
{
    public DirectoryInfo? Directory { get => Instance.Directory; }

    public string? DirectoryName { get => Instance.DirectoryName; }

    internal TreeFile(FileInfo file, string source)
        : base(file, source)
    {
        Length = file.Length;
        Include = true;
    }

    internal TreeFile(FileInfo file, string source, int depth)
        : base(file, source, depth)
    {
        Length = file.Length;
        Include = true;
    }

    internal TreeFile PropagateIncludeFlagIf(bool condition)
    {
        if (condition) Container!.SetIncludeFlag();
        return this;
    }
}
