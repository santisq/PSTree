#if WINDOWS
using Microsoft.Win32;
using PSTree.Extensions;
using PSTree.Style;

namespace PSTree;

public sealed class TreeRegistryValue : TreeRegistryBase
{
    private readonly string _parentPath;

    private readonly string _valueName;

    internal override bool Include { get; set; } = true;

    public RegistryValueKind Kind { get; }

    public override string Name { get; }

    public override string? PSParentPath { get; }

    internal TreeRegistryValue(
        RegistryKey key, string value, string source, int depth) :
        base(string.Empty, source)
    {
        _parentPath = key.Name;
        _valueName = value;
        Depth = depth;
        Name = GetNameOrDefault(value);
        Kind = key.GetValueKind(value);
        Hierarchy = GetColoredName(Name, Kind).Indent(depth);
        PSParentPath = $"{_providerPath}{_parentPath}";
    }

    private static string GetNameOrDefault(string value) =>
        string.IsNullOrEmpty(value) ? "(Default)" : value;

    public object? GetValue() => Registry.GetValue(_parentPath, _valueName, null);

    private static string GetColoredName(string name, RegistryValueKind kind) =>
        TreeStyle.Instance.Registry.GetColoredValue(name, kind);

    internal TreeRegistryValue SetIncludeFlagIf(bool condition)
    {
        if (condition)
        {
            ParentNode?.SetIncludeFlag();
        }

        return this;
    }
}
#endif
