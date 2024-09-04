using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace PSTree.Style;

public static class PSTreeStyle
{
    private const string _red = "\x1B[31;1m";

    private const string _yellow = "\x1B[33;1m";

    private static readonly StringComparer s_comparer = StringComparer.InvariantCultureIgnoreCase;

    private static readonly Dictionary<string, string> s_extension;

    public static string Directory { get; set; } = "\x1B[44;1m";

    public const string Reset = "\x1B[0m";

    public static ReadOnlyDictionary<string, string> Extension { get => new(s_extension); }

    static PSTreeStyle() => s_extension = new(s_comparer)
    {
        [".zip"] = _red,
        [".tgz"] = _red,
        [".gz"] = _red,
        [".tar"] = _red,
        [".nupkg"] = _red,
        [".cab"] = _red,
        [".7z"] = _red,
        [".ps1"] = _yellow,
        [".psd1"] = _yellow,
        [".psm1"] = _yellow,
        [".ps1xml"] = _yellow
    };

    internal static string GetColoredName(this FileInfo file)
    {
        if (s_extension.TryGetValue(file.Extension, out string vt))
        {
            return $"{vt}{file.Name}{Reset}";
        }

        return file.Name;
    }

    internal static string GetColoredName(this DirectoryInfo directory) =>
        $"{Directory}{directory.Name}{Reset}";
}
