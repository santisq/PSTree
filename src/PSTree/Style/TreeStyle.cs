using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using PSTree.Extensions;

namespace PSTree.Style;

public sealed class TreeStyle
{
    private const string s_esc = "\x1B";

    private static TreeStyle? s_instance;

    private static readonly Regex s_validate = new(
        @"^\x1B\[(?:[0-9]+;?){1,}m$",
        RegexOptions.Compiled);

    internal static StringComparer Comparer { get; } = StringComparer.InvariantCultureIgnoreCase;

    internal static bool IsWindows { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    public OutputRendering OutputRendering { get; set; } = OutputRendering.Host;

    public Palette Palette { get; } = new();

    public string Reset { get; } = "\x1B[0m";

    public static TreeStyle Instance { get => s_instance ??= new(); }

    public FileSystemStyle FileSystem { get; } = new();

#if WINDOWS
    public RegistryStyle Registry { get; } = new();
#endif

    internal TreeStyle()
    { }

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
                vt: (string)property.GetValue(instance)!,
                padding: 10);

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
