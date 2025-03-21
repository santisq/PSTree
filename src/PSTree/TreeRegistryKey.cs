#if WINDOWS
using Microsoft.Win32;
using PSTree.Extensions;
using PSTree.Style;

namespace PSTree;

public sealed class TreeRegistryKey : TreeRegistryBase
{
    public string Kind { get; } = "RegistryKey";

    public override string Name { get; }

    public int SubKeyCount { get; }

    public int ValueCount { get; }

    public RegistryView View { get; }

    internal TreeRegistryKey(
        RegistryKey key, string name, string source, int depth) :
        base(GetColoredName(name).Indent(depth), source, key.Name)
    {
        Name = name;
        Depth = depth;
        SubKeyCount = key.SubKeyCount;
        ValueCount = key.ValueCount;
        View = key.View;
    }

    internal TreeRegistryKey(
        RegistryKey key, string name, string source) :
        base(GetColoredName(name), source, key.Name)
    {
        Name = name;
        SubKeyCount = key.SubKeyCount;
        ValueCount = key.ValueCount;
        View = key.View;
    }

    private static string GetColoredName(string name) =>
        TreeStyle.Instance.Registry.GetColoredKey(name);
}
#endif
