using System.IO;
using PSTree.Style;

namespace PSTree;

public sealed class PSTreeFile : PSTreeFileSystemInfo<FileInfo>
{
    public DirectoryInfo Directory => Instance.Directory;

    public string DirectoryName => Instance.DirectoryName;

    internal PSTreeFile(FileInfo file, int depth, string source) :
        base(file, file.GetColoredName().Indent(depth), depth, source) =>
        Length = file.Length;

    internal PSTreeFile(FileInfo file, string source) :
        base(file, file.GetColoredName(), source) =>
        Length = file.Length;
}
