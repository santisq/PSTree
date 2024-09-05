using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace PSTree.Style;

public sealed class PSTreeStyle
{
    private static readonly PSTreeStyle s_instance = new();

    private const string _red = "\x1B[31;1m";

    private const string _yellow = "\x1B[33;1m";

    private static readonly StringComparer s_comparer = StringComparer.InvariantCultureIgnoreCase;

    private readonly Dictionary<string, string> _extension = new(s_comparer)
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

    public string Directory { get; set; } = "\x1B[44;1m";

    public string Reset { get; } = "\x1B[0m";

    public ReadOnlyDictionary<string, string> Extension { get => new(_extension); }

    public static PSTreeStyle Instance { get => s_instance; }

    internal string GetColoredName(FileInfo file)
    {
        if (_extension.TryGetValue(file.Extension, out string vt))
        {
            return $"{vt}{file.Name}{Reset}";
        }

        return file.Name;
    }

    internal string GetColoredName(DirectoryInfo directory) =>
        $"{Directory}{directory.Name}{Reset}";
}
