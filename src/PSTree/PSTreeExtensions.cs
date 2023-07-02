using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using Microsoft.PowerShell.Commands;

namespace PSTree;

internal static class PSTreeStatic
{
    private static readonly List<(string, ProviderInfo)> s_normalizedPaths = new();

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

            if (index < 0)
            {
                continue;
            }

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

        return inputObject.ToArray();
    }

    internal static (string, ProviderInfo)[] NormalizePath(
        this string[] paths, bool isLiteral, PSCmdlet cmdlet)
    {
        Collection<string> resolvedPaths;
        ProviderInfo provider;
        s_normalizedPaths.Clear();

        foreach (string path in paths)
        {
            if (isLiteral)
            {
                string resolvedPath = cmdlet.SessionState.Path.GetUnresolvedProviderPathFromPSPath(
                    path, out provider, out _);

                s_normalizedPaths.Add((resolvedPath, provider));
                continue;
            }

            try
            {
                resolvedPaths = cmdlet.GetResolvedProviderPathFromPSPath(path, out provider);

                foreach (string resolvedPath in resolvedPaths)
                {
                    s_normalizedPaths.Add((resolvedPath, provider));
                }
            }
            catch (Exception e)
            {
                cmdlet.WriteError(ExceptionHelpers.ResolvePathError(path, e));
            }
        }

        return s_normalizedPaths.ToArray();
    }

    internal static (string?, ProviderInfo?) NormalizePath(
        this string path, bool isLiteral, PSCmdlet cmdlet) =>
        NormalizePath(new string[1] { path }, isLiteral, cmdlet)
        .FirstOrDefault();

    internal static bool AssertFileSystem(this ProviderInfo provider) =>
        provider.ImplementingType == typeof(FileSystemProvider);

    internal static bool AssertArchive(this string path) =>
        !File.GetAttributes(path).HasFlag(FileAttributes.Directory);

    internal static bool AssertDirectory(this string path) =>
        File.GetAttributes(path).HasFlag(FileAttributes.Directory);
}
