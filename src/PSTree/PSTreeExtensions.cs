using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PSTree;

internal static class PSTreeStatic
{
    internal static string Indent(this string inputString, int indentation) =>
        new string(' ', (4 * indentation) - 4) + "└── " + inputString;

    internal static PSTreeFileSystemInfo[] ConvertToTree(
        this List<PSTreeFileSystemInfo> inputObject)
    {
        // Well, I don't know what was I thinking when I wrote this, but it works :)

        Regex re = new(@"└|\S", RegexOptions.Compiled);

        for (int i = 0; i < inputObject.Count; i++)
        {
            int index = inputObject[i].Hierarchy.IndexOf('└');

            if (index >= 0)
            {
                int z = i - 1;
                while (!re.IsMatch(inputObject[z].Hierarchy[index].ToString()))
                {
                    char[] replace = inputObject[z].Hierarchy.ToCharArray();
                    replace[index] = '│';
                    inputObject[z].Hierarchy = new string(replace);
                    z--;
                }

                if (inputObject[z].Hierarchy[index] == '└')
                {
                    char[] replace = inputObject[z].Hierarchy.ToCharArray();
                    replace[index] = '├';
                    inputObject[z].Hierarchy = new string(replace);
                }
            }
        }

        return inputObject.ToArray();
    }
}
