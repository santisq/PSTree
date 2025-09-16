using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Management.Automation;
using PSTree.Extensions;

namespace PSTree;

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class TreeCommandBase : PSCmdlet
{
    private WildcardPattern[]? _excludePatterns;

    private WildcardPattern[]? _includePatterns;

    private string[]? _paths;

    protected const string PathSet = "Path";

    protected const string LiteralPathSet = "LiteralPath";

    protected bool WithExclude { get; private set; }

    protected bool WithInclude { get; private set; }

    protected bool IsLiteral
    {
        get => MyInvocation.BoundParameters.ContainsKey(nameof(LiteralPath));
    }

    protected bool Canceled { get; set; }

    [Parameter(
        ParameterSetName = PathSet,
        Position = 0,
        ValueFromPipeline = true)]
    [SupportsWildcards]
    [ValidateNotNullOrEmpty]
    public virtual string[]? Path
    {
        get => _paths;
        set => _paths = value;
    }

    [Parameter(
        ParameterSetName = LiteralPathSet,
        ValueFromPipelineByPropertyName = true)]
    [Alias("PSPath")]
    [ValidateNotNullOrEmpty]
    public virtual string[]? LiteralPath
    {
        get => _paths;
        set => _paths = value;
    }

    [Parameter]
    [ValidateRange(0, int.MaxValue)]
    [Alias("p", "dp")]
    public virtual int Depth { get; set; } = 3;

    [Parameter]
    [Alias("rec", "r")]
    public SwitchParameter Recurse { get; set; }

    [Parameter]
    [SupportsWildcards]
    [ValidateNotNullOrEmpty]
    [Alias("exc")]
    public string[]? Exclude { get; set; }

    [Parameter]
    [SupportsWildcards]
    [ValidateNotNullOrEmpty]
    [Alias("inc")]
    public string[]? Include { get; set; }

    protected override void BeginProcessing()
    {
        if (Recurse && !MyInvocation.BoundParameters.ContainsKey(nameof(Depth)))
        {
            Depth = int.MaxValue;
        }

        const WildcardOptions options = WildcardOptions.Compiled
            | WildcardOptions.CultureInvariant
            | WildcardOptions.IgnoreCase;

        if (Exclude is not null)
        {
            _excludePatterns = [.. Exclude.Select(e => new WildcardPattern(e, options))];
            WithExclude = true;
        }

        if (Include is not null)
        {
            _includePatterns = [.. Include.Select(e => new WildcardPattern(e, options))];
            WithInclude = true;
        }
    }

    protected override void StopProcessing() => Canceled = true;

    protected IEnumerable<(ProviderInfo, string)> EnumerateResolvedPaths()
    {
        Collection<string> resolvedPaths;
        ProviderInfo provider;

        foreach (string path in _paths ?? [SessionState.Path.CurrentLocation.Path])
        {
            if (IsLiteral)
            {
                string resolved = SessionState.Path.GetUnresolvedProviderPathFromPSPath(
                    path: path,
                    provider: out provider,
                    drive: out _);

                yield return (provider, resolved);
                continue;
            }

            try
            {
                resolvedPaths = GetResolvedProviderPathFromPSPath(path, out provider);
            }
            catch (Exception exception)
            {
                WriteError(exception.ToResolvePathError(path));
                continue;
            }

            foreach (string resolved in resolvedPaths)
            {
                yield return (provider, resolved);
            }
        }
    }

    private static bool MatchAny(
        string name,
        WildcardPattern[] patterns)
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

    protected bool ShouldInclude(string item) =>
        !WithInclude || MatchAny(item, _includePatterns!);

    protected bool ShouldExclude(string item) =>
        WithExclude && MatchAny(item, _excludePatterns!);
}
