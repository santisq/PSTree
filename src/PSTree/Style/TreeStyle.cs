using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using PSTree.Extensions;

namespace PSTree.Style;

public sealed partial class TreeStyle
{
    private const string ESC = "\x1B";

    private static TreeStyle? s_instance;

#if NET8_0_OR_GREATER
    [GeneratedRegex(@"^\x1B\[(?:[0-9]+;?){1,}m$", RegexOptions.Compiled)]
    private static partial Regex ValidateRegex();

    private static readonly Regex s_validate = ValidateRegex();
#else
    private static readonly Regex s_validate = new(
        @"^\x1B\[(?:[0-9]+;?){1,}m$",
        RegexOptions.Compiled);
#endif
    internal bool ColoringDisabled { get; private set; }

    internal static StringComparer Comparer { get; } = StringComparer.InvariantCultureIgnoreCase;

    internal static bool IsWindows { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    internal RenderingSet RenderingSet { get; private set; } = RenderingSet.Fancy;

    public OutputRendering OutputRendering
    {
        get;
        set
        {
            ColoringDisabled = value == OutputRendering.PlainText;
            field = value;
        }
    } = OutputRendering.Host;

    public Palette Palette { get; } = new();

    public string Reset { get; } = $"{ESC}[0m";

    public RenderingStyle RenderingStyle
    {
        get;
        set
        {
            RenderingSet = value switch
            {
                RenderingStyle.Fancy => RenderingSet.Fancy,
                _ => throw new ArgumentOutOfRangeException(nameof(RenderingStyle))
            };

            field = value;
        }
    } = RenderingStyle.Fancy;

    public static TreeStyle Instance { get => s_instance ??= new(); }

    public FileSystemStyle FileSystem { get; } = new();

#if WINDOWS
    public RegistryStyle Registry { get; } = new();
#endif

    internal TreeStyle()
    { }

    internal static string ThrowIfInvalidSequence(string vt)
    {
        if (!s_validate.IsMatch(vt)) vt.ThrowInvalidSequence();
        return vt;
    }

    internal static string FormatType(object instance)
    {
        int i = 1;
        const string reset = $"{ESC}[0m";
        PropertyInfo[] properties = instance.GetType().GetProperties();
        StringBuilder builder = new(properties.Length);

        foreach (PropertyInfo property in properties)
        {
            string value = Instance.EscapeSequence(
                vt: (string)property.GetValue(instance)!,
                padding: 10);

            builder.Append(value);
            builder = i++ % 4 == 0
                ? builder.AppendLine(reset)
                : builder.Append(reset);
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
        $"{vt}{vt.Replace(ESC, "`e")}{Reset}";

    private string EscapeSequence(string vt, int padding) =>
        $"{vt}{vt.Replace(ESC, "`e").PadRight(padding)}{Reset}";
}
