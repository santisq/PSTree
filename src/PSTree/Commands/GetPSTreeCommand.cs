using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace PSTree;

[Cmdlet(VerbsCommon.Get, "PSTree", DefaultParameterSetName = "Path")]
[OutputType(typeof(PSTreeDirectory), typeof(PSTreeFile))]
[Alias("pstree")]
public sealed class GetPSTreeCommand : PSCmdlet
{
    private bool _isLiteral;

    private bool _withExclude;

    private string[]? _paths;

    private readonly Regex _re = new(@"[\\/]$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private WildcardPattern[]? _excludePatterns;

    private readonly PSTreeIndexer _indexer = new();

    private readonly Stack<PSTreeDirectory> _stack = new();

    private readonly PSTreeHelper _helper = new();

    [Parameter(
        ParameterSetName = "Path",
        Position = 0,
        ValueFromPipeline = true
    )]
    [SupportsWildcards]
    [ValidateNotNullOrEmpty]
    public string[]? Path
    {
        get => _paths;
        set
        {
            _paths = value;
            _isLiteral = false;
        }
    }

    [Parameter(
        ParameterSetName = "LiteralPath",
        ValueFromPipelineByPropertyName = true
    )]
    [Alias("PSPath")]
    [ValidateNotNullOrEmpty]
    public string[]? LiteralPath
    {
        get => _paths;
        set
        {
            _paths = value;
            _isLiteral = true;
        }
    }

    [Parameter]
    [ValidateRange(0, int.MaxValue)]
    public int Depth = 3;

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

    protected override void BeginProcessing()
    {
        if (Recurse.IsPresent && !MyInvocation.BoundParameters.ContainsKey("Depth"))
        {
            Depth = int.MaxValue;
        }

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

        foreach (string path in _paths.NormalizePath(_isLiteral, this))
        {
            string source = _re.Replace(path, "");

            if (path.IsArchive())
            {
                WriteObject(new PSTreeFile(new FileInfo(path), source));
                continue;
            }

            WriteObject(
                Traverse(new DirectoryInfo(path), source),
                enumerateCollection: true);
        }
    }

    private PSTreeFileSystemInfo[] Traverse(
        DirectoryInfo directory,
        string source)
    {
        _indexer.Clear();
        _helper.Clear();
        _stack.Push(new PSTreeDirectory(directory, source));

        while (_stack.Count > 0)
        {
            IEnumerable<FileSystemInfo> enumerator;
            PSTreeDirectory next = _stack.Pop();
            int level = next.Depth + 1;
            long size = 0;

            try
            {
                enumerator = next.EnumerateFileSystemInfos();
                bool keepProcessing = level <= Depth;

                foreach (FileSystemInfo item in enumerator)
                {
                    if (!Force.IsPresent && item.IsHidden())
                    {
                        continue;
                    }

                    if (_withExclude && ShouldExclude(item))
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

                        if (keepProcessing)
                        {
                            _helper.AddFile(file, level, source);
                        }

                        continue;
                    }

                    if (keepProcessing || RecursiveSize.IsPresent)
                    {
                        _stack.Push(new PSTreeDirectory(
                            (DirectoryInfo)item, level, source));
                    }
                }

                next.Length = size;

                if (RecursiveSize.IsPresent)
                {
                    _indexer.Index(next, size);
                }

                if (next.Depth <= Depth)
                {
                    _helper.Add(next);
                    _helper.TryAddFiles();
                }
            }
            catch (PipelineStoppedException)
            {
                throw;
            }
            catch (Exception e)
            {
                if (next.Depth <= Depth)
                {
                    _helper.Add(next);
                }

                WriteError(ExceptionHelpers.EnumerationError(next, e));
            }
        }

        return _helper.GetResult();
    }

    private bool ShouldExclude(FileSystemInfo item) =>
        _excludePatterns.Any(e => e.IsMatch(item.FullName));
}
