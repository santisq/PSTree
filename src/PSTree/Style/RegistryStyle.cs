#if WINDOWS
using Microsoft.Win32;

namespace PSTree.Style;

public sealed class RegistryStyle
{
    private string _registryKey = "\x1B[44;1m";

    public string RegistryKey
    {
        get => _registryKey;
        set => _registryKey = TreeStyle.ThrowIfInvalidSequence(value);
    }

    public ValueKind RegistryValueKind { get; }

    internal RegistryStyle() => RegistryValueKind = new();

    internal string GetColoredKey(string key)
    {
        if (TreeStyle.Instance.OutputRendering is not OutputRendering.Host)
        {
            return key;
        }

        return $"{RegistryKey}{key}{TreeStyle.Instance.Reset}";
    }

    internal string GetColoredValue(string value, RegistryValueKind kind)
    {
        if (TreeStyle.Instance.OutputRendering is not OutputRendering.Host)
        {
            return value;
        }

        if (RegistryValueKind.TryGetValue(kind, out string? vt))
        {
            return $"{vt}{value}{TreeStyle.Instance.Reset}";
        }

        return value;
    }
}
#endif
