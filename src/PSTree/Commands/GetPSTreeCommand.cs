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
[OutputType(typeof(TreeDirectory), typeof(TreeFile), typeof(TreeSummary))]
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

    [Parameter]
    [Alias("t")]
    [ValidateRange(1, int.MaxValue)]
    public override int Top { get; set; }

    protected override void BeginProcessing()
    {
        if (MyInvocation.Uses(nameof(Top)))
        {
            (bool IsBound, bool IsValid) = CheckSortMode();

            if (!IsValid) this.ThrowIncompatibleSortError();

            RecursiveSize = true;
            if (!IsBound) SortBy = FileSystemSortMode.Size;
        }

        base.BeginProcessing();
    }

    private (bool IsBound, bool IsValid) CheckSortMode()
    {
        bool bound = MyInvocation.Uses(nameof(SortBy));
        bool valid = !bound || SortBy
            is FileSystemSortMode.Size
            or FileSystemSortMode.DirectoriesFirstBySize;

        return (bound, valid);
    }

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
                this.WriteInvalidPathError(path);
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
        string source = current.Source;

        try
        {
            foreach (FileSystemInfo item in current.EnumerateFileSystemInfos())
            {
                if (!Force && IsHidden(item) || ShouldExclude(item.Name))
                    continue;

                if (item is DirectoryInfo dir)
                {
                    TreeDirectory treedir = current.CreateDirectory(dir, source);

                    if (showThisLevel)
                        current.AddChild(treedir);

                    if (shouldTraverse && !IsReparsePoint(dir))
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
                TreeFile treefile = current.CreateFile(file, source);
                current.AddChild(treefile);
            }

            current.AggregateUp(
                accumulatedLength,
                RecursiveSize,
                HasInclude && hasFile);
        }
        catch (Exception exception)
        {
            WriteError(exception.ToEnumerationError(current));
        }
    }

    private static bool IsHidden(FileSystemInfo item)
        => item.Attributes.HasFlag(FileAttributes.Hidden);

    private static bool IsReparsePoint(DirectoryInfo dir)
        => dir.Attributes.HasFlag(FileAttributes.ReparsePoint);

    protected override IComparer<TreeFileSystemInfo>? GetComparer()
        => TreeComparerFactory.Get(SortBy);
}
