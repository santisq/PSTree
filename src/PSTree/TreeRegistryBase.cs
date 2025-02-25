#if WINDOWS

namespace PSTree;

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

    private static string? GetPSParentPath(string? path) =>
        string.IsNullOrEmpty(path = System.IO.Path.GetDirectoryName(path))
            ? null : $"{_providerPath}{path}";
}
#endif
