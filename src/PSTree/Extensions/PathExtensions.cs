using System.IO;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;

namespace PSTree.Extensions;

internal static class PathExtensions
{
    internal static bool ValidateProvider(this ProviderInfo provider)
    {
        if (provider.ImplementingType == typeof(FileSystemProvider))
        {
            return true;
        }

        return false;
    }

    internal static string Normalize(this string path)
    {
        if (path.ValidateExists())
        {
            return path;
        }

        path = path.TrimEnd(['\\', '/']);

        if (string.IsNullOrWhiteSpace(path))
        {
            return Path.DirectorySeparatorChar.ToString();
        }

        if (Path.GetPathRoot(path) == path)
        {
            return string.Concat(path, Path.DirectorySeparatorChar);
        }

        return path;
    }

    internal static bool ValidateExists(this string path)
    {
        if (File.Exists(path) || Directory.Exists(path))
        {
            return true;
        }

        return false;
    }

    internal static bool IsHidden(this FileSystemInfo item) =>
        item.Attributes.HasFlag(FileAttributes.Hidden);

    private static bool MatchAny(
        FileSystemInfo item,
        WildcardPattern[] patterns)
    {
        foreach (WildcardPattern pattern in patterns)
        {
            if (pattern.IsMatch(item.FullName))
            {
                return true;
            }
        }

        return false;
    }

    internal static bool ShouldInclude(
        this FileInfo file,
        WildcardPattern[]? patterns)
    {
        if (patterns is null)
        {
            return true;
        }

        return MatchAny(file, patterns);
    }

    internal static bool ShouldExclude(
        this FileSystemInfo item,
        WildcardPattern[]? patterns)
    {
        if (patterns is null)
        {
            return false;
        }

        return MatchAny(item, patterns);
    }
}
