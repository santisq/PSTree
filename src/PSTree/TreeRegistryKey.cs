using Microsoft.Win32;
using PSTree.Extensions;

namespace PSTree;

#if !WINDOWS
    [ExcludeFromCodeCoverage]
#endif
public sealed class TreeRegistryKey : TreeRegistryBase
{
    public string Kind { get; } = "RegistryKey";

    public int SubKeyCount { get; }

    public int ValueCount { get; }

    public RegistryView View { get; }

    internal TreeRegistryKey(
        RegistryKey key, string name, string source, int depth) :
        base(name.Indent(depth), source, key.Name)
    {
        Depth = depth;
        SubKeyCount = key.SubKeyCount;
        ValueCount = key.ValueCount;
        View = key.View;
    }

    internal TreeRegistryKey(
        RegistryKey key, string name, string source) :
        base(name, source, key.Name)
    {
        SubKeyCount = key.SubKeyCount;
        ValueCount = key.ValueCount;
        View = key.View;
    }
}
