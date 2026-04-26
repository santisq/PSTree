using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;
using PSTree.Comparers;
using PSTree.Extensions;
using PSTree.Interfaces;
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

            WriteObject(
                Build(new TreeDirectory(path)),
                enumerateCollection: true);
        }
    }

    protected override IEnumerable<ITree> Build(TreeDirectory directory)
    {
        string source = directory.FullName;
        int maxDp = 0;

        Push(directory);
        while (ShouldContinue())
        {
            TreeDirectory current = Pop();

            long length = 0;
            int level = current.Depth + 1;
            maxDp = Math.Max(maxDp, level);

            try
            {
                bool include = level <= Depth;
                bool traverse = include || RecursiveSize;

                foreach (FileSystemInfo item in current.EnumerateFileSystemInfos())
                {
                    if (!Force && IsHidden(item) || ShouldExclude(item.Name))
                        continue;

                    if (item is DirectoryInfo dir)
                    {
                        TreeDirectory treedir = current.CreateDirectory(dir, source);
                        current.ItemCount++;

                        if (include) current.AddChild(treedir);
                        if (traverse) Push(treedir);
                        continue;
                    }

                    FileInfo file = (FileInfo)item;
                    if (traverse && ShouldInclude(file.Name))
                    {
                        length += file.Length;
                        if (Directory || !include) continue;

                        current.ItemCount++;
                        TreeFile treefile = current.CreateFile(file, source);
                        current.AddChild(treefile);
                    }
                }

                current.AggregateUp(
                    length: length,
                    recursive: RecursiveSize,
                    propagateInclude: WithInclude);
            }
            catch (Exception exception)
            {
                WriteError(exception.ToEnumerationError(current));
            }
        }

        // if (WithInclude)
        // {
        //     for (int i = _builder.Items.Count - 1; i >= 0; i--)
        //     {
        //         TreeFileSystemInfo current = _builder.Items[i];
        //         if (!current.Include) current.RecursiveDecrement();
        //     }
        // }

        return directory.Render(maxDp, Comparer);
        // return _builder.GetTree(WithInclude && !Directory, maxDp);
    }

    private static bool IsHidden(FileSystemInfo item)
        => item.Attributes.HasFlag(FileAttributes.Hidden);

    protected override IComparer<TreeFileSystemInfo> GetComparer() => SortBy switch
    {
        FileSystemSortMode.FilesFirst => TreeFileSystemComparer.ByFile,
        FileSystemSortMode.DirectoriesFirst => TreeFileSystemComparer.ByDirectory,
        FileSystemSortMode.Size => TreeFileSystemComparer.BySize,
        _ => throw new ArgumentOutOfRangeException(nameof(SortBy))
    };
}
