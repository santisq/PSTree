using System.IO;
using Microsoft.Win32;

namespace PSTree;

public abstract class PSTreeRegistryBase(string hierarchy, string name)
    : PSTreeBase(hierarchy)
{
    public string Name { get; } = name;

    protected static string Combine(RegistryKey key, string value) =>
        Path.Combine(key.Name, value);
}
