using System.IO;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;

namespace PSTree.Extensions;

internal static class PathExtensions
{
    private static readonly char[] _dirSeparator = @"\/".ToCharArray();

    internal static bool Validate(
        this ProviderInfo provider,
        string path,
        PSCmdlet cmdlet)
    {
        if (provider.ImplementingType == typeof(FileSystemProvider))
        {
            return true;
        }

        ErrorRecord error = provider.ToInvalidProviderError(path);
        cmdlet.WriteError(error);
        return false;
    }

    internal static bool Exists(this string path) =>
        File.Exists(path) || Directory.Exists(path);

    internal static bool IsHidden(this FileSystemInfo item) =>
        item.Attributes.HasFlag(FileAttributes.Hidden);

    internal static string TrimExcess(this string path)
    {
        path = path.TrimEnd(_dirSeparator);

        if (string.IsNullOrWhiteSpace(path))
        {
            return System.IO.Path.DirectorySeparatorChar.ToString();
        }

        if (System.IO.Path.GetPathRoot(path) == path)
        {
            return string.Concat(path.ToUpper(), System.IO.Path.DirectorySeparatorChar);
        }

        return path;
    }

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
