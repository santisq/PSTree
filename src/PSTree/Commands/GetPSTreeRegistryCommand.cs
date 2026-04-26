using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;
using System.Security;
using Microsoft.Win32;
using PSTree.Comparers;
using PSTree.Extensions;
using PSTree.Interfaces;
using PSTree.Nodes;
using PSTree.Registry;
using IOPath = System.IO.Path;

namespace PSTree.Commands;

[Cmdlet(VerbsCommon.Get, "PSTreeRegistry", DefaultParameterSetName = PathSet)]
#if WINDOWS
[OutputType(typeof(TreeRegistryKey), typeof(TreeRegistryValue))]
#else
[ExcludeFromCodeCoverage]
#endif
[Alias("pstreereg")]
public sealed class GetPSTreeRegistryCommand
    : TreeCommandBase<TreeRegistryKey, TreeRegistryBase, RegistrySortMode>
{
    [Parameter]
    [Alias("k", "key")]
    public SwitchParameter KeysOnly { get; set; }

    protected override void BeginProcessing()
    {
        this.ThrowIfNotSupportedPlatform();
        base.BeginProcessing();
    }

#if WINDOWS
    protected override void ProcessRecord()
    {
        foreach ((ProviderInfo provider, string path) in EnumerateResolvedPaths())
        {
            if (provider.Name != "Registry")
            {
                WriteError(provider.ToInvalidProviderError(path, expected: "Registry"));
                continue;
            }

            if (!TryGetKey(path, out RegistryKey? key))
                continue;

            WriteObject(
                Build(new TreeRegistryKey(key)),
                enumerateCollection: true);
        }
    }

    protected override IEnumerable<ITree> Build(TreeRegistryKey key)
    {
        string source = key.Path!;
        int maxDp = 0;

        Push(key);
        while (ShouldContinue())
        {
            TreeRegistryKey current = Pop();

            int level = current.Depth + 1;
            maxDp = Math.Max(maxDp, level);

            using (current)
            {
                if (level > Depth) continue;
                if (KeysOnly) goto PushKeys;

                foreach (string value in current.GetValueNames())
                {
                    if (!ShouldSkipValue(value))
                        current.AddValue(value, source);
                }

            PushKeys:
                foreach (string name in current.EnumerateKeys())
                {
                    if (ShouldExclude(name)) continue;

                    try
                    {
                        if (current.TryAddSubKey(name, source, out TreeRegistryKey? subKey))
                            Push(subKey);
                    }
                    catch (SecurityException exception)
                    {
                        string path = IOPath.Combine(current.Name, name);
                        WriteError(exception.ToSecurityError(path));
                    }
                }

                // if (WithInclude && _builder.HasLeaf())
                //     current.PropagateInclude();
            }

            // Build:
            //     _builder.Add(current);
            //     _builder.Flush();
        }

        return key.Render(maxDp, Comparer);
        // return _builder.GetTree(WithInclude && !KeysOnly, maxDepth);
    }

    private bool ShouldSkipValue(string value) => ShouldExclude(value) || !ShouldInclude(value);

    private bool TryGetKey(string path, [NotNullWhen(true)] out RegistryKey? key)
    {
        string[] tokens = path.Split(['\\'], 2);
        key = default;

        if (!RegistryMappings.TryGetKey(tokens[0], out RegistryKey? value))
            return false;

        if (tokens.Length == 2 && !string.IsNullOrWhiteSpace(tokens[1]))
        {
            try
            {
                if ((key = value.OpenSubKey(tokens[1])) is null)
                {
                    WriteError(path.ToInvalidPathError());
                    return false;
                }
            }
            catch (SecurityException exception)
            {
                WriteError(exception.ToSecurityError(path));
                return false;
            }

            return true;
        }

        key = value;
        return true;
    }

    protected override IComparer<TreeRegistryBase> GetComparer() => SortBy switch
    {
        RegistrySortMode.ValuesFirst => TreeRegistryComparer.ByValue,
        RegistrySortMode.KeysFirst => TreeRegistryComparer.ByKey,
        _ => throw new ArgumentOutOfRangeException(nameof(SortBy))
    };
#endif
}
