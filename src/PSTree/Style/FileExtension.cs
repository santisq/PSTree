using System.Collections.Generic;
using System.Text;

namespace PSTree.Style;

public sealed class FileExtension
{
    private const string _red = "\x1B[31;1m";

    private const string _yellow = "\x1B[33;1m";

    private readonly Dictionary<string, string> _extension;

    public string this[string extension]
    {
        get => _extension[extension];
        set => _extension[ValidateExtension(extension)] = TreeStyle.ThrowIfInvalidSequence(value);
    }

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

    // public ReadOnlyDictionary<string, string> Extension { get => new(_extension); }

    public override string ToString()
    {
        StringBuilder sb = new(_extension.Count);
        foreach (KeyValuePair<string, string> pair in _extension)
        {
            string value = $"{pair.Value}{pair.Key}{TreeStyle.Instance.Reset}";
            sb.AppendLine(value);
        }

        return sb.ToString();
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
}
