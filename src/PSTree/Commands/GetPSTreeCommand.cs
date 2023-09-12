using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace PSTree;

[Cmdlet(VerbsCommon.Get, "PSTree", DefaultParameterSetName = "Path")]
[OutputType(typeof(PSTreeDirectory), typeof(PSTreeFile))]
[Alias("pstree")]
public sealed class GetPSTreeCommand : PSCmdlet
{
    private bool _isLiteral;

    private bool _withExclude;

    private string[]? _paths;

    private WildcardPattern[]? _excludePatterns;

    private readonly PSTreeIndexer _indexer = new();

    private readonly Stack<PSTreeDirectory> _stack = new();

    private readonly PSTreeCache _cache = new();

    [Parameter(
        ParameterSetName = "Path",
        Position = 0,
        ValueFromPipeline = true
    )]
    [SupportsWildcards]
    [ValidateNotNullOrEmpty]
    public string[]? Path
    {
        get => _paths;
        set
        {
            _paths = value;
            _isLiteral = false;
        }
    }

    [Parameter(
        ParameterSetName = "LiteralPath",
        ValueFromPipelineByPropertyName = true
    )]
    [Alias("PSPath")]
    [ValidateNotNullOrEmpty]
    public string[]? LiteralPath
    {
        get => _paths;
        set
        {
            _paths = value;
            _isLiteral = true;
        }
    }

    [Parameter]
    public uint Depth { get; set; } = 3;

    [Parameter]
    public SwitchParameter Recurse { get; set; }

    [Parameter]
    public SwitchParameter Force { get; set; }

    [Parameter]
    public SwitchParameter Directory { get; set; }

    [Parameter]
    public SwitchParameter RecursiveSize { get; set; }

    [Parameter]
    [SupportsWildcards]
    [ValidateNotNullOrEmpty]
    public string[]? Exclude { get; set; }

    protected override void BeginProcessing()
    {
        if (Recurse.IsPresent && !MyInvocation.BoundParameters.ContainsKey("Depth"))
        {
            Depth = int.MaxValue;
        }

        if (Exclude is not null)
        {
            const WildcardOptions wpoptions =
                WildcardOptions.Compiled
                | WildcardOptions.CultureInvariant
                | WildcardOptions.IgnoreCase;

            _excludePatterns = Exclude
                .Select(e => new WildcardPattern(e, wpoptions))
                .ToArray();

            _withExclude = true;
        }
    }

    protected override void ProcessRecord()
    {
        _paths ??= new[] { SessionState.Path.CurrentLocation.Path };

        foreach (string path in _paths.NormalizePath(_isLiteral, this))
        {
            string source = path.TrimExcess();

            if (source.IsArchive())
            {
                WriteObject(new PSTreeFile(new FileInfo(source), source));
                continue;
            }

            WriteObject(
                Traverse(new DirectoryInfo(source), source),
                enumerateCollection: true);
        }
    }

    private PSTreeFileSystemInfo[] Traverse(
        DirectoryInfo directory,
        string source)
    {
        _indexer.Clear();
        _cache.Clear();
        _stack.Push(new PSTreeDirectory(directory, source));

        while (_stack.Count > 0)
        {
            IEnumerable<FileSystemInfo> enumerator;
            PSTreeDirectory next = _stack.Pop();
            int level = next.Depth + 1;
            long size = 0;

            try
            {
                enumerator = next.EnumerateFileSystemInfos();
                bool keepProcessing = level <= Depth;

                foreach (FileSystemInfo item in enumerator)
                {
                    if (!Force.IsPresent && item.IsHidden())
                    {
                        continue;
                    }

                    if (_withExclude && ShouldExclude(item))
                    {
                        continue;
                    }

                    if (item is FileInfo file)
                    {
                        size += file.Length;

                        if (Directory.IsPresent)
                        {
                            continue;
                        }

                        if (keepProcessing)
                        {
                            _cache.AddFile(file, level, source);
                        }

                        continue;
                    }

                    if (keepProcessing || RecursiveSize.IsPresent)
                    {
                        _stack.Push(new PSTreeDirectory(
                            (DirectoryInfo)item, level, source));
                    }
                }

                next.Length = size;

                if (RecursiveSize.IsPresent)
                {
                    _indexer.Index(next, size);
                }

                if (next.Depth <= Depth)
                {
                    _cache.Add(next);
                    _cache.TryAddFiles();
                }
            }
            catch (Exception e) when (e is PipelineStoppedException or FlowControlException)
            {
                throw;
            }
            catch (Exception e)
            {
                if (next.Depth <= Depth)
                {
                    _cache.Add(next);
                }

                WriteError(ExceptionHelpers.EnumerationError(next, e));
            }
        }

        return _cache.GetTree();
    }

    private bool ShouldExclude(FileSystemInfo item) =>
        _excludePatterns.Any(e => e.IsMatch(item.FullName));
}
