using PSTree.Interfaces;

namespace PSTree.Comparers;

internal static class TreeComparers
{
    internal static int ByContainersFirst<T>(T x, T y)
        where T : ITree
    {
        if (x.IsContainer != y.IsContainer)
            return x.IsContainer ? -1 : 1;

        return x.Name.CompareTo(y.Name);
    }

    internal static int ByLeavesFirst<T>(T x, T y)
        where T : ITree
    {
        if (x.IsContainer != y.IsContainer)
            return x.IsContainer ? 1 : -1;

        return x.Name.CompareTo(y.Name);
    }

    internal static int BySize<T>(T x, T y)
        where T : IFileSystemNode
    {
        return y.Length.CompareTo(x.Length);
    }
}
