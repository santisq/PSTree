using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Win32;

namespace PSTree;

internal static class RegistryMappings
{
    private static readonly Dictionary<string, RegistryKey> s_map = new(StringComparer.OrdinalIgnoreCase)
    {
        ["HKEY_CURRENT_USER"] = Registry.CurrentUser,
        ["HKEY_LOCAL_MACHINE"] = Registry.LocalMachine,
        ["HKEY_CLASSES_ROOT"] = Registry.ClassesRoot,
        ["HKEY_USERS"] = Registry.Users,
        ["HKEY_CURRENT_CONFIG"] = Registry.CurrentConfig
    };

    internal static bool TryGetKey(string key, [NotNullWhen(true)] out RegistryKey? value)
        => s_map.TryGetValue(key, out value);
}
