using System.Collections.Generic;
using System.Linq;
using System.Text;
#if WINDOWS
using Microsoft.Win32;
#endif

namespace PSTree.Extensions;

internal static class TreeExtensions
{
    private const string Vertical = "│   ";
    private const string Space = "    ";
    private const string Branch = "├── ";
    private const string LastBranch = "└── ";

    extension(TreeFileSystemInfo[] tree)
    {
        internal TreeFileSystemInfo[] Format(Dictionary<string, int> itemCounts)
        {
            StringBuilder sb = new(256);
            bool[] isLastChild = PrecomputeLastChild(tree);
            List<bool> continues = new(32);

            for (int i = 0; i < tree.Length; i++)
            {
                sb.Clear();

                TreeFileSystemInfo current = tree[i];
                int depth = current.Depth;

                if (current is TreeDirectory directory &&
                    itemCounts.TryGetValue(directory.FullName, out int count))
                {
                    directory.IndexCount(count);
                }

                if (depth == 0)
                {
                    current.Hierarchy = sb.GetStyledName(current);
                    continue;
                }

                AppendAncestorPrefix(sb, continues, depth);
                bool isLast = isLastChild[i];

                current.Hierarchy = sb
                    .Append(isLast ? LastBranch : Branch)
                    .GetStyledName(current);

                continues[depth] = !isLast;
                TrimDeeperContinuations(continues, depth);
            }

            return tree;
        }
    }

    private static bool[] PrecomputeLastChild(ITree[] tree)
    {
        bool[] isLastChild = new bool[tree.Length];

        for (int i = 0; i < tree.Length; i++)
        {
            int d = tree[i].Depth;
            int j = i + 1;

            while (j < tree.Length && tree[j].Depth > d) j++;

            isLastChild[i] = j == tree.Length || tree[j].Depth != d;
        }

        return isLastChild;
    }

    private static void AppendAncestorPrefix(
        StringBuilder sb, List<bool> continues, int depth)
    {
        while (continues.Count <= depth)
            continues.Add(false);

        for (int lev = 1; lev < depth; lev++)
            sb.Append(continues[lev] ? Vertical : Space);
    }

    private static void TrimDeeperContinuations(List<bool> continues, int depth)
    {
        if (continues.Count > depth + 1)
            continues.RemoveRange(depth + 1, continues.Count - depth - 1);
    }

    extension<TBase, TLeaf>(TLeaf leaf)
        where TLeaf : TBase
        where TBase : ITree
    {
        internal void AddToCache(TreeBuilder<TBase, TLeaf> cache)
            => cache.Add(leaf);
    }

    extension<T>(T container)
    {
        internal void PushToStack(Stack<T> stack) => stack.Push(container);
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
    extension(TreeRegistryBase[] tree)
    {
        internal TreeRegistryBase[] Format()
        {
            StringBuilder sb = new(256);
            bool[] isLastChild = PrecomputeLastChild(tree);
            List<bool> continues = new(32);

            for (int i = 0; i < tree.Length; i++)
            {
                sb.Clear();

                TreeRegistryBase current = tree[i];
                int depth = current.Depth;

                if (depth == 0)
                {
                    current.Hierarchy = sb.GetStyledName(current);
                    continue;
                }

                AppendAncestorPrefix(sb, continues, depth);

                bool isLast = isLastChild[i];

                current.Hierarchy = sb
                    .Append(isLast ? LastBranch : Branch)
                    .GetStyledName(current);

                continues[depth] = !isLast;

                TrimDeeperContinuations(continues, depth);
            }

            return tree;
        }
    }

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
        internal (TreeRegistryKey, RegistryKey) CreateTreeKey(string name)
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
