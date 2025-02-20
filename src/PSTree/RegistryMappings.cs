using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Win32;

namespace PSTree;

internal static class RegistryMappings
{
    internal static bool TryGetKey(string key, [NotNullWhen(true)] out RegistryKey? value) =>
        _map.TryGetValue(key, out value);

    private static readonly Dictionary<string, RegistryKey> _map = new()
    {
        ["HKEY_CURRENT_USER"] = Registry.CurrentUser,
        ["HKEY_LOCAL_MACHINE"] = Registry.LocalMachine,
        ["HKEY_CLASSES_ROOT"] = Registry.ClassesRoot,
        ["HKEY_USERS"] = Registry.Users,
        ["HKEY_CURRENT_CONFIG"] = Registry.CurrentConfig
    };
}
