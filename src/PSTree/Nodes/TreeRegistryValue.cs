#if WINDOWS
using Microsoft.Win32;

namespace PSTree.Nodes;

public sealed class TreeRegistryValue : TreeRegistryBase
{
    private readonly string _parentPath;

    private readonly string _valueName;

    internal override bool Include { get; set; } = true;

    internal override bool IsContainer { get; } = false;

    public RegistryValueKind Kind { get; }

    public override string Name { get; }

    public override string? PSParentPath { get; }

    internal TreeRegistryValue(
        TreeRegistryKey key,
        string value,
        string source,
        int depth)
        : base(source, depth)
    {
        _parentPath = key.Name;
        _valueName = value;
        Container = key;
        Name = GetNameOrDefault(value);
        Kind = key.GetValueKind(value);
        PSParentPath = $"{ProviderPath}{_parentPath}";
    }

    private static string GetNameOrDefault(string value) =>
        string.IsNullOrEmpty(value) ? "(Default)" : value;

    public object? GetValue()
        => Microsoft.Win32.Registry.GetValue(_parentPath, _valueName, null);
}
#endif
