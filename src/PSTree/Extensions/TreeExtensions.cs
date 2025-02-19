using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace PSTree.Extensions;

internal static class TreeExtensions
{
    [ThreadStatic]
    private static StringBuilder? s_sb;

    internal static string Indent(this string inputString, int indentation)
    {
        s_sb ??= new StringBuilder();
        s_sb.Clear();

        return s_sb.Append(' ', (4 * indentation) - 4)
            .Append("└── ")
            .Append(inputString)
            .ToString();
    }

    internal static PSTreeFileSystemInfo[] Format(
        this PSTreeFileSystemInfo[] tree,
        Dictionary<string, int> itemCounts)
    {
        int index;
        for (int i = 0; i < tree.Length; i++)
        {
            PSTreeFileSystemInfo current = tree[i];

            if (current is PSTreeDirectory directory &&
                itemCounts.TryGetValue(directory.FullName, out int count))
            {
                directory.IndexCount(count);
            }

            if ((index = current.Hierarchy.IndexOf('└')) == -1)
            {
                continue;
            }

            for (int z = i - 1; z >= 0; z--)
            {
                current = tree[z];
                string hierarchy = current.Hierarchy;

                if (char.IsWhiteSpace(hierarchy[index]))
                {
                    current.Hierarchy = hierarchy.ReplaceAt(index, '│');
                    continue;
                }

                if (hierarchy[index] == '└')
                {
                    current.Hierarchy = hierarchy.ReplaceAt(index, '├');
                }

                break;
            }
        }

        return tree;
    }

    internal static PSTreeRegistryBase[] Format(
        this PSTreeRegistryBase[] tree)
    {
        int index;
        for (int i = 0; i < tree.Length; i++)
        {
            PSTreeRegistryBase current = tree[i];

            if ((index = current.Hierarchy.IndexOf('└')) == -1)
            {
                continue;
            }

            for (int z = i - 1; z >= 0; z--)
            {
                current = tree[z];
                string hierarchy = current.Hierarchy;

                if (char.IsWhiteSpace(hierarchy[index]))
                {
                    current.Hierarchy = hierarchy.ReplaceAt(index, '│');
                    continue;
                }

                if (hierarchy[index] == '└')
                {
                    current.Hierarchy = hierarchy.ReplaceAt(index, '├');
                }

                break;
            }
        }

        return tree;
    }

    private static string ReplaceAt(this string input, int index, char newChar)
    {
        char[] chars = input.ToCharArray();
        chars[index] = newChar;
        return new string(chars);
    }

    internal static (PSTreeRegistryKey, RegistryKey) CreateTreeKey(this RegistryKey key, string name) =>
        (new PSTreeRegistryKey(key, name), key);

    internal static (PSTreeRegistryKey, RegistryKey) CreateTreeKey(this RegistryKey key, string name, int depth) =>
        (new PSTreeRegistryKey(key, name, depth), key);

    internal static void Deconstruct(this string[] strings, out string @base, out string subKey) =>
        (@base, subKey) = (strings[0], strings[1]);
}
