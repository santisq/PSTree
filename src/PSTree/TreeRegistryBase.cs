namespace PSTree;

public abstract class TreeRegistryBase(
    string hierarchy,
    string source,
    string? path = null)
    : TreeBase(hierarchy, source)
{
    protected const string _providerPath = @"Microsoft.PowerShell.Core\Registry::";

    public string? Path { get; } = path;

    public virtual string? PSPath { get => field ??= GetPSPath(Path); }

    public virtual string? PSParentPath { get => field ??= GetPSParentPath(Path); }

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
