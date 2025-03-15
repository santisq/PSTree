#if WINDOWS
using Microsoft.Win32;
using PSTree.Extensions;
using PSTree.Style;

namespace PSTree;

public sealed class TreeRegistryValue : TreeRegistryBase
{
    private readonly string _parentPath;

    internal override bool Include { get; set; } = true;

    public RegistryValueKind Kind { get; }

    public override string Name { get; }

    public override string? PSParentPath { get; }

    internal TreeRegistryValue(
        RegistryKey key, string value, string source, int depth) :
        base(string.Empty, source)
    {
        _parentPath = key.Name;
        Kind = key.GetValueKind(value);
        Hierarchy = GetColoredName(value, Kind).Indent(depth);
        Depth = depth;
        Name = value;
        PSParentPath = $"{_providerPath}{_parentPath}";
    }

    public object? GetValue() => Registry.GetValue(_parentPath, Name, null);

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
