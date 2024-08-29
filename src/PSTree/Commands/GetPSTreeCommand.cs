using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace PSTree;

[Cmdlet(VerbsCommon.Get, "PSTree", DefaultParameterSetName = "Path")]
[OutputType(typeof(PSTreeDirectory), typeof(PSTreeFile))]
[Alias("pstree")]
public sealed partial class GetPSTreeCommand : PSCmdlet
{
    private bool _isLiteral;

    private bool _withExclude;

    private bool _withInclude;

    private string[]? _paths;

    private WildcardPattern[]? _excludePatterns;

    private WildcardPattern[]? _includePatterns;

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

    [Parameter]
    [SupportsWildcards]
    [ValidateNotNullOrEmpty]
    public string[]? Include { get; set; }

    protected override void BeginProcessing()
    {
        if (Recurse.IsPresent && !MyInvocation.BoundParameters.ContainsKey("Depth"))
        {
            Depth = int.MaxValue;
        }

        const WildcardOptions wpoptions =
            WildcardOptions.Compiled
            | WildcardOptions.CultureInvariant
            | WildcardOptions.IgnoreCase;

        if (Exclude is not null)
        {
            _excludePatterns = Exclude
                .Select(e => new WildcardPattern(e, wpoptions))
                .ToArray();

            _withExclude = true;
        }

        // this Parameter only targets files, there is no reason to use it
        // if -Directory is in use
        if (Include is not null && !Directory.IsPresent)
        {
            _includePatterns = Include
                .Select(e => new WildcardPattern(e, wpoptions))
                .ToArray();

            _withInclude = true;
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

        bool ShouldInclude(FileInfo file)
        {
            if (!_withInclude)
            {
                return true;
            }

            return _includePatterns.Any(e => e.IsMatch(file.FullName));
        }

        bool ShouldExclude(FileSystemInfo item)
        {
            if (!_withExclude)
            {
                return false;
            }

            return _excludePatterns.Any(e => e.IsMatch(item.FullName));
        }

        static IOrderedEnumerable<FileSystemInfo> GetSortedEnumerable(PSTreeDirectory treedir) =>
            treedir
                .EnumerateFileSystemInfos()
                .OrderBy(e => e is DirectoryInfo)
                .ThenBy(e => e, new FileSystemEntryComparer());

        while (_stack.Count > 0)
        {
            IOrderedEnumerable<FileSystemInfo> enumerator;
            PSTreeDirectory next = _stack.Pop();
            int level = next.Depth + 1;
            long size = 0;

            try
            {
                enumerator = GetSortedEnumerable(next);
                bool keepProcessing = level <= Depth;

                foreach (FileSystemInfo item in enumerator)
                {
                    if (!Force.IsPresent && item.IsHidden())
                    {
                        continue;
                    }

                    if (ShouldExclude(item))
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

                        if (keepProcessing && ShouldInclude(file))
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
}
