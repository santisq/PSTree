using System.Collections.Generic;
using System.IO;

namespace PSTree;

internal sealed class PSTreeIndexer
{
    private readonly Dictionary<string, PSTreeDirectory> _indexer = [];

    internal void IndexLength(PSTreeDirectory directory, long length)
    {
        _indexer[directory.FullName.TrimEnd(Path.DirectorySeparatorChar)] = directory;

        foreach (string parent in directory.Parents)
        {
            if (_indexer.TryGetValue(parent, out PSTreeDirectory paretDir))
            {
                paretDir.Length += length;
            }
        }
    }

    internal void IndexItemCount(PSTreeDirectory directory, int count)
    {
        directory.ItemCount = count;
        directory.TotalItemCount = count;
        _indexer[directory.FullName.TrimEnd(Path.DirectorySeparatorChar)] = directory;

        foreach (string parent in directory.Parents)
        {
            if (_indexer.TryGetValue(parent, out PSTreeDirectory parentDir))
            {
                parentDir.TotalItemCount += count;
            }
        }
    }

    internal void Clear() => _indexer.Clear();
}
