using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using PSTree.Nodes;

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

    internal bool IsExecutable(TreeFile file) => _exec.Contains(file.Extension);

    internal bool TryGetExtensionVt(TreeFile file, [NotNullWhen(true)] out string? vt)
        => Extension.TryGetValue(file.Extension, out vt);
}
