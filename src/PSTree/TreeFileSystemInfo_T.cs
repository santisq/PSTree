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

    public override string Name { get => Instance.Name; }

    public string Mode { get => FileSystemProvider.Mode(InstanceAsPSO); }

    public string FullName { get => Instance.FullName; }

    public string Extension { get => Instance.Extension; }

    public FileAttributes Attributes { get => Instance.Attributes; }

    public DateTime CreationTime { get => Instance.CreationTime; }

    public DateTime CreationTimeUtc { get => Instance.CreationTimeUtc; }

    public DateTime LastWriteTime { get => Instance.LastWriteTime; }

    public DateTime LastWriteTimeUtc { get => Instance.LastWriteTimeUtc; }

    public DateTime LastAccessTime { get => Instance.LastAccessTime; }

    public DateTime LastAccessTimeUtc { get => Instance.LastAccessTimeUtc; }

    private protected TreeFileSystemInfo(T fileSystemInfo, string source, int depth = 0)
        : base(source, depth)
    {
        Instance = fileSystemInfo;
        // Depth = depth;
    }

    // private protected TreeFileSystemInfo(
    //     T fileSystemInfo, string source)
    //     : base(source)
    // {
    //     Instance = fileSystemInfo;
    // }

    public bool HasFlag(FileAttributes flag) => Instance.Attributes.HasFlag(flag);

    public T GetUnderlyingObject() => Instance;

    public override string ToString() => FullName;

    public void Refresh() => Instance.Refresh();
}
