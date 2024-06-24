using System;
using System.IO;

namespace ProjectBuilder;

internal static class Extensions
{
    internal static void CopyRecursive(this DirectoryInfo source, string? destination)
    {
        if (destination is null)
        {
            throw new ArgumentNullException($"Destination path is null.", nameof(destination));
        }

        if (!Directory.Exists(destination))
        {
            Directory.CreateDirectory(destination);
        }

        foreach (DirectoryInfo dir in source.EnumerateDirectories("*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dir.FullName.Replace(source.FullName, destination));
        }

        foreach (FileInfo file in source.EnumerateFiles("*", SearchOption.AllDirectories))
        {
            file.CopyTo(file.FullName.Replace(source.FullName, destination));
        }
    }
}
