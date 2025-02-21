using Microsoft.Win32;
using PSTree.Extensions;

namespace PSTree;

#if !WINDOWS
    [ExcludeFromCodeCoverage]
#endif
public sealed class TreeRegistryValue : TreeRegistryBase
{
    private readonly string _parentPath;

    public RegistryValueKind Kind { get; }

    public string Name { get; }

    public override string? PSParentPath { get; }

    internal TreeRegistryValue(
        RegistryKey key, string value, string source, int depth) :
        base(value.Indent(depth), source)
    {
        _parentPath = key.Name;
        Kind = key.GetValueKind(value);
        Depth = depth;
        Name = value;
        PSParentPath = $"{_providerPath}{_parentPath}";
    }

    public object? GetValue() => Registry.GetValue(_parentPath, Name, null);
}
