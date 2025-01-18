using System;
using System.Text;

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

    internal static PSTreeFileSystemInfo[] ConvertToTree(
        this PSTreeFileSystemInfo[] inputObject)
    {
        int index;
        for (int i = 0; i < inputObject.Length; i++)
        {
            PSTreeFileSystemInfo current = inputObject[i];
            if ((index = current.Hierarchy.IndexOf('└')) == -1)
            {
                continue;
            }

            for (int z = i - 1; z >= 0; z--)
            {
                current = inputObject[z];
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

        return inputObject;
    }

    private static string ReplaceAt(this string input, int index, char newChar)
    {
        char[] chars = input.ToCharArray();
        chars[index] = newChar;
        return new string(chars);
    }
}
