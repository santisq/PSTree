using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;

namespace PSTree;

internal static class PSTreeStatic
{
    internal static string Indent(string inputString, int indentation)
    {
        return new string(' ', (4 * indentation) - 4) + "└── " + inputString;
    }

    // internal static void DrawTree(object[] inputObject)
    // {
    //     char corner = '└';
    //     char dash = '─';
    //     char pipe = '│';
    //     char lightVert = '├';
    //     string cornerConnect = string.Concat(corner, dash, dash, ' ');
    // }
}

public abstract class PSTreeFileSystemInfo<T>
    where T : FileSystemInfo
{
    private PSObject? _pso;

    private PSObject InstancePso => _pso ??= PSObject.AsPSObject(Instance);

    protected T Instance { get; }

    internal int Depth { get; set; }

    public string Mode => FileSystemProvider.Mode(InstancePso);

    public string Hierarchy { get; internal set; }

    public long Length { get; internal set; }

    public string Name => Instance.Name;

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
    {
        Instance  = fileSystemInfo;
        Depth     = depth;
        Hierarchy = PSTreeStatic.Indent(fileSystemInfo.Name, depth);
    }

    private protected PSTreeFileSystemInfo(T fileSystemInfo)
    {
        Instance  = fileSystemInfo;
        Hierarchy = fileSystemInfo.Name;
    }

    public bool HasFlag(FileAttributes flag) => Instance.Attributes.HasFlag(flag);
}

public sealed class PSTreeDirectory : PSTreeFileSystemInfo<DirectoryInfo>
{
    public DirectoryInfo Parent => Instance.Parent;

    internal PSTreeDirectory(DirectoryInfo directoryInfo, int depth) : base(directoryInfo, depth) { }

    internal PSTreeDirectory(DirectoryInfo directoryInfo) : base(directoryInfo) { }

    public IEnumerable<FileInfo> EnumerateFiles() =>
        Instance.EnumerateFiles();

    public IEnumerable<DirectoryInfo> EnumerateDirectories() =>
        Instance.EnumerateDirectories();

    public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos() =>
        Instance.EnumerateFileSystemInfos();

    internal IEnumerable<string> GetParents(Dictionary<string, PSTreeDirectory> map)
    {
        int index = -1;
        string path = Instance.FullName;

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

public sealed class PSTreeFile : PSTreeFileSystemInfo<FileInfo>
{
    public DirectoryInfo Directory => Instance.Directory;

    public string DirectoryName => Instance.DirectoryName;

    internal PSTreeFile(FileInfo fileInfo, int depth) : base(fileInfo, depth) {
        Length = fileInfo.Length;
    }
}

[Cmdlet(VerbsCommon.Get, "PSTree", DefaultParameterSetName = "Depth")]
[Alias("pstree")]
public sealed class PSTree : PSCmdlet
{
    private bool _isRecursive;

    [Parameter(ValueFromPipeline = true, Position = 0)]
    [Alias("PSPath")]
    public string? LiteralPath { get; set; }

    [Parameter(ParameterSetName = "Depth")]
    public int Depth = 3;

    [Parameter(ParameterSetName = "Recurse")]
    public SwitchParameter Recurse { get; set; }

    [Parameter]
    public SwitchParameter Force { get; set; }

    [Parameter]
    public SwitchParameter Directory { get; set; }

    [Parameter]
    public SwitchParameter RecursiveSize { get; set; }

    protected override void BeginProcessing()
    {
        _isRecursive = RecursiveSize.IsPresent || Recurse.IsPresent;
    }

    protected override void ProcessRecord()
    {
        string resolvedPath = GetUnresolvedProviderPathFromPSPath(LiteralPath);

        try
        {
            if(!File.GetAttributes(resolvedPath).HasFlag(FileAttributes.Directory))
            {
                new PSTreeFile(new FileInfo(resolvedPath), 0);
                return;
            }
        }
        catch(Exception except)
        {
            ThrowTerminatingError(
                new ErrorRecord(
                    except,
                    "PSTree.GetAttributes",
                    ErrorCategory.NotSpecified,
                    resolvedPath));
        }

        Dictionary<string, PSTreeDirectory> indexer = new();
        Stack<PSTreeDirectory> stack = new();
        List<PSTreeFile> files = new();
        stack.Push(new PSTreeDirectory(new DirectoryInfo(resolvedPath)));

        while(stack.Count > 0)
        {
            PSTreeDirectory next = stack.Pop();
            int level = next.Depth + 1;
            long size  = 0;

            try
            {
                IEnumerable<FileSystemInfo> enumerator = next.EnumerateFileSystemInfos();

                bool keepProcessing = _isRecursive || level <= Depth;

                foreach(FileSystemInfo item in enumerator)
                {
                    if(!Force.IsPresent && item.Attributes.HasFlag(FileAttributes.Hidden))
                    {
                        continue;
                    }

                    if(item is FileInfo file)
                    {
                        size += file.Length;

                        if(Directory.IsPresent)
                        {
                            continue;
                        }

                        files.Add(new PSTreeFile(file, level));
                        continue;
                    }

                    if(keepProcessing)
                    {
                        stack.Push(new PSTreeDirectory((DirectoryInfo) item, level));
                    }
                }

                next.Length = size;
                indexer[next.FullName] = next;

                if(RecursiveSize.IsPresent)
                {
                    foreach(string parent in next.GetParents(indexer))
                    {
                        indexer[parent].Length += size;
                    }
                }

                if(Recurse.IsPresent || next.Depth <= Depth)
                {
                    WriteObject(next);

                    if(files.Count > 0 && (Recurse.IsPresent || level <= Depth))
                    {
                        WriteObject(files.ToArray(), true);
                        files.Clear();
                    }
                }
            }
            catch(PipelineStoppedException)
            {
                throw;
            }
            catch(Exception except)
            {
                if(Recurse.IsPresent || next.Depth <= Depth) {
                    WriteObject(next);
                }

                WriteError(
                    new ErrorRecord(
                        except,
                        "PSTree.Enumerate",
                        ErrorCategory.NotSpecified,
                        next));
            }
        }
    }
}