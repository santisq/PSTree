using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;

namespace PSTree;

internal static class PathExtensions
{
    [ThreadStatic]
    private static List<string>? s_normalizedPaths;

    private static readonly char[] _dirSeparator = @"\/".ToCharArray();

    internal static string[] NormalizePath(
        this string[] paths,
        bool isLiteral,
        PSCmdlet cmdlet)
    {
        s_normalizedPaths ??= [];
        s_normalizedPaths.Clear();

        Collection<string> resolvedPaths;
        ProviderInfo provider;

        foreach (string path in paths)
        {
            if (isLiteral)
            {
                string resolvedPath = cmdlet.SessionState.Path.GetUnresolvedProviderPathFromPSPath(
                    path, out provider, out _);

                if (!provider.IsFileSystem())
                {
                    cmdlet.WriteError(provider.ToInvalidProviderError(path));
                    continue;
                }

                if (!resolvedPath.Exists())
                {
                    cmdlet.WriteError(resolvedPath.ToInvalidPathError());
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
                        cmdlet.WriteError(provider.ToInvalidProviderError(resolvedPath));
                        continue;
                    }

                    s_normalizedPaths.Add(resolvedPath);
                }
            }
            catch (Exception exception)
            {
                cmdlet.WriteError(exception.ToResolvePathError(path));
            }
        }

        return [.. s_normalizedPaths];
    }

    internal static string NormalizePath(this string path, bool isLiteral, PSCmdlet cmdlet) =>
        NormalizePath([path], isLiteral, cmdlet)
            .FirstOrDefault();

    internal static bool IsFileSystem(this ProviderInfo provider) =>
        provider.ImplementingType == typeof(FileSystemProvider);

    internal static bool IsArchive(this string path) =>
        !File.GetAttributes(path).HasFlag(FileAttributes.Directory);

    internal static bool Exists(this string path) =>
        File.Exists(path) || Directory.Exists(path);

    internal static bool IsHidden(this FileSystemInfo item) =>
        item.Attributes.HasFlag(FileAttributes.Hidden);

    internal static string TrimExcess(this string path)
    {
        path = path.TrimEnd(_dirSeparator);

        if (string.IsNullOrWhiteSpace(path))
        {
            return Path.DirectorySeparatorChar.ToString();
        }

        if (Path.GetPathRoot(path) == path)
        {
            return string.Concat(path.ToUpper(), Path.DirectorySeparatorChar);
        }

        return path;
    }
}
