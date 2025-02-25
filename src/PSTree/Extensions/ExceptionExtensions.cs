using System;
using System.Management.Automation;
using System.Security;
#if !WINDOWS
using System.Security;
#endif

namespace PSTree.Extensions;

internal static class ExceptionExtensions
{
    internal static ErrorRecord ToInvalidPathError(this string path) =>
        new(
            new ItemNotFoundException(
                $"Cannot find path '{path}' because it does not exist."),
            "InvalidPath", ErrorCategory.InvalidArgument, path);

    internal static ErrorRecord ToInvalidProviderError(
        this ProviderInfo provider,
        string path,
        string expected = "FileSystem") =>
        new(
            new ArgumentException(
                $"The resolved path '{path}' is not a {expected} path but '{provider.Name}'."),
            "InvalidProvider", ErrorCategory.InvalidArgument, path);

    internal static ErrorRecord ToResolvePathError(this Exception exception, string path) =>
        new(exception, "ResolvePath", ErrorCategory.NotSpecified, path);

    internal static ErrorRecord ToEnumerationError(this Exception exception, TreeFileSystemInfo item) =>
        new(exception, "EnumerationFailure", ErrorCategory.NotSpecified, item);

    internal static void ThrowInvalidSequence(this string vt) =>
        throw new ArgumentException(
            $"The specified string contains printable content when it should only contain ANSI escape sequences: '{vt}'.");

    internal static string ThrowIfInvalidExtension(this string extension)
    {
        #if NET6_0_OR_GREATER
        if (extension.StartsWith('.'))
        {
            return extension;
        }
        #else
        if (extension.StartsWith("."))
        {
            return extension;
        }
        #endif

        throw new ArgumentException(
            $"When adding or removing extensions, the extension must start with a period: '{extension}'.");
    }

#if WINDOWS
    internal static ErrorRecord ToSecurityError(this SecurityException exception, string path) =>
        new(exception, "SecurityException", ErrorCategory.OpenError, path);
#else
    internal static void ThrowNotSupportedPlatform(this PSCmdlet cmdlet)
    {
        PlatformNotSupportedException exception = new(
            "The 'Get-PSTreeRegistry' cmdlet is only supported on Windows.");

        cmdlet.ThrowTerminatingError(new ErrorRecord(
            exception, "NotSupportedPlatform", ErrorCategory.InvalidOperation, null));
    }
#endif
}
