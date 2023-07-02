using System.Collections.Generic;
using System.IO;

namespace PSTree;

internal class PSTreeIndexer
{
    private readonly Dictionary<string, PSTreeDirectory> _indexer = new();

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
