#if !WINDOWS
using System.Diagnostics.CodeAnalysis;
#endif

namespace PSTree;

#if !WINDOWS
[ExcludeFromCodeCoverage]
#endif
public abstract class TreeRegistryBase(
    string hierarchy,
    string source,
    string? path = null)
    : TreeBase(hierarchy, source)
{
    private string? _pspath;

    private string? _psparentpath;

    protected const string _providerPath = @"Microsoft.PowerShell.Core\Registry::";

    public string? Path { get; } = path;

    public virtual string? PSPath { get => _pspath ??= GetPSPath(Path); }

    public virtual string? PSParentPath { get => _psparentpath ??= GetPSParentPath(Path); }

    private static string? GetPSPath(string? path) =>
        string.IsNullOrEmpty(path) ? null : $"{_providerPath}{path}";

    private static string? GetPSParentPath(string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        path = System.IO.Path.GetDirectoryName(path);
        return string.IsNullOrEmpty(path) ? null : $"{_providerPath}{path}";
    }
}
