using System;
using System.Management.Automation;

namespace PSTree;

internal static class Exceptions
{
    internal static ErrorRecord ToInvalidPathError(this string path) =>
        new(new Exception($"Cannot find path '{path}' because it does not exist."),
            "InvalidPath", ErrorCategory.InvalidArgument, path);

    internal static ErrorRecord ToInvalidProviderError(this ProviderInfo provider, string path) =>
        new(new ArgumentException($"The resolved path '{path}' is not a FileSystem path but '{provider.Name}'."),
            "InvalidProvider", ErrorCategory.InvalidArgument, path);

    internal static ErrorRecord ToResolvePathError(this Exception exception, string path) =>
        new(exception, "ResolvePath", ErrorCategory.NotSpecified, path);

    internal static ErrorRecord ToEnumerationError(this Exception exception, PSTreeFileSystemInfo item) =>
        new(exception, "EnumerationFailure", ErrorCategory.NotSpecified, item);

    internal static void ThrowInvalidSequence(string vt) => throw new ArgumentException(
        $"The specified string contains printable content when it should only contain ANSI escape sequences: {vt}.");
}