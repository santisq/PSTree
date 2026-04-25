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
public sealed class GetPSTreeCommand : TreeCommandBase<TreeDirectory>
{
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

    protected override ITree Traverse(TreeDirectory directory)
    {
        _builder.Clear();
        Push(directory);

        string source = directory.FullName;
        int maxDp = 0;

        while (ShouldContinue())
        {
            TreeDirectory next = Pop();

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
                            TreeDirectory treedir = new(dir, source, level)
                            {
                                Container = next
                            };

                            next.AddChild(treedir);
                            Push(treedir);
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
                    TreeFile treefile = new(file, source, level)
                    {
                        Container = next
                    };

                    next.AddChild(treefile);
                }

                next.AggregateUp(
                    length: length,
                    recursive: RecursiveSize,
                    propagateInclude: WithInclude && _builder.HasLeaf());

                // next.Children?.Sort(TreeComparer.Value);
                // if (next.Depth <= Depth)
                // {
                //     _builder.Add(next);
                //     _builder.Flush();
                // }
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

        directory.MaxDepth = maxDp;
        return directory;
        // return _builder.GetTree(WithInclude && !Directory, maxDp);
    }

    // private IEnumerable<TreeFileSystemInfo> GetTree(TreeDirectory directory)
    // {
    //     const string Vertical = "│   ";
    //     const string Space = "    ";
    //     const string Branch = "├── ";
    //     const string LastBranch = "└── ";

    //     foreach (TreeFileSystemInfo info in directory.Enumerate())
    //     {

    //     }
    // }


    private static bool IsHidden(FileSystemInfo item)
        => item.Attributes.HasFlag(FileAttributes.Hidden);
}
