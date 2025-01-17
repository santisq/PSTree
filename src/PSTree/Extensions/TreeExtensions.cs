using System.Text;

namespace PSTree.Extensions;

internal static class TreeExtensions
{
    private static readonly StringBuilder s_sb = new();

    internal static string Indent(this string inputString, int indentation)
    {
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
        PSTreeFileSystemInfo current;

        for (int i = 0; i < inputObject.Length; i++)
        {
            current = inputObject[i];
            if ((index = current.Hierarchy.IndexOf('└')) == -1)
            {
                continue;
            }

            int z;
            char[] replace;
            for (z = i - 1; z >= 0; z--)
            {
                current = inputObject[z];
                if (!char.IsWhiteSpace(current.Hierarchy[index]))
                {
                    UpdateCorner(index, current);
                    break;
                }

                replace = current.Hierarchy.ToCharArray();
                replace[index] = '│';
                current.Hierarchy = new string(replace);
            }
        }

        return inputObject;
    }

    private static void UpdateCorner(int index, PSTreeFileSystemInfo current)
    {
        if (current.Hierarchy[index] == '└')
        {
            char[] replace = current.Hierarchy.ToCharArray();
            replace[index] = '├';
            current.Hierarchy = new string(replace);
        }
    }
}
