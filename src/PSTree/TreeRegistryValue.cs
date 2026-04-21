#if WINDOWS
using Microsoft.Win32;

namespace PSTree;

public sealed class TreeRegistryValue : TreeRegistryBase
{
    private readonly string _parentPath;

    private readonly string _valueName;

    public RegistryValueKind Kind { get; }

    public override string Name { get; }

    public override string? PSParentPath { get; }

    internal TreeRegistryValue(
        RegistryKey key,
        string value,
        string source,
        int depth)
        : base(source)
    {
        _parentPath = key.Name;
        _valueName = value;
        Depth = depth;
        Name = GetNameOrDefault(value);
        Kind = key.GetValueKind(value);
        PSParentPath = $"{ProviderPath}{_parentPath}";
        Include = true;
    }

    private static string GetNameOrDefault(string value) =>
        string.IsNullOrEmpty(value) ? "(Default)" : value;

    public object? GetValue() => Registry.GetValue(_parentPath, _valueName, null);

    internal TreeRegistryValue SetIncludeFlagIf(bool condition)
    {
        if (condition) Container!.SetIncludeFlag();
        return this;
    }
}
#endif
