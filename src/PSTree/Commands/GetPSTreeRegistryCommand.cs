using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;
using System.Security;
using Microsoft.Win32;
using PSTree.Extensions;

namespace PSTree.Commands;

[Cmdlet(VerbsCommon.Get, "PSTreeRegistry", DefaultParameterSetName = PathSet)]
[OutputType(typeof(PSTreeRegistryKey), typeof(PSTreeRegistryValue))]
[Alias("pstreereg")]
public sealed class GetPSTreeRegistryCommand : CommandWithPathBase
{
    private readonly List<PSTreeRegistryValue> _values = [];

    private readonly List<PSTreeRegistryBase> _result = [];

    private readonly Stack<(PSTreeRegistryKey, RegistryKey)> _stack = [];

    private readonly Dictionary<string, RegistryKey> _map = new()
    {
        ["HKEY_CURRENT_USER"] = Registry.CurrentUser,
        ["HKEY_LOCAL_MACHINE"] = Registry.LocalMachine
    };

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
        (string @base, string subkey) = path.Split(['\\'], 2);
        key = default;

        if (!_map.TryGetValue(@base, out RegistryKey? value))
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

    private PSTreeBase[] Traverse(RegistryKey key)
    {
        Clear();
        _stack.Push(key.CreateTreeKey(System.IO.Path.GetFileName(key.Name)));

        while (_stack.Count > 0)
        {
            (PSTreeRegistryKey tree, key) = _stack.Pop();
            int depth = tree.Depth + 1;

            foreach (string value in key.GetValueNames())
            {
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                _values.Add(new PSTreeRegistryValue(key, value, depth));
            }

            if (depth <= Depth)
            {
                PushSubKeys(key, depth);
            }

            _result.Add(tree);

            if (_values.Count > 0)
            {
                _result.AddRange([.. _values]);
                _values.Clear();
            }

            key.Dispose();
        }

        return _result.ToArray().Format();
    }

    private void PushSubKeys(RegistryKey key, int depth)
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

                _stack.Push(subkey.CreateTreeKey(keyname, depth));
            }
            catch (Exception exception)
            {
                WriteError(exception.ToNotSpecifiedError(keyname));
            }
        }
    }

    private void Clear()
    {
        _stack.Clear();
        _values.Clear();
        _result.Clear();
    }
}
