#if WINDOWS
namespace PSTree;

public abstract class TreeRegistryBase(string source, string? path = null)
    : TreeBase<TreeRegistryKey>(source)
{
    protected const string ProviderPath = @"Microsoft.PowerShell.Core\Registry::";

    public abstract string Name { get; }

    public string? Path { get; } = path;

    public virtual string? PSPath { get => field ??= GetPSPath(Path); }

    public virtual string? PSParentPath { get => field ??= GetPSParentPath(Path); }

    private static string? GetPSPath(string? path) =>
        string.IsNullOrEmpty(path) ? null : $"{ProviderPath}{path}";

    private static string? GetPSParentPath(string? path) =>
        string.IsNullOrEmpty(path = System.IO.Path.GetDirectoryName(path))
            ? null : $"{ProviderPath}{path}";
}
#endif
