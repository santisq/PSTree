#if WINDOWS

namespace PSTree;

public abstract class TreeRegistryBase(string hierarchy, string source, string? path = null)
    : TreeBase<TreeRegistryKey>(hierarchy, source)
{
    private string? _pspath;

    private string? _psparentpath;

    protected const string ProviderPath = @"Microsoft.PowerShell.Core\Registry::";

    public abstract string Name { get; }

    public string? Path { get; } = path;

    public virtual string? PSPath { get => _pspath ??= GetPSPath(Path); }

    public virtual string? PSParentPath { get => _psparentpath ??= GetPSParentPath(Path); }

    private static string? GetPSPath(string? path) =>
        string.IsNullOrEmpty(path) ? null : $"{ProviderPath}{path}";

    private static string? GetPSParentPath(string? path) =>
        string.IsNullOrEmpty(path = System.IO.Path.GetDirectoryName(path))
            ? null : $"{ProviderPath}{path}";
}
#endif
