using System;
using System.IO;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;

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

    public DateTime LastWriteTimeUtc => Instance.LastAccessTimeUtc;

    public DateTime LastAccessTime => Instance.LastAccessTime;

    public DateTime LastAccessTimeUtc => Instance.LastAccessTimeUtc;

    private protected PSTreeFileSystemInfo(T fileSystemInfo, int depth) :
        base(fileSystemInfo.Name.Indent(depth))
    {
        Instance = fileSystemInfo;
        Depth = depth;
    }

    private protected PSTreeFileSystemInfo(T fileSystemInfo) :
        base(fileSystemInfo.Name) => Instance = fileSystemInfo;

    public bool HasFlag(FileAttributes flag) => Instance.Attributes.HasFlag(flag);
}
