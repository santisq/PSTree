using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PSTree.Style;

public sealed class FileExtension
{
    private const string _red = "\x1B[31;1m";
    private const string _yellow = "\x1B[33;1m";

    private readonly Dictionary<string, string> _extension;

    internal FileExtension()
    {
        _extension = new(TreeStyle.Comparer)
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
    }

    public ReadOnlyDictionary<string, string> Extension { get => new(_extension); }

    public bool TryGetValue(string extension, out string vt) => _extension.TryGetValue(extension, out vt);
}
