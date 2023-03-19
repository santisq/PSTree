using System;
using System.IO;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;
using System.Text.RegularExpressions;

namespace PSTree;

internal static class PSTreeStatic
{
    internal static string[] _suffix;

    static PSTreeStatic() =>
        _suffix = new string[] { "Bytes", "Kb", "Mb", "Gb", "Tb", "Pb", "Eb", "Zb", "Yb" };

    internal static string Indent(string inputString, int indentation) =>
        new string(' ', (4 * indentation) - 4) + "└── " + inputString;

    internal static void DrawTree(List<PSTreeFileSystemInfo> inputObject)
    {
        Regex re = new(@"└|\S");

        for(int i = 0; i < inputObject.Count; i++)
        {
            int index = inputObject[i].Hierarchy.IndexOf('└');

            if(index >= 0)
            {
                int z = i - 1;
                while(!re.IsMatch(inputObject[z].Hierarchy[index].ToString()))
                {
                    char[] replace = inputObject[z].Hierarchy.ToCharArray();
                    replace[index] = '│';
                    inputObject[z].Hierarchy = new string(replace);
                    z--;
                }

                if(inputObject[z].Hierarchy[index] == '└')
                {
                    char[] replace = inputObject[z].Hierarchy.ToCharArray();
                    replace[index] = '├';
                    inputObject[z].Hierarchy = new string(replace);
                }
            }
        }
    }

    internal static string FormatLength(long length)
    {
        int index = 0;
        double len = length;

        while(len >= 1024) {
            len /= 1024;
            index++;
        }

        return string.Format("{0} {1}", Math.Round(len, 2), _suffix[index]);
    }
}

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

[Cmdlet(VerbsCommon.Get, "PSTree", DefaultParameterSetName = "Depth")]
[OutputType(typeof(PSTreeDirectory))]
[OutputType(typeof(PSTreeFile))]
public sealed class PSTree : PSCmdlet
{
    private bool _isRecursive;

    private Dictionary<string, PSTreeDirectory> _indexer = new();

    private readonly List<PSTreeFile> _files = new();

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

    protected override void BeginProcessing() =>
        _isRecursive = RecursiveSize.IsPresent || Recurse.IsPresent;

    protected override void ProcessRecord()
    {
        string resolvedPath = LiteralPath ?? GetUnresolvedProviderPathFromPSPath(string.Empty);

        if(resolvedPath != Path.GetPathRoot(resolvedPath))
        {
            resolvedPath = resolvedPath.TrimEnd(Path.DirectorySeparatorChar);
        }

        Stack<PSTreeDirectory> stack = new();
        List<PSTreeFileSystemInfo> result = new();

        try
        {
            var item = InvokeProvider.Item.Get(new string[1] { resolvedPath }, true, true)[0];

            if(item.BaseObject is not FileInfo && item.BaseObject is not DirectoryInfo)
            {
                ThrowTerminatingError(
                    new ErrorRecord(
                        new NotSupportedException("Not supported file system path."),
                        "PStree.NotSupported",
                        ErrorCategory.NotImplemented,
                        resolvedPath
                    ));
            }

            if(item.BaseObject is FileInfo file)
            {
                WriteObject(new PSTreeFile(file, 0));
                return;
            }

            stack.Push(new PSTreeDirectory((DirectoryInfo) item.BaseObject));
        }
        catch(Exception except)
        {
            ThrowTerminatingError(
                new ErrorRecord(
                    except,
                    "PSTree.GetItem",
                    ErrorCategory.NotSpecified,
                    resolvedPath));
        }

        if(RecursiveSize.IsPresent)
        {
            _indexer = new();
        }

        while(stack.Count > 0)
        {
            PSTreeDirectory next = stack.Pop();
            int level = next.Depth + 1;
            long size = 0;

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

                        if(Recurse.IsPresent || level <= Depth)
                        {
                            _files.Add(new PSTreeFile(file, level));
                        }

                        continue;
                    }

                    if(keepProcessing)
                    {
                        stack.Push(new PSTreeDirectory((DirectoryInfo) item, level));
                    }
                }

                next.Length = size;

                if(RecursiveSize.IsPresent)
                {
                    _indexer[next.FullName.TrimEnd(Path.DirectorySeparatorChar)] = next;

                    foreach(string parent in next.GetParents())
                    {
                        if(_indexer.ContainsKey(parent))
                        {
                            _indexer[parent].Length += size;
                        }
                    }
                }

                if(Recurse.IsPresent || next.Depth <= Depth)
                {
                    result.Add(next);

                    if(_files.Count > 0)
                    {
                        result.AddRange(_files.ToArray());
                        _files.Clear();
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
                    result.Add(next);
                }

                WriteError(
                    new ErrorRecord(
                        except,
                        "PSTree.Enumerate",
                        ErrorCategory.NotSpecified,
                        next));
            }
        }

        PSTreeStatic.DrawTree(result);
        WriteObject(result.ToArray(), true);

        if(_indexer.Count > 0)
        {
            _indexer.Clear();
        }

        if(_files.Count > 0)
        {
            _files.Clear();
        }
    }
}