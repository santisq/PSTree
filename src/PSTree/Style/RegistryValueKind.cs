#if WINDOWS
using Microsoft.Win32;

namespace PSTree.Style;

public sealed class ValueKind : StyleDictionaryBase<RegistryValueKind>
{
    internal ValueKind() : base([])
    { }

    protected override RegistryValueKind Validate(RegistryValueKind key) => key;
}
#endif
