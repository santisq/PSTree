#if WINDOWS
using System;
using Microsoft.Win32;
using PSTree.Extensions;
using PSTree.Native;
using PSTree.Style;

namespace PSTree;

public sealed class TreeRegistryKey : TreeRegistryBase
{
    public string Kind { get; } = "RegistryKey";

    public override string Name { get; }

    public int SubKeyCount { get; }

    public int ValueCount { get; }

    public RegistryView View { get; }

    public DateTime? LastWriteTime { get; }

    internal TreeRegistryKey(
        RegistryKey key, string name, string source, int depth) :
        base(GetColoredName(name).Indent(depth), source, key.Name)
    {
        Name = name;
        Depth = depth;
        SubKeyCount = key.SubKeyCount;
        ValueCount = key.ValueCount;
        View = key.View;
        LastWriteTime = key.GetLastWriteTime();
    }

    internal TreeRegistryKey(
        RegistryKey key, string name, string source) :
        base(GetColoredName(name), source, key.Name)
    {
        Name = name;
        SubKeyCount = key.SubKeyCount;
        ValueCount = key.ValueCount;
        View = key.View;
        LastWriteTime = key.GetLastWriteTime();
    }

    private static string GetColoredName(string name) =>
        TreeStyle.Instance.Registry.GetColoredKey(name);
}
#endif
