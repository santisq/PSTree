using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace PSTree.Commands;

[Cmdlet(VerbsCommon.Get, "PSTree", DefaultParameterSetName = "Path")]
[OutputType(typeof(PSTreeDirectory), typeof(PSTreeFile))]
[Alias("pstree")]
public sealed class GetPSTreeCommand : PSCmdlet
{
    private bool _isLiteral;

    private string[]? _paths;

    private WildcardPattern[]? _excludePatterns;

    private WildcardPattern[]? _includePatterns;

    private readonly PSTreeIndexer _indexer = new();

    private readonly Stack<PSTreeDirectory> _stack = new();

    private readonly PSTreeCache _cache = new();

    private readonly PSTreeComparer _comparer = new();

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
        }

        // this Parameter only targets files, there is no reason to use it if -Directory is in use
        if (Include is not null && !Directory.IsPresent)
        {
            _includePatterns = Include
                .Select(e => new WildcardPattern(e, wpoptions))
                .ToArray();
        }
    }

    protected override void ProcessRecord()
    {
        _paths ??= [SessionState.Path.CurrentLocation.Path];

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
            PSTreeDirectory next = _stack.Pop();
            int level = next.Depth + 1;
            long size = 0;
            int childCount = 0;

            try
            {
                bool keepProcessing = level <= Depth;
                foreach (FileSystemInfo item in next.GetSortedEnumerable(_comparer))
                {
                    childCount++;
                    if (!Force.IsPresent && item.IsHidden())
                    {
                        continue;
                    }

                    if (ShouldExclude(item, _excludePatterns))
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

                        if (keepProcessing && ShouldInclude(file, _includePatterns))
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
                _indexer.IndexItemCount(next, childCount);

                if (RecursiveSize.IsPresent)
                {
                    _indexer.IndexLength(next, size);
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

    private static bool MatchAny(
        FileSystemInfo item,
        WildcardPattern[] patterns)
    {
        foreach (WildcardPattern pattern in patterns)
        {
            if (pattern.IsMatch(item.FullName))
            {
                return true;
            }
        }

        return false;
    }

    private static bool ShouldInclude(
        FileInfo file,
        WildcardPattern[]? patterns)
    {
        if (patterns is null)
        {
            return true;
        }

        return MatchAny(file, patterns);
    }

    private static bool ShouldExclude(
        FileSystemInfo item,
        WildcardPattern[]? patterns)
    {
        if (patterns is null)
        {
            return false;
        }

        return MatchAny(item, patterns);
    }
}
