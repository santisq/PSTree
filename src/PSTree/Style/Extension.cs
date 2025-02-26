using System.Collections.Generic;
using PSTree.Extensions;

namespace PSTree.Style;

public sealed class Extension : StyleDictionaryBase<string>
{
    private const string _red = "\x1B[31;1m";

    private const string _yellow = "\x1B[33;1m";

    internal Extension() : base(GetExtensions())
    { }

    private static Dictionary<string, string> GetExtensions() =>
        new(TreeStyle.Comparer)
        {
            // Archive formats
            [".zip"] = _red,
            [".tgz"] = _red,
            [".gz"] = _red,
            [".tar"] = _red,
            [".nupkg"] = _red,
            [".cab"] = _red,
            [".7z"] = _red,

            // PowerShell files
            [".ps1"] = _yellow,
            [".psd1"] = _yellow,
            [".psm1"] = _yellow,
            [".ps1xml"] = _yellow
        };

    protected override string Validate(string key) => key.ThrowIfInvalidExtension();
}
