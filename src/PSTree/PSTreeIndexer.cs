using System.Collections.Generic;
using System.IO;

namespace PSTree;

internal sealed class PSTreeIndexer
{
    private readonly Dictionary<string, PSTreeDirectory> _indexer = [];

    internal void Index(PSTreeDirectory directory, long length)
    {
        _indexer[directory.FullName.TrimEnd(Path.DirectorySeparatorChar)] = directory;

        foreach (string parent in directory.GetParents())
        {
            if (_indexer.ContainsKey(parent))
            {
                _indexer[parent].Length += length;
            }
        }
    }

    internal void Clear() => _indexer.Clear();
}
