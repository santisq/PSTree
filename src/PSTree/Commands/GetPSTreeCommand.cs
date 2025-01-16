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

    private readonly Stack<PSTreeDirectory> _stack = new();

    private readonly Cache _cache = new();

    private readonly TreeComparer _comparer = new();

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

        if (Include is not null)
        {
            _includePatterns = [.. Include.Select(e => new WildcardPattern(e, options))];
        }
    }

    protected override void ProcessRecord()
    {
        foreach (string path in EnumerateResolvedPaths())
        {
            if (File.Exists(path))
            {
                WriteObject(PSTreeFile.Create(path));
                continue;
            }

            WriteObject(
                Traverse(PSTreeDirectory.Create(path)),
                enumerateCollection: true);
        }
    }

    private PSTreeFileSystemInfo[] Traverse(PSTreeDirectory directory)
    {
        _cache.Clear();
        _stack.Push(directory);
        string source = directory.FullName;

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
                    if (!Force.IsPresent && item.IsHidden())
                    {
                        continue;
                    }

                    if (ShouldExclude(item))
                    {
                        continue;
                    }

                    if (item is FileInfo fileInfo)
                    {
                        if (Directory.IsPresent)
                        {
                            continue;
                        }

                        if (keepProcessing && ShouldInclude(fileInfo))
                        {
                            childCount++;
                            size += fileInfo.Length;

                            PSTreeFile file = PSTreeFile
                                .Create(fileInfo, source, level)
                                .WithParent(next);

                            _cache.AddFile(file);
                        }

                        continue;
                    }

                    if (keepProcessing || RecursiveSize.IsPresent)
                    {
                        PSTreeDirectory dir = PSTreeDirectory
                            .Create((DirectoryInfo)item, source, level)
                            .WithParent(next);

                        if (_includePatterns is null)
                        {
                            dir._shouldInclude = true;
                            childCount++;
                        }

                        _stack.Push(dir);
                    }
                }

                next.Length = size;
                next.IndexItemCount(childCount);

                if (RecursiveSize.IsPresent)
                {
                    next.IndexLength(size);
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

    private static bool MatchAny(string name, WildcardPattern[] patterns)
    {
        foreach (WildcardPattern pattern in patterns)
        {
            if (pattern.IsMatch(name))
            {
                return true;
            }
        }

        return false;
    }

    private bool ShouldInclude(FileInfo item) =>
        _includePatterns is null || MatchAny(item.Name, _includePatterns);

    private bool ShouldExclude(FileSystemInfo item) =>
        _excludePatterns is not null && MatchAny(item.Name, _excludePatterns);
}
