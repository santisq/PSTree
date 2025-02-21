using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;
using System.Security;
using Microsoft.Win32;
using PSTree.Extensions;

namespace PSTree.Commands;

[Cmdlet(VerbsCommon.Get, "PSTreeRegistry", DefaultParameterSetName = PathSet)]
[OutputType(typeof(TreeRegistryKey), typeof(TreeRegistryValue))]
[Alias("pstreereg")]
public sealed class GetPSTreeRegistryCommand : CommandWithPathBase
{
    private readonly Cache<TreeRegistryBase, TreeRegistryValue> _cache = new();

    private readonly Stack<(TreeRegistryKey, RegistryKey)> _stack = [];

    [Parameter]
    [ValidateRange(0, int.MaxValue)]
    public int Depth { get; set; } = 3;

    [Parameter]
    public SwitchParameter Recurse { get; set; }

    protected override void BeginProcessing()
    {
        this.ThrowIfNotSupportedPlatform();
        if (Recurse.IsPresent && !MyInvocation.BoundParameters.ContainsKey(nameof(Depth)))
        {
            Depth = int.MaxValue;
        }
    }

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

    private TreeBase[] Traverse(RegistryKey key)
    {
        _cache.Clear();
        _stack.Push(key.CreateTreeKey(System.IO.Path.GetFileName(key.Name)));
        string source = key.Name;

        while (_stack.Count > 0)
        {
            (TreeRegistryKey tree, key) = _stack.Pop();
            int depth = tree.Depth + 1;

            using (key)
            {
                if (depth <= Depth)
                {
                    foreach (string value in key.GetValueNames())
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            continue;
                        }

                        _cache.Add(new TreeRegistryValue(key, value, source, depth));
                    }

                    PushSubKeys(key, source, depth);
                }
            }

            _cache.Add(tree);
            _cache.Flush();
        }

        return _cache.Items.ToArray().Format();
    }

    private void PushSubKeys(RegistryKey key, string source, int depth)
    {
        foreach (string keyname in key.GetSubKeyNames())
        {
            try
            {
                RegistryKey? subkey = key.OpenSubKey(keyname);

                if (subkey is null)
                {
                    continue;
                }

                _stack.Push(subkey.CreateTreeKey(keyname, source, depth));
            }
            catch (Exception exception)
            {
                WriteError(exception.ToNotSpecifiedError(keyname));
            }
        }
    }
}
