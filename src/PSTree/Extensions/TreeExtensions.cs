namespace PSTree.Extensions;

internal static class TreeExtensions
{
    extension<TBase, TLeaf>(TLeaf leaf)
        where TLeaf : TBase
        where TBase : ITree
    {
        internal void AddTo(TreeBuilder<TBase, TLeaf> cache) => cache.Add(leaf);
    }
#if WINDOWS
    extension(string[] strings)
    {
        internal void Deconstruct(out string baseKey, out string? subKey)
        {
            baseKey = strings[0];
            subKey = strings.Length == 1 ? null : strings[1];
        }
    }
#endif
}
