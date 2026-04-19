using System;
using System.IO;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;

namespace PSTree;

public abstract class TreeFileSystemInfo<T> : TreeFileSystemInfo
    where T : FileSystemInfo
{
    private PSObject InstanceAsPSO { get => field ??= PSObject.AsPSObject(Instance); }

    protected T Instance { get; }

    public override string Name => Instance.Name;

    public string Mode => FileSystemProvider.Mode(InstanceAsPSO);

    public string FullName => Instance.FullName;

    public string Extension => Instance.Extension;

    public FileAttributes Attributes => Instance.Attributes;

    public DateTime CreationTime => Instance.CreationTime;

    public DateTime CreationTimeUtc => Instance.CreationTimeUtc;

    public DateTime LastWriteTime => Instance.LastWriteTime;

    public DateTime LastWriteTimeUtc => Instance.LastWriteTimeUtc;

    public DateTime LastAccessTime => Instance.LastAccessTime;

    public DateTime LastAccessTimeUtc => Instance.LastAccessTimeUtc;

    private protected TreeFileSystemInfo(
        T fileSystemInfo, string source, int depth)
        : base(source)
    {
        Instance = fileSystemInfo;
        Depth = depth;
    }

    private protected TreeFileSystemInfo(
        T fileSystemInfo, string source)
        : base(source)
    {
        Instance = fileSystemInfo;
    }

    public bool HasFlag(FileAttributes flag) => Instance.Attributes.HasFlag(flag);

    public T GetUnderlyingObject() => Instance;

    public override string ToString() => FullName;

    public void Refresh() => Instance.Refresh();
}
