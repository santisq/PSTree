using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace PSTree.Style;

public sealed class FileSystemStyle
{
    private readonly HashSet<string> _exec = new(TreeStyle.Comparer)
    {
        ".com",
        ".exe",
        ".bat",
        ".cmd",
        ".vbs",
        ".vbe",
        ".js",
        ".jse",
        ".wsf",
        ".wsh",
        ".msc",
        ".cpl"
    };

    public string Directory
    {
        get;
        set => field = TreeStyle.ThrowIfInvalidSequence(value);
    } = "\x1B[44;1m";

    public string Executable
    {
        get;
        set => field = TreeStyle.ThrowIfInvalidSequence(value);
    } = "\x1B[32;1m";

    public Extension Extension { get; } = new();

    internal bool FileIsExecutable(TreeFile file) => _exec.Contains(file.Extension);

    internal bool TryGetExtensionVt(TreeFile file, [NotNullWhen(true)] out string? vt)
        => Extension.TryGetValue(file.Extension, out vt);

    internal string GetColoredName(FileInfo file)
    {
        if (TreeStyle.Instance.OutputRendering is not OutputRendering.Host)
        {
            return file.Name;
        }

        if (Extension.TryGetValue(file.Extension, out string? vt))
        {
            return $"{vt}{file.Name}{TreeStyle.Instance.Reset}";
        }

        if (TreeStyle.IsWindows && _exec.Contains(file.Extension))
        {
            return $"{Executable}{file.Name}{TreeStyle.Instance.Reset}";
        }

        return file.Name;
    }

    internal string GetColoredName(DirectoryInfo directory)
    {
        if (TreeStyle.Instance.OutputRendering is OutputRendering.PlainText)
        {
            return directory.Name;
        }

        return $"{Directory}{directory.Name}{TreeStyle.Instance.Reset}";
    }
}
