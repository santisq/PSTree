using System.Collections.Generic;
using PSTree.Extensions;

namespace PSTree.Style;

public sealed class Extension : StyleDictionaryBase<string>
{
    private const string Red = "\x1B[31;1m";

    private const string Yellow = "\x1B[33;1m";

    internal Extension() : base(GetExtensions())
    { }

    private static Dictionary<string, string> GetExtensions() =>
        new(TreeStyle.Comparer)
        {
            // Archive formats
            [".zip"] = Red,
            [".tgz"] = Red,
            [".gz"] = Red,
            [".tar"] = Red,
            [".nupkg"] = Red,
            [".cab"] = Red,
            [".7z"] = Red,

            // PowerShell files
            [".ps1"] = Yellow,
            [".psd1"] = Yellow,
            [".psm1"] = Yellow,
            [".ps1xml"] = Yellow
        };

    protected override string Validate(string key) => key.ThrowIfInvalidExtension();
}
