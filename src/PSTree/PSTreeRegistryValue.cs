using Microsoft.Win32;
using PSTree.Extensions;

namespace PSTree;

public sealed class PSTreeRegistryValue : PSTreeRegistryBase
{
    public RegistryValueKind Kind { get; }

    public string Name { get; }

    internal PSTreeRegistryValue(
        RegistryKey key, string value, string source, int depth) :
        base(value.Indent(depth), source, Combine(key, value))
    {
        Kind = key.GetValueKind(value);
        Depth = depth;
        Name = value;
    }

    private static string Combine(RegistryKey key, string value) =>
        System.IO.Path.Combine(key.Name, value);

    public object? GetValue() =>
        Registry.GetValue(Path.Substring(0, Path.Length - Name.Length), Name, null);
}
