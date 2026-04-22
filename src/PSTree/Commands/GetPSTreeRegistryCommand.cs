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
public sealed class GetPSTreeRegistryCommand : TreeCommandBase
{
#if WINDOWS
    private readonly TreeBuilder<TreeRegistryBase, TreeRegistryValue> _builder = new();

    private readonly Stack<(TreeRegistryKey, RegistryKey)> _stack = [];
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

            WriteObject(Traverse(key), enumerateCollection: true);
        }
    }

    private ITree[] Traverse(RegistryKey regkey)
    {
        _builder.Clear();

        string source = regkey.Name;
        int maxDepth = 0;

        regkey.
            AsTreeKey(IOPath.GetFileName(regkey.Name)).
            Push(_stack);

        while (!Canceled && _stack.Count > 0)
        {
            (TreeRegistryKey tree, regkey) = _stack.Pop();

            int level = tree.Depth + 1;
            maxDepth = Math.Max(maxDepth, level);

            using (regkey)
            {
                if (level > Depth) goto Build;
                if (KeysOnly) goto PushKeys;

                foreach (string value in regkey.GetValueNames())
                {
                    if (ShouldSkipValue(value)) continue;

                    new TreeRegistryValue(regkey, value, source, level)
                        .AddParent<TreeRegistryValue>(tree)
                        .AddTo(_builder);
                }

            PushKeys:
                foreach (string keyname in regkey.EnumerateKeys())
                {
                    if (ShouldExclude(keyname)) continue;

                    try
                    {
                        RegistryKey? subkey = regkey.OpenSubKey(keyname);
                        if (subkey is null) continue;

                        subkey
                            .AsTreeKey(keyname, source, level)
                            .AddParent(tree)
                            .Push(_stack);
                    }
                    catch (SecurityException exception)
                    {
                        string path = IOPath.Combine(regkey.Name, keyname);
                        WriteError(exception.ToSecurityError(path));
                    }
                }

                if (WithInclude && _builder.HasLeaf())
                    tree.PropagateInclude();
            }

        Build:
            _builder.Add(tree);
            _builder.Flush();
        }

        return _builder.GetTree(WithInclude && !KeysOnly, maxDepth);
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
