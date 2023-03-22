using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace PSTree;

[Cmdlet(VerbsCommon.Get, "PSTree", DefaultParameterSetName = "Depth")]
[OutputType(typeof(PSTreeDirectory))]
[OutputType(typeof(PSTreeFile))]
public sealed class PSTree : PSCmdlet
{
    private bool _isRecursive;

    private WildcardPattern[]? _excludePatterns;

    private readonly Dictionary<string, PSTreeDirectory> _indexer = new();

    private readonly Stack<PSTreeDirectory> _stack = new();

    private readonly List<PSTreeFileSystemInfo> _result = new();

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

    [Parameter]
    [SupportsWildcards]
    public string[]? Exclude { get; set; }

    protected override void BeginProcessing()
    {
        _isRecursive = RecursiveSize.IsPresent || Recurse.IsPresent;

        if(Exclude is not null)
        {
            const WildcardOptions wpoptions =
                WildcardOptions.Compiled
                | WildcardOptions.CultureInvariant
                | WildcardOptions.IgnoreCase;

            _excludePatterns = Exclude.Select(e => new WildcardPattern(e, wpoptions)).ToArray();
        }
    }

    protected override void ProcessRecord()
    {
        _indexer.Clear();
        _files.Clear();
        _result.Clear();

        string resolvedPath = LiteralPath ?? GetUnresolvedProviderPathFromPSPath(string.Empty);

        if(resolvedPath != Path.GetPathRoot(resolvedPath))
        {
            resolvedPath = resolvedPath.TrimEnd(Path.DirectorySeparatorChar);
        }

        try
        {
            var item = InvokeProvider.Item.Get(new string[1] { resolvedPath }, true, true)[0];

            if(item.BaseObject is not FileInfo && item.BaseObject is not DirectoryInfo)
            {
                ThrowTerminatingError(new ErrorRecord(
                    new NotSupportedException("Not supported file system path."),
                    "PStree.NotSupported",
                    ErrorCategory.NotImplemented,
                    resolvedPath));
            }

            if(item.BaseObject is FileInfo file)
            {
                WriteObject(new PSTreeFile(file, 0));
                return;
            }

            _stack.Push(new PSTreeDirectory((DirectoryInfo) item.BaseObject));
        }
        catch(Exception e)
        {
            ThrowTerminatingError(new ErrorRecord(
                e, "PSTree.GetItem", ErrorCategory.NotSpecified, resolvedPath));
        }

        while(_stack.Count > 0)
        {
            PSTreeDirectory next = _stack.Pop();
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

                    if (Exclude is not null && _excludePatterns.Any(e => e.IsMatch(item.FullName)))
                    {
                        continue;
                    }

                    if (item is FileInfo file)
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
                        _stack.Push(new PSTreeDirectory((DirectoryInfo) item, level));
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
                    _result.Add(next);

                    if(_files.Count > 0)
                    {
                        _result.AddRange(_files.ToArray());
                        _files.Clear();
                    }
                }
            }
            catch(PipelineStoppedException)
            {
                throw;
            }
            catch(Exception e)
            {
                if(Recurse.IsPresent || next.Depth <= Depth) {
                    _result.Add(next);
                }

                WriteError(new ErrorRecord(
                    e, "PSTree.Enumerate", ErrorCategory.NotSpecified, next));
            }
        }

        PSTreeStatic.DrawTree(_result);
        WriteObject(_result.ToArray(), true);
    }
}