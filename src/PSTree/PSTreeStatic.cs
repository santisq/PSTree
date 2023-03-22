using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PSTree;

internal static class PSTreeStatic
{
    internal static string[] _suffix;

    static PSTreeStatic() =>
        _suffix = new string[] { "Bytes", "Kb", "Mb", "Gb", "Tb", "Pb", "Eb", "Zb", "Yb" };

    internal static string Indent(string inputString, int indentation) =>
        new string(' ', (4 * indentation) - 4) + "└── " + inputString;

    internal static void DrawTree(List<PSTreeFileSystemInfo> inputObject)
    {
        Regex re = new(@"└|\S");

        for(int i = 0; i < inputObject.Count; i++)
        {
            int index = inputObject[i].Hierarchy.IndexOf('└');

            if(index >= 0)
            {
                int z = i - 1;
                while(!re.IsMatch(inputObject[z].Hierarchy[index].ToString()))
                {
                    char[] replace = inputObject[z].Hierarchy.ToCharArray();
                    replace[index] = '│';
                    inputObject[z].Hierarchy = new string(replace);
                    z--;
                }

                if(inputObject[z].Hierarchy[index] == '└')
                {
                    char[] replace = inputObject[z].Hierarchy.ToCharArray();
                    replace[index] = '├';
                    inputObject[z].Hierarchy = new string(replace);
                }
            }
        }
    }

    internal static string FormatLength(long length)
    {
        int index = 0;
        double len = length;

        while(len >= 1024) {
            len /= 1024;
            index++;
        }

        return string.Format("{0} {1}", Math.Round(len, 2), _suffix[index]);
    }
}