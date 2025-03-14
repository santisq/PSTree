using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;
using PSTree.Extensions;

namespace PSTree.Commands;

[Cmdlet(VerbsCommon.Get, "PSTree", DefaultParameterSetName = PathSet)]
[OutputType(typeof(TreeDirectory), typeof(TreeFile))]
[Alias("pstree")]
public sealed class GetPSTreeCommand : TreeCommandBase
{
    private readonly Stack<TreeDirectory> _stack = new();

    private readonly Cache<TreeFileSystemInfo, TreeFile> _cache = new();

    private readonly TreeComparer _comparer = new();

    [Parameter]
    public SwitchParameter Force { get; set; }

    [Parameter]
    public SwitchParameter Directory { get; set; }

    [Parameter]
    public SwitchParameter RecursiveSize { get; set; }

    protected override void ProcessRecord()
    {
        foreach ((ProviderInfo provider, string path) in EnumerateResolvedPaths())
        {
            if (provider.ImplementingType != typeof(FileSystemProvider))
            {
                WriteError(provider.ToInvalidProviderError(path));
                continue;
            }

            if (File.Exists(path))
            {
                FileInfo file = new(path);
                if (!ShouldExclude(file.Name) && ShouldInclude(file.Name))
                {
                    WriteObject(TreeFile.Create(file, path));
                }

                continue;
            }

            if (!System.IO.Directory.Exists(path))
            {
                WriteError(path.ToInvalidPathError());
                continue;
            }

            WriteObject(
                Traverse(TreeDirectory.Create(path)),
                enumerateCollection: true);
        }
    }

    private TreeFileSystemInfo[] Traverse(TreeDirectory directory)
    {
        _cache.Clear();
        _stack.Push(directory);
        string source = directory.FullName;

        while (_stack.Count > 0)
        {
            TreeDirectory next = _stack.Pop();
            int level = next.Depth + 1;
            long totalLength = 0;

            try
            {
                bool keepProcessing = level <= Depth;
                foreach (FileSystemInfo item in next.GetSortedEnumerable(_comparer))
                {
                    if (!Force && IsHidden(item) || ShouldExclude(item.Name))
                    {
                        continue;
                    }

                    if (item is FileInfo fileInfo)
                    {
                        if (Directory)
                        {
                            totalLength += fileInfo.Length;
                            continue;
                        }

                        if (!ShouldInclude(fileInfo.Name))
                        {
                            continue;
                        }

                        if (!keepProcessing && !RecursiveSize)
                        {
                            continue;
                        }

                        totalLength += fileInfo.Length;

                        if (keepProcessing)
                        {
                            TreeFile file = TreeFile
                                .Create(fileInfo, source, level)
                                .AddParent(next)
                                .SetIncludeFlagIf(WithInclude);

                            _cache.Add(file);
                        }

                        continue;
                    }

                    if (!keepProcessing && !RecursiveSize)
                    {
                        continue;
                    }

                    TreeDirectory dir = TreeDirectory
                        .Create((DirectoryInfo)item, source, level)
                        .AddParent(next);

                    if (keepProcessing && Directory || !WithInclude)
                    {
                        dir.ShouldInclude = true;
                    }

                    _stack.Push(dir);
                }

                next.Length = totalLength;

                if (RecursiveSize)
                {
                    next.IndexLength(totalLength);
                }

                if (next.Depth <= Depth)
                {
                    _cache.Add(next);
                    _cache.Flush();
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

        return GetTree(WithInclude && !Directory);
    }

    private static bool IsHidden(FileSystemInfo item) =>
        item.Attributes.HasFlag(FileAttributes.Hidden);

    private TreeFileSystemInfo[] GetTree(bool condition)
    {
        TreeFileSystemInfo[] result = condition
            ? [.. _cache.Items.Where(static e => e.ShouldInclude)]
            : [.. _cache.Items];

        return result.Format(GetItemCount(result));
    }

    private static Dictionary<string, int> GetItemCount(TreeFileSystemInfo[] items)
    {
        Dictionary<string, int> counts = [];
        foreach (TreeFileSystemInfo item in items)
        {
            string? path = item.ParentNode?.FullName;
            if (path is not null && !counts.TryAdd(path, 1))
            {
                counts[path]++;
            }
        }

        return counts;
    }
}
