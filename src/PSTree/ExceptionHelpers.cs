using System;
using System.Management.Automation;

namespace PSTree;

internal static class ExceptionHelpers
{
    internal static ErrorRecord InvalidPathError(string path) =>
        new(new Exception($"Cannot find path '{path}' because it does not exist."),
            "InvalidPath", ErrorCategory.InvalidArgument, path);

    internal static ErrorRecord InvalidProviderError(string path, ProviderInfo provider) =>
        new(new ArgumentException($"The resolved path '{path}' is not a FileSystem path but '{provider.Name}'."),
            "InvalidProvider", ErrorCategory.InvalidArgument, path);

    internal static ErrorRecord ResolvePathError(string path, Exception e) =>
        new(e, "ResolvePath", ErrorCategory.NotSpecified, path);

    internal static ErrorRecord EnumerationError(PSTreeFileSystemInfo item, Exception e) =>
        new(e, "PSTree.Enumerate", ErrorCategory.NotSpecified, item);
}
