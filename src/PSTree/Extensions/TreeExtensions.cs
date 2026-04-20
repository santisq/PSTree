using System.Collections.Generic;
using System.Linq;
#if WINDOWS
using Microsoft.Win32;
#endif

namespace PSTree.Extensions;

internal static class TreeExtensions
{
    extension<TBase, TLeaf>(TLeaf leaf)
        where TLeaf : TBase
        where TBase : ITree
    {
        internal void AddTo(TreeBuilder<TBase, TLeaf> cache)
            => cache.Add(leaf);
    }

    extension<TContainer>(TContainer item)
    {
        internal void Push(Stack<TContainer> stack) => stack.Push(item);
    }

#if !NETCOREAPP
    extension<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
    {
        internal bool TryAdd(TKey key, TValue value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
                return true;
            }

            return false;
        }
    }
#endif

#if WINDOWS
    extension(RegistryKey key)
    {
        internal IEnumerable<string> EnumerateKeys() =>
#if NET8_0_OR_GREATER
            key.GetSubKeyNames().OrderDescending();
#else
            key.GetSubKeyNames().OrderByDescending(e => e);
#endif
    }

    extension(RegistryKey key)
    {
        internal (TreeRegistryKey, RegistryKey) CreateKey(string name)
            => (new TreeRegistryKey(key, name, key.Name), key);

        internal (TreeRegistryKey, RegistryKey) CreateTreeKey(string name, string source, int depth)
            => (new TreeRegistryKey(key, name, source, depth), key);
    }

    extension((TreeRegistryKey, RegistryKey) treeKey)
    {
        internal (TreeRegistryKey, RegistryKey) AddParent(TreeRegistryKey parent)
        {
            treeKey.Item1.AddParent<TreeRegistryKey>(parent);
            return treeKey;
        }
    }

    extension(string[] strings)
    {
        internal void Deconstruct(out string baseKey, out string? subKey)
        {
            baseKey = strings[0];
            subKey = strings.Length == 1 ? null : strings[1];
        }
    }
#endif
}
