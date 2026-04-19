#if WINDOWS
using System.Diagnostics.CodeAnalysis;

namespace PSTree.Style;

public sealed class RegistryStyle
{
    public string RegistryKey
    {
        get;
        set => field = TreeStyle.ThrowIfInvalidSequence(value);
    } = "\x1B[44;1m";

    public ValueKind RegistryValueKind { get; } = new();

    internal bool TryGetValueKindVt(TreeRegistryValue value, [NotNullWhen(true)] out string? vt)
        => RegistryValueKind.TryGetValue(value.Kind, out vt);
}
#endif
