using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;

namespace PSTree;

internal static class PSTreeStatic
{
    internal static string Indent(string inputString, int indentation)
    {
        return new string(' ', 4 * indentation) + inputString;
    }
}

internal sealed class PSTreeFile
{
    internal readonly FileInfo _instance;

    internal readonly int _depth;

    public string Hierarchy { get; internal set; }

    public long Length { get; internal set; }

    public string FullName { get; }

    internal PSTreeFile(FileInfo fileInfo, int depth)
    {
        _instance = fileInfo;
        _depth    = depth;
        Hierarchy = PSTreeStatic.Indent(fileInfo.Name, depth);
        Length    = fileInfo.Length;
        FullName  = fileInfo.FullName;
    }

    public bool HasFlag(FileAttributes flag)
    {
        return _instance.Attributes.HasFlag(flag);
    }
}

internal sealed class PSTreeDirectory
{
    internal readonly DirectoryInfo _instance;

    internal readonly int _depth;

    public string Hierarchy { get; internal set; }

    public long Length { get; internal set; }

    public string FullName { get; }

    internal PSTreeDirectory(DirectoryInfo directoryInfo, int depth)
    {
        _instance = directoryInfo;
        _depth    = depth;
        Hierarchy = PSTreeStatic.Indent(directoryInfo.Name, depth);
        FullName  = directoryInfo.FullName;
    }

    public bool HasFlag(FileAttributes flag)
    {
        return _instance.Attributes.HasFlag(flag);
    }

    public IEnumerable<FileInfo> EnumerateFiles() =>
        _instance.EnumerateFiles();

    public IEnumerable<DirectoryInfo> EnumerateDirectories() =>
        _instance.EnumerateDirectories();

    public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos() =>
        _instance.EnumerateFileSystemInfos();

    internal IEnumerable<string> GetParents(Dictionary<string, PSTreeDirectory> map)
    {
        int index = -1;
        string path = _instance.FullName;

        while((index = path.IndexOf(Path.DirectorySeparatorChar, index + 1)) != -1)
        {
            string parent = path.Substring(0, index);

            if(map.ContainsKey(parent))
            {
                yield return parent;
            }
        }
    }
}