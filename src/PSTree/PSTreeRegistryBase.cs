namespace PSTree;

public abstract class PSTreeRegistryBase(string hierarchy, string source, string path)
    : PSTreeBase(hierarchy, source)
{
    public string Path { get; } = path;
}
