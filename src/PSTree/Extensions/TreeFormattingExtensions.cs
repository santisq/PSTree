using System;
using System.Text;
using PSTree.Interfaces;
using PSTree.Nodes;
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
        internal void SetStyledName(ITree tree)
            => tree.Hierarchy = tree switch
            {
                TreeDirectory dir => builder.GetStyledName(dir),
                TreeFile file => builder.GetStyledName(file),
#if WINDOWS
                TreeRegistryKey key => builder.GetStyledName(key),
                TreeRegistryValue value => builder.GetStyledName(value),
#endif
                _ => throw new ArgumentOutOfRangeException(nameof(tree))
            };

        private string GetStyledName(TreeDirectory directory)
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

        private string GetStyledName(TreeFile file)
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

            if (TreeStyle.IsWindows && FileSystemStyle.IsExecutable(file))
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
        private string GetStyledName(TreeRegistryKey key)
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

        private string GetStyledName(TreeRegistryValue value)
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
#endif
    }
}
