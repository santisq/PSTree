using Microsoft.Win32;
using PSTree.Extensions;

namespace PSTree;

public sealed class PSTreeRegistryKey : PSTreeRegistryBase
{
    public string Kind { get; } = "RegistryKey";

    internal PSTreeRegistryKey(RegistryKey key, string name, int depth) :
        base(name.Indent(depth), key.Name)
    {
        Depth = depth;
    }

    internal PSTreeRegistryKey(RegistryKey key, string name) :
        base(name, key.Name)
    { }
}
