#if WINDOWS
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Win32;
using PSTree.Registry;
using IOPath = System.IO.Path;

namespace PSTree.Nodes;

public sealed class TreeRegistryKey : TreeRegistryBase, IDisposable
{
    private readonly RegistryKey _key;

    internal override bool IsContainer { get; } = true;

    public string Kind { get; } = "RegistryKey";

    public override string Name { get; }

    public int SubKeyCount { get; }

    public int ValueCount { get; }

    public RegistryView View { get; }

    public DateTime? LastWriteTime { get; }

    internal TreeRegistryKey(RegistryKey key)
        : this(key, IOPath.GetFileName(key.Name), key.Name)
    { }

    private TreeRegistryKey(
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

    internal bool TryAddSubKey(
        string name,
        string source,
        [NotNullWhen(true)] out TreeRegistryKey? treeKey)
    {
        treeKey = null;
        if (_key.OpenSubKey(name) is not RegistryKey key)
            return false;

        treeKey = new(key, name, source, Depth + 1)
        {
            Container = this
        };

        AddChild(treeKey);
        return true;
    }

    internal void AddValue(string value, string source)
        => AddChild(new TreeRegistryValue(this, value, source, Depth + 1));

    internal RegistryValueKind GetValueKind(string value) => _key.GetValueKind(value);

    public void Dispose() => _key.Dispose();
}
#endif
