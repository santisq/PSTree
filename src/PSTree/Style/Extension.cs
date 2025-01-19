using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using PSTree.Extensions;

namespace PSTree.Style;

public sealed class Extension
{
    private const string _red = "\x1B[31;1m";

    private const string _yellow = "\x1B[33;1m";

    private readonly Dictionary<string, string> _extension;

    public ICollection<string> Keys => _extension.Keys;

    public ICollection<string> Values => _extension.Values;

    public int Count => _extension.Count;

    public string this[string extension]
    {
        get => _extension[extension];
        set => _extension[ValidateExtension(extension)] = TreeStyle.ThrowIfInvalidSequence(value);
    }

    internal Extension()
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

    public override string ToString()
    {
        StringBuilder builder = new(_extension.Count);
        int len = _extension.Keys.Max(static e => e.Length);
        foreach (KeyValuePair<string, string> pair in _extension)
        {
            builder
                .Append(pair.Key.PadRight(len + 1))
                .Append(" → ")
                .AppendLine(TreeStyle.Instance.EscapeSequence(pair.Value));
        }

        return builder.ToString();
    }

    [Hidden, EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetEscapedValues(Extension extension)
    {
        StringBuilder builder = new(extension.Count);
        foreach (string value in extension.Values)
        {
            builder.AppendLine(TreeStyle.Instance.EscapeSequence(value));
        }
        return builder.ToString();
    }

    private string ValidateExtension(string extension)
    {
        if (!extension.StartsWith("."))
        {
            extension.ThrowInvalidExtension();
        }

        return extension;
    }

    public bool TryGetValue(string extension, out string vt) => _extension.TryGetValue(extension, out vt);

    public bool ContainsKey(string extension) => _extension.ContainsKey(extension);

    public void Add(string extension, string vt) =>
        _extension.Add(
            ValidateExtension(extension),
            TreeStyle.ThrowIfInvalidSequence(vt));

    public bool Remove(string extension) => _extension.Remove(ValidateExtension(extension));

    public void Clear() => _extension.Clear();
}
