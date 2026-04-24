using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;
using System.Security;
using Microsoft.Win32;
using PSTree.Extensions;
using IOPath = System.IO.Path;

namespace PSTree.Commands;

[Cmdlet(VerbsCommon.Get, "PSTreeRegistry", DefaultParameterSetName = PathSet)]
#if WINDOWS
[OutputType(typeof(TreeRegistryKey), typeof(TreeRegistryValue))]
#else
[ExcludeFromCodeCoverage]
#endif
[Alias("pstreereg")]
public sealed class GetPSTreeRegistryCommand : TreeCommandBase<TreeRegistryKey>
{
#if WINDOWS
    private readonly TreeBuilder<TreeRegistryBase, TreeRegistryValue> _builder = new();
#endif

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
                Traverse(new TreeRegistryKey(key)),
                enumerateCollection: true);
        }
    }

    protected override ITree Traverse(TreeRegistryKey key)
    {
        _builder.Clear();
        Push(key);

        string source = key.Path!;
        int maxDepth = 0;

        while (ShouldContinue())
        {
            TreeRegistryKey next = Pop();

            int level = next.Depth + 1;
            maxDepth = Math.Max(maxDepth, level);

            using (next)
            {
                if (level > Depth) goto Build;
                if (KeysOnly) goto PushKeys;

                foreach (string value in next.GetValueNames())
                {
                    if (ShouldSkipValue(value)) continue;

                    next.CreateValue(value, source)
                        .AddTo(_builder);
                }

            PushKeys:
                foreach (string name in next.EnumerateKeys())
                {
                    if (ShouldExclude(name)) continue;

                    try
                    {
                        if (next.TryCreateKey(name, source, out TreeRegistryKey? subKey))
                            Push(subKey);
                    }
                    catch (SecurityException exception)
                    {
                        string path = IOPath.Combine(key.Name, name);
                        WriteError(exception.ToSecurityError(path));
                    }
                }

                if (WithInclude && _builder.HasLeaf())
                    next.PropagateInclude();
            }

        Build:
            _builder.Add(next);
            _builder.Flush();
        }

        return _builder.Items[0];
        // return _builder.GetTree(WithInclude && !KeysOnly, maxDepth);
    }

    private bool ShouldSkipValue(string value) => ShouldExclude(value) || !ShouldInclude(value);

    private bool TryGetKey(string path, [NotNullWhen(true)] out RegistryKey? key)
    {
        (string @base, string? subkey) = path.Split(['\\'], 2);
        key = default;

        if (!RegistryMappings.TryGetKey(@base, out RegistryKey? value))
            return false;

        if (!string.IsNullOrWhiteSpace(subkey))
        {
            try
            {
                if ((key = value.OpenSubKey(subkey)) is null)
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
#endif
}
