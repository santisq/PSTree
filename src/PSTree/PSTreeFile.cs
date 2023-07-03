using System.IO;

namespace PSTree;

public sealed class PSTreeFile : PSTreeFileSystemInfo<FileInfo>
{
    public DirectoryInfo Directory => Instance.Directory;

    public string DirectoryName => Instance.DirectoryName;

    internal PSTreeFile(FileInfo fileInfo, int depth, string source) :
        base(fileInfo, depth, source) =>
        Length = fileInfo.Length;

    internal PSTreeFile(FileInfo fileInfo, string source) :
        base(fileInfo, source) =>
        Length = fileInfo.Length;
}
