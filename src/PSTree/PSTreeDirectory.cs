using System.Collections.Generic;
using System.IO;

namespace PSTree;

public sealed class PSTreeDirectory : PSTreeFileSystemInfo<DirectoryInfo>
{
    public DirectoryInfo Parent => Instance.Parent;

    internal PSTreeDirectory(DirectoryInfo directoryInfo, int depth, string source) :
        base(directoryInfo, depth, source)
    { }

    internal PSTreeDirectory(DirectoryInfo directoryInfo, string source) :
        base(directoryInfo, source)
    { }

    public IEnumerable<FileInfo> EnumerateFiles() =>
        Instance.EnumerateFiles();

    public IEnumerable<DirectoryInfo> EnumerateDirectories() =>
        Instance.EnumerateDirectories();

    public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos() =>
        Instance.EnumerateFileSystemInfos();

    public IEnumerable<string> GetParents()
    {
        int index = -1;
        string path = Instance.FullName;

        while ((index = path.IndexOf(Path.DirectorySeparatorChar, index + 1)) != -1)
        {
            yield return path.Substring(0, index);
        }
    }
}
