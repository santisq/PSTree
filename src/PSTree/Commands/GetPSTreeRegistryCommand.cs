using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;
using System.Security;
using Microsoft.Win32;
using PSTree.Extensions;

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
            {
                continue;
            }

            WriteObject(Traverse(key), enumerateCollection: true);
        }
    }

    private TreeRegistryBase[] Traverse(RegistryKey registryKey)
    {
        _builder.Clear();

        registryKey.
            CreateTreeKey(System.IO.Path.GetFileName(registryKey.Name)).
            PushToStack(_stack);

        string source = registryKey.Name;

        while (_stack.Count > 0 && !Canceled)
        {
            (TreeRegistryKey tree, registryKey) = _stack.Pop();
            int depth = tree.Depth + 1;

            using (registryKey)
            {
                if (depth <= Depth)
                {
                    if (KeysOnly)
                    {
                        goto PushKeys;
                    }

                    foreach (string value in registryKey.GetValueNames())
                    {
                        if (ShouldSkipValue(value))
                        {
                            continue;
                        }

                        new TreeRegistryValue(registryKey, value, source, depth)
                            .AddParent<TreeRegistryValue>(tree)
                            .SetIncludeFlagIf(WithInclude)
                            .AddToCache(_builder);
                    }

                PushKeys:
                    foreach (string keyname in registryKey.EnumerateKeys())
                    {
                        if (ShouldExclude(keyname))
                        {
                            continue;
                        }

                        try
                        {
                            RegistryKey? subkey = registryKey.OpenSubKey(keyname);

                            if (subkey is null)
                            {
                                continue;
                            }

                            subkey
                                .CreateTreeKey(keyname, source, depth)
                                .AddParent(tree)
                                .PushToStack(_stack);
                        }
                        catch (SecurityException exception)
                        {
                            string path = System.IO.Path.Combine(registryKey.Name, keyname);
                            WriteError(exception.ToSecurityError(path));
                        }
                    }
                }
            }

            _builder.Add(tree);
            _builder.Flush();
        }

        return _builder.GetTree(WithInclude && !KeysOnly).Format();
    }

    private bool ShouldSkipValue(string value) => ShouldExclude(value) || !ShouldInclude(value);

    private bool TryGetKey(string path, [NotNullWhen(true)] out RegistryKey? key)
    {
        (string @base, string? subkey) = path.Split(['\\'], 2);
        key = default;

        if (!RegistryMappings.TryGetKey(@base, out RegistryKey? value))
        {
            return false;
        }

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
