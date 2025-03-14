using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

        return s_sb
            .Append(' ', (4 * indentation) - 4)
            .Append("└── ")
            .Append(inputString)
            .ToString();
    }

    internal static TreeFileSystemInfo[] Format(
        this TreeFileSystemInfo[] tree,
        Dictionary<string, int> itemCounts)
    {
        int index;
        for (int i = 0; i < tree.Length; i++)
        {
            TreeFileSystemInfo current = tree[i];

            if (current is TreeDirectory directory &&
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

#if NETCOREAPP
    [SkipLocalsInit]
#endif
    private static unsafe string ReplaceAt(this string input, int index, char newChar)
    {
#if NETCOREAPP
        return string.Create(input.Length, (input, index, newChar),
            static (buffer, state) =>
            {
                state.input.AsSpan().CopyTo(buffer);
                buffer[state.index] = state.newChar;
            });
#else
        if (input.Length > 0x200)
        {
            char[] chars = input.ToCharArray();
            chars[index] = newChar;
            return new string(chars);
        }

        char* pChars = stackalloc char[0x200];
        fixed (char* source = input)
        {
            Buffer.MemoryCopy(
                source,
                pChars,
                0x200 * sizeof(char),
                input.Length * sizeof(char));
        }

        pChars[index] = newChar;
        return new string(pChars, 0, input.Length);
#endif
    }

#if !NETCOREAPP
    internal static bool TryAdd<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        TValue value)
    {
        if (!dictionary.ContainsKey(key))
        {
            dictionary.Add(key, value);
            return true;
        }

        return false;
    }
#endif

#if WINDOWS
    internal static TreeRegistryBase[] Format(
        this TreeRegistryBase[] tree)
    {
        int index;
        for (int i = 0; i < tree.Length; i++)
        {
            TreeRegistryBase current = tree[i];

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

    internal static (TreeRegistryKey, RegistryKey) CreateTreeKey(
        this RegistryKey key, string name) =>
        (new TreeRegistryKey(key, name, key.Name), key);

    internal static (TreeRegistryKey, RegistryKey) CreateTreeKey(
        this RegistryKey key, string name, string source, int depth) =>
        (new TreeRegistryKey(key, name, source, depth), key);

    internal static void Deconstruct(
        this string[] strings,
        out string @base,
        out string? subKey)
    {
        (@base, subKey) = (strings[0], strings.Length == 1 ? null : strings[1]);
    }
#endif
}
