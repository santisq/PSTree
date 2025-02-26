using System.Collections.Generic;
using System.IO;

namespace PSTree.Style;

public sealed class FileSystemStyle
{
    private string _directory = "\x1B[44;1m";

    private string _executable = "\x1B[32;1m";

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
        get => _directory;
        set => _directory = TreeStyle.ThrowIfInvalidSequence(value);
    }

    public string Executable
    {
        get => _executable;
        set => _executable = TreeStyle.ThrowIfInvalidSequence(value);
    }

    public Extension Extension { get; }

    internal FileSystemStyle() => Extension = new();

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
