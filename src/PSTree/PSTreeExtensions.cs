using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Text.RegularExpressions;
using Microsoft.PowerShell.Commands;

namespace PSTree;

internal static class PSTreeStatic
{
    private static readonly List<string> s_normalizedPaths = new();

    private static readonly Regex s_re = new(@"└|\S", RegexOptions.Compiled);

    internal static string Indent(this string inputString, int indentation) =>
        new string(' ', (4 * indentation) - 4) + "└── " + inputString;

    internal static PSTreeFileSystemInfo[] ConvertToTree(
        this PSTreeFileSystemInfo[] inputObject)
    {
        // Well, I don't know what was I thinking when I wrote this, but it works :)

        for (int i = 0; i < inputObject.Length; i++)
        {
            int index = inputObject[i].Hierarchy.IndexOf('└');

            if (index < 0)
            {
                continue;
            }

            int z = i - 1;

            while (!s_re.IsMatch(inputObject[z].Hierarchy[index].ToString()))
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

        return inputObject;
    }

    internal static string[] NormalizePath(
        this string[] paths,
        bool isLiteral,
        PSCmdlet cmdlet)
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

                if (!provider.IsFileSystem())
                {
                    cmdlet.WriteError(ExceptionHelpers.InvalidProviderError(path, provider));
                    continue;
                }

                if (!resolvedPath.Exists())
                {
                    cmdlet.WriteError(ExceptionHelpers.InvalidPathError(resolvedPath));
                    continue;
                }

                s_normalizedPaths.Add(resolvedPath);
                continue;
            }

            try
            {
                resolvedPaths = cmdlet.GetResolvedProviderPathFromPSPath(path, out provider);

                foreach (string resolvedPath in resolvedPaths)
                {
                    if (!provider.IsFileSystem())
                    {
                        cmdlet.WriteError(ExceptionHelpers.InvalidProviderError(
                            resolvedPath, provider));
                        continue;
                    }

                    s_normalizedPaths.Add(resolvedPath);
                }
            }
            catch (Exception e)
            {
                cmdlet.WriteError(ExceptionHelpers.ResolvePathError(path, e));
            }
        }

        return s_normalizedPaths.ToArray();
    }

    internal static bool Exists(this string path) =>
        File.Exists(path) || Directory.Exists(path);

    internal static bool IsFileSystem(this ProviderInfo provider) =>
        provider.ImplementingType == typeof(FileSystemProvider);

    internal static bool IsArchive(this string path) =>
        !File.GetAttributes(path).HasFlag(FileAttributes.Directory);

    internal static bool IsDirectory(this string path) =>
        File.GetAttributes(path).HasFlag(FileAttributes.Directory);

    internal static bool IsHidden(this FileSystemInfo item) =>
        item.Attributes.HasFlag(FileAttributes.Hidden);
}
