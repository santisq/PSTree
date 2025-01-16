using System.IO;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;

namespace PSTree.Extensions;

internal static class PathExtensions
{
    internal static bool IsFileSystem(this ProviderInfo provider) =>
        provider.ImplementingType == typeof(FileSystemProvider);

    internal static bool Exists(this string path) =>
        File.Exists(path) || Directory.Exists(path);

    internal static bool IsHidden(this FileSystemInfo item) =>
        item.Attributes.HasFlag(FileAttributes.Hidden);
}
