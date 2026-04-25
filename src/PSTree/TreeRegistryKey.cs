#if WINDOWS
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Win32;
using PSTree.Native;
using IOPath = System.IO.Path;

namespace PSTree;

public sealed class TreeRegistryKey : TreeRegistryBase, IDisposable
{
    private readonly RegistryKey _key;

    public string Kind { get; } = "RegistryKey";

    public override string Name { get; }

    public int SubKeyCount { get; }

    public int ValueCount { get; }

    public RegistryView View { get; }

    public DateTime? LastWriteTime { get; }

    internal TreeRegistryKey(RegistryKey key)
        : this(key, IOPath.GetFileName(key.Name), key.Name)
    { }

    internal TreeRegistryKey(
        RegistryKey key,
        string name,
        string source,
        int depth = 0)
        : base(source, depth, key.Name)
    {
        _key = key;
        Name = name;
        SubKeyCount = key.SubKeyCount;
        ValueCount = key.ValueCount;
        View = key.View;
        LastWriteTime = key.GetLastWriteTime();
    }

    internal string[] GetValueNames() => _key.GetValueNames();

    internal IEnumerable<string> EnumerateKeys() =>
#if NET8_0_OR_GREATER
        _key.GetSubKeyNames().OrderDescending();
#else
        _key.GetSubKeyNames().OrderByDescending(e => e);
#endif

    internal bool TryCreateKey(
        string name,
        string source,
        [NotNullWhen(true)] out TreeRegistryKey? tree)
    {
        tree = null;
        if (_key.OpenSubKey(name) is not RegistryKey treeKey)
            return false;

        tree = new(treeKey, name, source, Depth + 1)
        {
            Container = this
        };

        AddChild(tree);
        return true;
    }

    internal TreeRegistryValue CreateValue(string value, string source)
    {
        TreeRegistryValue tvalue = new(this, value, source, Depth + 1);
        AddChild(tvalue);
        return tvalue;
    }

    internal RegistryValueKind GetValueKind(string value) => _key.GetValueKind(value);

    public void Dispose() => _key.Dispose();
}
#endif
