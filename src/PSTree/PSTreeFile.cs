using System.IO;

namespace PSTree;

public sealed class PSTreeFile : PSTreeFileSystemInfo<FileInfo>
{
    public DirectoryInfo Directory => Instance.Directory;

    public string DirectoryName => Instance.DirectoryName;

    internal PSTreeFile(FileInfo fileInfo, int depth) :
        base(fileInfo, depth) => Length = fileInfo.Length;

    internal PSTreeFile(FileInfo fileInfo) :
        base(fileInfo) => Length = fileInfo.Length;
}
