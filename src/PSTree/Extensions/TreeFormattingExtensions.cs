using System.Text;
using PSTree.Style;

namespace PSTree.Extensions;

internal static class TreeFormattingExtension
{
    private const string Reset = "\x1B[0m";

    private static TreeStyle Style { get => TreeStyle.Instance; }

    private static FileSystemStyle FileSystemStyle { get => TreeStyle.Instance.FileSystem; }

#if WINDOWS
    private static RegistryStyle RegistryStyle { get => TreeStyle.Instance.Registry; }
#endif

    extension(StringBuilder builder)
    {
        internal string GetStyledName(TreeFileSystemInfo fs)
            => fs is TreeDirectory dir
                ? builder.GetStyledName(dir)
                : builder.GetStyledName((TreeFile)fs);

        internal string GetStyledName(TreeDirectory directory)
        {
            if (Style.ColoringDisabled)
                return builder
                    .Append(directory.Name)
                    .ToString();

            return builder
                .Append(FileSystemStyle.Directory)
                .Append(directory.Name)
                .Append(Reset)
                .ToString();
        }

        internal string GetStyledName(TreeFile file)
        {
            if (Style.ColoringDisabled)
                return builder
                    .Append(file.Name)
                    .ToString();

            if (FileSystemStyle.TryGetExtensionVt(file, out string? extensionVt))
                return builder
                    .Append(extensionVt)
                    .Append(file.Name)
                    .Append(Reset)
                    .ToString();

            if (TreeStyle.IsWindows && FileSystemStyle.FileIsExecutable(file))
                return builder
                    .Append(FileSystemStyle.Executable)
                    .Append(file.Name)
                    .Append(Reset)
                    .ToString();

            return builder
                .Append(file.Name)
                .ToString();
        }

#if WINDOWS
        internal string GetStyledName(TreeRegistryBase reg)
            => reg is TreeRegistryKey key
                ? builder.GetStyledName(key)
                : builder.GetStyledName((TreeRegistryValue)reg);

        internal string GetStyledName(TreeRegistryKey key)
        {
            if (Style.ColoringDisabled)
                return builder
                    .Append(key.Name)
                    .ToString();

            return builder
                .Append(RegistryStyle.RegistryKey)
                .Append(key.Name)
                .Append(Reset)
                .ToString();
        }

        internal string GetStyledName(TreeRegistryValue value)
        {
            if (Style.ColoringDisabled)
                return builder
                    .Append(value.Name)
                    .ToString();

            if (RegistryStyle.TryGetValueKindVt(value, out string? valueKindVt))
                return builder
                    .Append(valueKindVt)
                    .Append(value.Name)
                    .ToString();

            return builder
                .Append(value.Name)
                .ToString();
        }
    }
#endif
}
