using PSTree.Internal;

namespace PSTree;

public abstract class TreeFileSystemInfo(string source)
    : TreeBase<TreeDirectory>(source)
{
    public abstract string Name { get; }

    public long Length { get; internal set; }

    public string GetFormattedLength()
        => _FormattingInternals.GetFormattedLength(Length);

    internal void RecursiveDecrement()
    {
        TreeDirectory? parent = Container;
        if (parent is null) return;

        parent.ItemCount--;
        parent.TotalItemCount--;

        for (parent = parent.Container; parent is not null; parent = parent.Container)
            parent.TotalItemCount--;
    }
}
