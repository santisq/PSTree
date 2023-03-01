using System;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace PSTree;

internal static class PSTreeStatic
{
    internal static string Indent(string inputString, int indentation)
    {
        return string.Concat(Enumerable.Repeat("    ", indentation)) + inputString;
    }
}

public sealed class PSTreeFile
{
    internal readonly FileInfo _instance;

    internal readonly int _depth;

    public string Hierarchy { get; set; }

    public long Length { get; set; }

    public string FullName { get; set; }

    PSTreeFile(FileInfo fileInfo, int depth)
    {
        _instance = fileInfo;
        _depth    = depth;
        Hierarchy = PSTreeStatic.Indent(fileInfo.Name, depth);
        Length    = fileInfo.Length;
        FullName  = fileInfo.FullName;
    }

    public bool HasFlag(FileAttributes flag)
    {
        return _instance.Attributes.HasFlag(flag);
    }
}
