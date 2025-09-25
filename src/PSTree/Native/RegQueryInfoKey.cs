#if WINDOWS
using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace PSTree.Native;

internal static partial class WinAPI
{
#if NET8_0_OR_GREATER
    [LibraryImport("advapi32.dll", EntryPoint = "RegQueryInfoKeyW")]
    private static partial int RegQueryInfoKey(
#else
    [DllImport("advapi32.dll")]
    private static extern int RegQueryInfoKey(
#endif
        SafeRegistryHandle hkey,
        IntPtr lpClass,
        ref uint lpcbClass,
        IntPtr lpReserved,
        IntPtr lpcSubKeys,
        IntPtr lpcbMaxSubKeyLen,
        IntPtr lpcbMaxClassLen,
        IntPtr lpcValues,
        IntPtr lpcbMaxValueNameLen,
        IntPtr lpcbMaxValueLen,
        IntPtr lpcbSecurityDescriptor,
        out long lpftLastWriteTime);

    internal static DateTime? GetLastWriteTime(this RegistryKey key)
    {
        uint lpcbClass = 0;

        int result = RegQueryInfoKey(
            hkey: key.Handle,
            lpClass: IntPtr.Zero,
            lpcbClass: ref lpcbClass,
            lpReserved: IntPtr.Zero,
            lpcSubKeys: IntPtr.Zero,
            lpcbMaxSubKeyLen: IntPtr.Zero,
            lpcbMaxClassLen: IntPtr.Zero,
            lpcValues: IntPtr.Zero,
            lpcbMaxValueNameLen: IntPtr.Zero,
            lpcbMaxValueLen: IntPtr.Zero,
            lpcbSecurityDescriptor: IntPtr.Zero,
            lpftLastWriteTime: out long lpftLastWriteTime);

        return result == 0
            ? DateTime.FromFileTime(lpftLastWriteTime) : null;
    }
}
#endif
