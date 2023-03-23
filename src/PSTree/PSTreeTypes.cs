using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;

namespace PSTree;

public abstract class PSTreeFileSystemInfo
{
    internal int Depth { get; set; }

    public string Hierarchy { get; internal set; }

    protected PSTreeFileSystemInfo(string hierarchy) => Hierarchy = hierarchy;

    public long Length { get; internal set; }
}

public abstract class PSTreeFileSystemInfo<T> : PSTreeFileSystemInfo
    where T : FileSystemInfo
{
    private PSObject? _pso;

    private PSObject InstancePso => _pso ??= PSObject.AsPSObject(Instance);

    protected T Instance { get; }

    public string Name => Instance.Name;

    public string Mode => FileSystemProvider.Mode(InstancePso);

    public string Size => PSTreeStatic.FormatLength(Length);

    public string FullName => Instance.FullName;

    public string Extension => Instance.Extension;

    public FileAttributes Attributes => Instance.Attributes;

    public DateTime CreationTime => Instance.CreationTime;

    public DateTime CreationTimeUtc => Instance.CreationTimeUtc;

    public DateTime LastWriteTime => Instance.LastWriteTime;

    public DateTime LastWriteTimeUtc => Instance.LastAccessTimeUtc;

    public DateTime LastAccessTime => Instance.LastAccessTime;

    public DateTime LastAccessTimeUtc => Instance.LastAccessTimeUtc;

    private protected PSTreeFileSystemInfo(T fileSystemInfo, int depth)
        : base(PSTreeStatic.Indent(fileSystemInfo.Name, depth))
    {
        Instance  = fileSystemInfo;
        Depth     = depth;
    }

    private protected PSTreeFileSystemInfo(T fileSystemInfo)
        : base(fileSystemInfo.Name) => Instance = fileSystemInfo;

    public bool HasFlag(FileAttributes flag) => Instance.Attributes.HasFlag(flag);
}

public sealed class PSTreeDirectory : PSTreeFileSystemInfo<DirectoryInfo>
{
    public DirectoryInfo Parent => Instance.Parent;

    internal PSTreeDirectory(DirectoryInfo directoryInfo, int depth)
        : base(directoryInfo, depth) { }

    internal PSTreeDirectory(DirectoryInfo directoryInfo)
        : base(directoryInfo) { }

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

        while((index = path.IndexOf(Path.DirectorySeparatorChar, index + 1)) != -1)
        {
            yield return path.Substring(0, index);
        }
    }
}

public sealed class PSTreeFile : PSTreeFileSystemInfo<FileInfo>
{
    public DirectoryInfo Directory => Instance.Directory;

    public string DirectoryName => Instance.DirectoryName;

    internal PSTreeFile(FileInfo fileInfo, int depth) : base(fileInfo, depth) =>
        Length = fileInfo.Length;
}