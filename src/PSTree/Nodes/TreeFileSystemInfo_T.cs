using System;
using System.IO;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;

namespace PSTree.Nodes;

public abstract class TreeFileSystemInfo<T>(T fsinfo, string source, int depth = 0)
    : TreeFileSystemInfo(source, depth) where T : FileSystemInfo
{
    private PSObject InstanceAsPSO { get => field ??= PSObject.AsPSObject(Instance); }

    protected T Instance { get; } = fsinfo;

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

    public bool HasFlag(FileAttributes flag) => Instance.Attributes.HasFlag(flag);

    public T GetUnderlyingObject() => Instance;

    public override string ToString() => FullName;

    public void Refresh() => Instance.Refresh();
}
