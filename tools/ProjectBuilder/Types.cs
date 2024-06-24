using System;

namespace ProjectBuilder;

public enum Configuration
{
    Debug,
    Release
}

internal record struct ModuleDownload(string Module, Version Version);
