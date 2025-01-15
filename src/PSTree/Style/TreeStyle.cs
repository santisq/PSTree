using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using PSTree.Extensions;

namespace PSTree.Style;

public sealed partial class TreeStyle
{
    private const string s_esc = "\x1B";

    private static TreeStyle s_instance = new();

    private string _directory = "\x1B[44;1m";

    private string _executable = "\x1B[32;1m";

    private readonly HashSet<string> _exec = new(Comparer)
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

    private readonly bool _isWin = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

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

    public string Executable
    {
        get => _executable;
        set => _executable = ThrowIfInvalidSequence(value);
    }

    public Extension Extension { get; } = new();

    public Palette Palette { get; } = new();

    public string Reset { get; } = "\x1B[0m";

    public static TreeStyle Instance { get => s_instance; }

    internal string GetColoredName(FileInfo file)
    {
        if (OutputRendering is not OutputRendering.Host)
        {
            return file.Name;
        }

        if (Extension.TryGetValue(file.Extension, out string vt))
        {
            return $"{vt}{file.Name}{Reset}";
        }

        if (_isWin && _exec.Contains(file.Extension))
        {
            return $"{Executable}{file.Name}{Reset}";
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
        StringBuilder builder = new(properties.Length);
        int i = 1;

        foreach (PropertyInfo property in properties)
        {
            string value = Instance.EscapeSequence(
                (string)property.GetValue(instance), 10);

            builder.Append(value);

            if (i++ % 4 == 0)
            {
                builder.AppendLine(Instance.Reset);
                continue;
            }

            builder.Append(Instance.Reset);
        }

        return builder.ToString();
    }

    public string CombineSequence(string left, string right)
    {
        ThrowIfInvalidSequence(left);
        ThrowIfInvalidSequence(right);
        return $"{left.TrimEnd('m')};{right.Substring(2)}";
    }

    public void ResetSettings() => s_instance = new();

    public string ToItalic(string vt)
    {
        ThrowIfInvalidSequence(vt);
        return $"{vt.TrimEnd('m')};3m";
    }

    public string ToBold(string vt)
    {
        ThrowIfInvalidSequence(vt);
        return $"{vt.TrimEnd('m')};1m";
    }

    public string EscapeSequence(string vt) =>
        $"{vt}{vt.Replace(s_esc, "`e")}{Reset}";

    private string EscapeSequence(string vt, int padding) =>
        $"{vt}{vt.Replace(s_esc, "`e").PadRight(padding)}{Reset}";
}
