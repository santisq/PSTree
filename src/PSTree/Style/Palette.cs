using System;

namespace PSTree.Style;

public sealed class Palette
{
    public sealed class ForegroundPalette
    {
        private string? _toString;

        public string Black { get; } = "\x1B[30m";

        public string BrightBlack { get; } = "\x1B[90m";

        public string White { get; } = "\x1B[37m";

        public string BrightWhite { get; } = "\x1B[97m";

        public string Red { get; } = "\x1B[31m";

        public string BrightRed { get; } = "\x1B[91m";

        public string Magenta { get; } = "\x1B[35m";

        public string BrightMagenta { get; } = "\x1B[95m";

        public string Blue { get; } = "\x1B[34m";

        public string BrightBlue { get; } = "\x1B[94m";

        public string Cyan { get; } = "\x1B[36m";

        public string BrightCyan { get; } = "\x1B[96m";

        public string Green { get; } = "\x1B[32m";

        public string BrightGreen { get; } = "\x1B[92m";

        public string Yellow { get; } = "\x1B[33m";

        public string BrightYellow { get; } = "\x1B[93m";

        public override string ToString() => _toString ??= TreeStyle.FormatType(this);
    }

    public sealed class BackgroundPalette
    {
        private string? _toString;

        public string Black { get; } = "\x1B[40m";

        public string BrightBlack { get; } = "\x1B[100m";

        public string White { get; } = "\x1B[47m";

        public string BrightWhite { get; } = "\x1B[107m";

        public string Red { get; } = "\x1B[41m";

        public string BrightRed { get; } = "\x1B[101m";

        public string Magenta { get; } = "\x1B[45m";

        public string BrightMagenta { get; } = "\x1B[105m";

        public string Blue { get; } = "\x1B[44m";

        public string BrightBlue { get; } = "\x1B[104m";

        public string Cyan { get; } = "\x1B[46m";

        public string BrightCyan { get; } = "\x1B[106m";

        public string Green { get; } = "\x1B[42m";

        public string BrightGreen { get; } = "\x1B[102m";

        public string Yellow { get; } = "\x1B[43m";

        public string BrightYellow { get; } = "\x1B[103m";

        public override string ToString() => _toString ??= TreeStyle.FormatType(this);
    }

    private string? _toString;

    public ForegroundPalette Foreground { get; } = new();

    public BackgroundPalette Background { get; } = new();

    public override string ToString() =>
        _toString ??= $"{Foreground}{Environment.NewLine}{Background}";
}
