using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace PSTree;

[Cmdlet(VerbsCommon.Get, "PSTree", DefaultParameterSetName = "Depth")]
[OutputType(typeof(PSTreeDirectory), typeof(PSTreeFile))]
[Alias("pstree")]
public sealed class PSTree : PSCmdlet
{
    private bool _isRecursive;

    private bool _isLiteral;

    private bool _withExclude;

    private string[]? _paths;

    private WildcardPattern[]? _excludePatterns;

    private readonly PSTreeIndexer _indexer = new();

    private readonly Stack<PSTreeDirectory> _stack = new();

    private readonly List<PSTreeFileSystemInfo> _result = new();

    private readonly List<PSTreeFile> _files = new();

    [Parameter(ParameterSetName = "Path", Position = 0, ValueFromPipeline = true)]
    [SupportsWildcards]
    public string[]? Path
    {
        get => _paths;
        set
        {
            _paths = value;
            _isLiteral = false;
        }
    }

    [Parameter(ParameterSetName = "LiteralPath", ValueFromPipelineByPropertyName = true)]
    [Alias("PSPath")]
    public string[]? LiteralPath
    {
        get => _paths;
        set
        {
            _paths = value;
            _isLiteral = true;
        }
    }

    [Parameter(ParameterSetName = "Depth")]
    [ValidateRange(0, int.MaxValue)]
    public int Depth = 3;

    [Parameter(ParameterSetName = "Recurse")]
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

    protected override void BeginProcessing()
    {
        _isRecursive = RecursiveSize.IsPresent || Recurse.IsPresent;

        if (Exclude is not null)
        {
            const WildcardOptions wpoptions =
                WildcardOptions.Compiled
                | WildcardOptions.CultureInvariant
                | WildcardOptions.IgnoreCase;

            _excludePatterns = Exclude
                .Select(e => new WildcardPattern(e, wpoptions))
                .ToArray();

            _withExclude = true;
        }
    }

    protected override void ProcessRecord()
    {
        _paths ??= new string[1] { SessionState.Path.CurrentLocation.Path };

        foreach ((string path, ProviderInfo provider) in _paths.NormalizePath(_isLiteral, this))
        {
            if (!provider.AssertFileSystem())
            {
                WriteError(ExceptionHelpers.NotFileSystemPathError(path, provider));
                continue;
            }

            if (path.AssertArchive())
            {
                WriteObject(new PSTreeFile(new FileInfo(path)));
                continue;
            }

            Traverse(new DirectoryInfo(path));
        }
    }

    private void Traverse(DirectoryInfo directory)
    {
        _indexer.Clear();
        _files.Clear();
        _result.Clear();
        _stack.Push(new PSTreeDirectory(directory));

        while (_stack.Count > 0)
        {
            PSTreeDirectory next = _stack.Pop();
            int level = next.Depth + 1;
            long size = 0;

            try
            {
                IEnumerable<FileSystemInfo> enumerator = next.EnumerateFileSystemInfos();

                bool keepProcessing = _isRecursive || level <= Depth;

                foreach (FileSystemInfo item in enumerator)
                {
                    if (!Force.IsPresent && item.Attributes.HasFlag(FileAttributes.Hidden))
                    {
                        continue;
                    }

                    if (_withExclude && _excludePatterns.Any(e => e.IsMatch(item.FullName)))
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

                        if (Recurse.IsPresent || level <= Depth)
                        {
                            _files.Add(new PSTreeFile(file, level));
                        }

                        continue;
                    }

                    if (keepProcessing)
                    {
                        _stack.Push(new PSTreeDirectory((DirectoryInfo)item, level));
                    }
                }

                next.Length = size;

                if (RecursiveSize.IsPresent)
                {
                    _indexer.Index(next, size);
                }

                if (Recurse.IsPresent || next.Depth <= Depth)
                {
                    _result.Add(next);

                    if (_files.Count > 0)
                    {
                        _result.AddRange(_files.ToArray());
                        _files.Clear();
                    }
                }
            }
            catch (PipelineStoppedException)
            {
                throw;
            }
            catch (Exception e)
            {
                if (Recurse.IsPresent || next.Depth <= Depth)
                {
                    _result.Add(next);
                }

                WriteError(new ErrorRecord(
                    e, "PSTree.Enumerate", ErrorCategory.NotSpecified, next));
            }
        }

        WriteObject(_result.ConvertToTree(), enumerateCollection: true);
    }
}
