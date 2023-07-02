using System;
using System.Management.Automation;

namespace PSTree;

internal static class ExceptionHelpers
{
    internal static ErrorRecord InvalidPathError(string path, Exception e) =>
        new(e, "InvalidPath", ErrorCategory.InvalidArgument, path);

    internal static ErrorRecord NotFileSystemPathError(string path, ProviderInfo provider) =>
        new(new ArgumentException($"The resolved path '{path}' is not a FileSystem path but '{provider.Name}'."),
            "NotFileSystemPath", ErrorCategory.InvalidArgument, path);

    internal static ErrorRecord ResolvePathError(string path, Exception e) =>
        new(e, "ResolvePath", ErrorCategory.NotSpecified, path);
}
