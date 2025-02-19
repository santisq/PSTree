using System;
using System.Management.Automation;
using System.Security;

namespace PSTree.Extensions;

internal static class ExceptionExtensions
{
    internal static ErrorRecord ToInvalidPathError(this string path) =>
        new(
            new Exception(
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

    internal static ErrorRecord ToEnumerationError(this Exception exception, PSTreeFileSystemInfo item) =>
        new(exception, "EnumerationFailure", ErrorCategory.NotSpecified, item);

    internal static void ThrowInvalidSequence(this string vt) =>
        throw new ArgumentException(
            $"The specified string contains printable content when it should only contain ANSI escape sequences: '{vt}'.");

    internal static void ThrowInvalidExtension(this string extension) =>
        throw new ArgumentException(
            $"When adding or removing extensions, the extension must start with a period: '{extension}'.");

    internal static ErrorRecord ToNotSpecifiedError(this Exception exception, object? context = null) =>
        new(exception, exception.GetType().Name, ErrorCategory.NotSpecified, context);

    internal static ErrorRecord ToSecurityError(this SecurityException exception, string path) =>
        new(exception, "SecurityException", ErrorCategory.OpenError, path);
}
