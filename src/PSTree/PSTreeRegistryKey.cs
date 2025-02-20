using System.ComponentModel.Design;
using Microsoft.Win32;
using PSTree.Extensions;

namespace PSTree;

public sealed class PSTreeRegistryKey : PSTreeRegistryBase
{
    public string Kind { get; } = "RegistryKey";

    public int SubKeyCount { get; }

    public int ValueCount { get; }

    public RegistryView View { get; }

    internal PSTreeRegistryKey(
        RegistryKey key, string name, string source, int depth) :
        base(name.Indent(depth), source, key.Name)
    {
        Depth = depth;
        SubKeyCount = key.SubKeyCount;
        ValueCount = key.ValueCount;
        View = key.View;
    }

    internal PSTreeRegistryKey(
        RegistryKey key, string name, string source) :
        base(name, source, key.Name)
    {
        SubKeyCount = key.SubKeyCount;
        ValueCount = key.ValueCount;
        View = key.View;
    }
}
