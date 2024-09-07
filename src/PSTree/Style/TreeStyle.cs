using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace PSTree.Style;

public sealed partial class TreeStyle
{
    private const string s_esc = "\x1B";

    private static readonly TreeStyle s_instance = new();

    private string _directory = "\x1B[44;1m";

    private static readonly Regex s_validate = new(
        @"^\x1B\[(?:[0-9]+;?){1,}m$",
        RegexOptions.Compiled);

    internal static StringComparer Comparer { get; } =
        StringComparer.InvariantCultureIgnoreCase;

    public OutputRendering OutputRendering { get; set; } = OutputRendering.Host;

    public string Directory
    {
        get => _directory;
        set => _directory = ThrowIfInvalidSequence(value);
    }

    public Extension Extension { get; } = new();

    public Palette Palette { get; } = new();

    public string Reset { get; } = "\x1B[0m";

    public static TreeStyle Instance { get => s_instance; }

    internal string GetColoredName(FileInfo file)
    {
        if (OutputRendering is OutputRendering.Host
            && Extension.TryGetValue(file.Extension, out string vt))
        {
            return $"{vt}{file.Name}{Reset}";
        }

        return file.Name;
    }

    internal string GetColoredName(DirectoryInfo directory)
    {
        if (OutputRendering is OutputRendering.PlainText)
        {
            return directory.Name;
        }

        return $"{Directory}{directory.Name}{Reset}";
    }

    internal static string ThrowIfInvalidSequence(string vt)
    {
        if (!s_validate.IsMatch(vt))
        {
            vt.ThrowInvalidSequence();
        }

        return vt;
    }

    internal static string FormatType(object instance)
    {
        PropertyInfo[] properties = instance.GetType().GetProperties();
        StringBuilder sb = new(properties.Length);
        int i = 1;

        foreach (PropertyInfo property in properties)
        {
            string value = Instance.EscapeSequence(
                (string)property.GetValue(instance), 10);

            if (i++ % 5 == 0)
            {
                sb.AppendLine(value);
                continue;
            }

            sb.Append(value);
        }

        return sb.ToString();
    }

    public string EscapeSequence(string vt) =>
        $"{vt}{vt.Replace(s_esc, "`e")}{Reset}";

    private string EscapeSequence(string vt, int padding) =>
        $"{vt}{vt.Replace(s_esc, "`e").PadRight(padding)}{Reset}";
}
