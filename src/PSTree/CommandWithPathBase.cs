using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Management.Automation;
using PSTree.Extensions;

namespace PSTree;

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class CommandWithPathBase : PSCmdlet
{
    protected const string PathSet = "Path";

    protected const string LiteralPathSet = "LiteralPath";

    protected string[]? _paths;

    protected bool IsLiteral
    {
        get => MyInvocation.BoundParameters.ContainsKey("LiteralPath");
    }

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

    protected IEnumerable<string> EnumerateResolvedPaths()
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

                if (!provider.IsFileSystem())
                {
                    WriteError(provider.ToInvalidProviderError(resolved));
                    continue;
                }

                if (!resolved.Exists())
                {
                    WriteError(resolved.ToInvalidPathError());
                    continue;
                }

                yield return resolved;
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
                if (!provider.IsFileSystem())
                {
                    WriteError(provider.ToInvalidProviderError(resolved));
                    continue;
                }

                yield return resolved;
            }
        }
    }
}
