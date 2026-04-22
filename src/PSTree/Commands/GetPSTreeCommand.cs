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
                Traverse(new TreeDirectory(path)),
                enumerateCollection: true);
        }
    }

    private ITree[] Traverse(TreeDirectory directory)
    {
        _builder.Clear();
        directory.Push(_stack);

        string source = directory.FullName;
        int maxDp = 0;

        while (!Canceled && _stack.Count > 0)
        {
            TreeDirectory next = _stack.Pop();

            long length = 0;
            int level = next.Depth + 1;
            maxDp = Math.Max(maxDp, level);

            try
            {
                bool processLevel = level <= Depth;
                bool shouldContinue = processLevel || RecursiveSize;

                foreach (FileSystemInfo item in next.GetSortedEnumerable())
                {
                    if (!Force && IsHidden(item) || ShouldExclude(item.Name))
                        continue;

                    if (item is DirectoryInfo dir)
                    {
                        if (shouldContinue)
                        {
                            next.ItemCount++;
                            new TreeDirectory(dir, source, level)
                                .AddParent<TreeDirectory>(next)
                                .Push(_stack);
                        }

                        continue;
                    }

                    FileInfo file = (FileInfo)item;
                    if (Directory)
                    {
                        length += file.Length;
                        continue;
                    }

                    if (!shouldContinue || !ShouldInclude(file.Name))
                        continue;

                    length += file.Length;
                    if (!processLevel) continue;

                    next.ItemCount++;
                    new TreeFile(file, source, level)
                        .AddParent<TreeFile>(next)
                        .AddTo(_builder);
                }

                next.AggregateUp(
                    length: length,
                    recursive: RecursiveSize,
                    propagateInclude: WithInclude && _builder.HasLeaf());

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

        if (WithInclude)
        {
            for (int i = _builder.Items.Count - 1; i >= 0; i--)
            {
                TreeFileSystemInfo current = _builder.Items[i];
                if (!current.Include) current.RecursiveDecrement();
            }
        }

        return _builder.GetTree(WithInclude && !Directory, maxDp);
    }

    private static bool IsHidden(FileSystemInfo item)
        => item.Attributes.HasFlag(FileAttributes.Hidden);
}
