using Microsoft.Win32;
using PSTree.Extensions;

namespace PSTree;

public sealed class PSTreeRegistryValue : PSTreeRegistryBase
{
    public RegistryValueKind Kind { get; }

    internal PSTreeRegistryValue(RegistryKey key, string value, int depth) :
        base(value.Indent(depth), Combine(key, value))
    {
        Kind = key.GetValueKind(value);
        Depth = depth;
    }
}
