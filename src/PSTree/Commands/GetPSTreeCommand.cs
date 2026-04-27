using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;
using PSTree.Comparers;
using PSTree.Extensions;
using PSTree.Nodes;

namespace PSTree.Commands;

[Cmdlet(VerbsCommon.Get, "PSTree", DefaultParameterSetName = PathSet)]
[OutputType(typeof(TreeDirectory), typeof(TreeFile))]
[Alias("pstree")]
public sealed class GetPSTreeCommand
    : TreeCommandBase<TreeDirectory, TreeFileSystemInfo, FileSystemSortMode>
{
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

            ProcessTree(new TreeDirectory(path));
        }
    }

    protected override void BuildOne(TreeDirectory current, int depth)
    {
        long accumulatedLength = 0;
        bool showThisLevel = depth <= Depth;
        bool shouldTraverse = showThisLevel || RecursiveSize;
        bool hasFile = false;

        try
        {
            foreach (FileSystemInfo item in current.EnumerateFileSystemInfos())
            {
                if (!Force && IsHidden(item) || ShouldExclude(item.Name))
                    continue;

                if (item is DirectoryInfo dir)
                {
                    TreeDirectory treedir = current.CreateDirectory(dir, CurrentSource);

                    if (showThisLevel)
                        current.AddChild(treedir);

                    if (shouldTraverse)
                        Push(treedir);

                    continue;
                }

                FileInfo file = (FileInfo)item;
                if (!shouldTraverse || !ShouldInclude(file.Name))
                    continue;

                accumulatedLength += file.Length;

                if (Directory || !showThisLevel)
                    continue;

                hasFile = true;
                TreeFile treefile = current.CreateFile(file, CurrentSource);
                current.AddChild(treefile);
            }

            current.AggregateUp(
                accumulatedLength,
                RecursiveSize,
                WithInclude && hasFile);
        }
        catch (Exception exception)
        {
            WriteError(exception.ToEnumerationError(current));
        }
    }

    private static bool IsHidden(FileSystemInfo item)
        => item.Attributes.HasFlag(FileAttributes.Hidden);

    protected override IComparer<TreeFileSystemInfo>? GetComparer() => SortBy switch
    {
        FileSystemSortMode.FilesFirst => TreeFileSystemComparer.ByFile,
        FileSystemSortMode.DirectoriesFirst => TreeFileSystemComparer.ByDirectory,
        FileSystemSortMode.Size => TreeFileSystemComparer.BySize,
        _ => null // None
    };
}
