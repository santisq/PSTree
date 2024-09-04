using System;
using System.IO;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;
using PSTree.Style;

namespace PSTree;

public abstract class PSTreeFileSystemInfo<T> : PSTreeFileSystemInfo
    where T : FileSystemInfo
{
    private PSObject? _pso;

    private PSObject InstancePso => _pso ??= PSObject.AsPSObject(Instance);

    protected T Instance { get; }

    public string Name => Instance.Name;

    public string Mode => FileSystemProvider.Mode(InstancePso);

    public string FullName => Instance.FullName;

    public string Extension => Instance.Extension;

    public FileAttributes Attributes => Instance.Attributes;

    public DateTime CreationTime => Instance.CreationTime;

    public DateTime CreationTimeUtc => Instance.CreationTimeUtc;

    public DateTime LastWriteTime => Instance.LastWriteTime;

    public DateTime LastWriteTimeUtc => Instance.LastWriteTimeUtc;

    public DateTime LastAccessTime => Instance.LastAccessTime;

    public DateTime LastAccessTimeUtc => Instance.LastAccessTimeUtc;

    private protected PSTreeFileSystemInfo(
        T fileSystemInfo, string hierarchy, int depth, string source) :
        base(hierarchy, source)
    {
        Instance = fileSystemInfo;
        Depth = depth;
    }

    private protected PSTreeFileSystemInfo(
        T fileSystemInfo, string hierarchy, string source) :
        base(hierarchy, source)
    {
        Instance = fileSystemInfo;
    }

    public bool HasFlag(FileAttributes flag) => Instance.Attributes.HasFlag(flag);

    public T GetUnderlyingObject() => Instance;

    public override string ToString() => FullName;

    public void Refresh() => Instance.Refresh();
}
