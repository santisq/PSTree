using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using PSTree.Extensions;

namespace PSTree.Commands;

[Cmdlet(VerbsCommon.Get, "PSTree", DefaultParameterSetName = PathSet)]
[OutputType(typeof(PSTreeDirectory), typeof(PSTreeFile))]
[Alias("pstree")]
public sealed class GetPSTreeCommand : CommandWithPathBase
{
    private WildcardPattern[]? _excludePatterns;

    private WildcardPattern[]? _includePatterns;

    private readonly PSTreeIndexer _indexer = new();

    private readonly Stack<PSTreeDirectory> _stack = new();

    private readonly PSTreeCache _cache = new();

    private readonly PSTreeComparer _comparer = new();

    [Parameter]
    [ValidateRange(0, int.MaxValue)]
    public int Depth { get; set; } = 3;

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

        const WildcardOptions options = WildcardOptions.Compiled
            | WildcardOptions.CultureInvariant
            | WildcardOptions.IgnoreCase;

        if (Exclude is not null)
        {
            _excludePatterns = [.. Exclude.Select(e => new WildcardPattern(e, options))];
        }

        // this Parameter only targets files, there is no reason to use it if -Directory is in use
        if (Include is not null && !Directory.IsPresent)
        {
            _includePatterns = [.. Include.Select(e => new WildcardPattern(e, options))];
        }
    }

    protected override void ProcessRecord()
    {
        foreach (string path in EnumerateResolvedPaths())
        {
            string source = path.TrimExcess();

            if (File.Exists(source))
            {
                WriteObject(PSTreeFile.Create(new FileInfo(source), source));
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
        _stack.Push(PSTreeDirectory.Create(directory, source));

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

                    if (item.ShouldExclude(_excludePatterns))
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

                        if (keepProcessing && file.ShouldInclude(_includePatterns))
                        {
                            _cache.AddFile(PSTreeFile.Create(file, source, level));
                        }

                        continue;
                    }

                    if (keepProcessing || RecursiveSize.IsPresent)
                    {
                        _stack.Push(PSTreeDirectory.Create(
                            (DirectoryInfo)item, source, level));
                    }
                }

                next.Length = size;
                _indexer[next.FullName] = next;
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
            catch (Exception exception)
            {
                if (next.Depth <= Depth)
                {
                    _cache.Add(next);
                }

                WriteError(exception.ToEnumerationError(next));
            }
        }

        return _cache.GetTree();
    }
}
