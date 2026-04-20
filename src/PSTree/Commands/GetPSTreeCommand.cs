using System;
using System.Collections.Generic;
using System.IO;
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

    private readonly TreeBuilder<TreeFileSystemInfo, TreeFile> _builder = new();

    [Parameter]
    [Alias("f")]
    public SwitchParameter Force { get; set; }

    [Parameter]
    [Alias("dir", "d")]
    public SwitchParameter Directory { get; set; }

    [Parameter]
    [Alias("rs")]
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
                    WriteObject(new TreeFile(file, path));
                }

                continue;
            }

            if (!System.IO.Directory.Exists(path))
            {
                WriteError(path.ToInvalidPathError());
                continue;
            }

            WriteObject(
                Traverse(new TreeDirectory(new DirectoryInfo(path), path)),
                enumerateCollection: true);
        }
    }

    private ITree[] Traverse(TreeDirectory directory)
    {
        _builder.Clear();
        directory.Push(_stack);
        string source = directory.FullName;
        int maxDepth = 0;

        while (!Canceled && _stack.Count > 0)
        {
            TreeDirectory next = _stack.Pop();
            int childCount = 0;
            int level = next.Depth + 1;
            long totalLength = 0;
            maxDepth = Math.Max(maxDepth, level);

            try
            {
                bool processLevel = level <= Depth;
                bool shouldContinue = processLevel || RecursiveSize;

                foreach (FileSystemInfo item in next.GetSortedEnumerable())
                {
                    if (!Force && IsHidden(item) || ShouldExclude(item.Name))
                        continue;

                    if (item is FileInfo fileInfo)
                    {
                        if (Directory)
                        {
                            totalLength += fileInfo.Length;
                            continue;
                        }

                        if (!shouldContinue || !ShouldInclude(fileInfo.Name))
                            continue;

                        totalLength += fileInfo.Length;

                        if (processLevel)
                        {
                            childCount++;

                            new TreeFile(fileInfo, source, level)
                                .AddParent<TreeFile>(next)
                                .SetIncludeFlagIf(WithInclude)
                                .AddTo(_builder);
                        }

                        continue;
                    }

                    if (!shouldContinue) continue;

                    childCount++;

                    new TreeDirectory((DirectoryInfo)item, source, level)
                        .AddParent<TreeDirectory>(next)
                        .SetIncludeFlagIf(processLevel && Directory || !WithInclude)
                        .Push(_stack);
                }

                next.Length = totalLength;
                next.IndexCount(childCount);

                if (RecursiveSize) next.IndexLength(totalLength);

                if (next.Depth <= Depth)
                {
                    _builder.Add(next);
                    _builder.Flush();
                }
            }
            catch (Exception exception)
            {
                if (next.Depth <= Depth) _builder.Add(next);
                WriteError(exception.ToEnumerationError(next));
            }
        }

        return _builder.GetTree(WithInclude && !Directory, maxDepth);
    }

    private static bool IsHidden(FileSystemInfo item)
        => item.Attributes.HasFlag(FileAttributes.Hidden);
}
