using System.Collections.Generic;
using System.IO;

namespace PSTree;

internal sealed class PSTreeCache
{
    private readonly List<PSTreeFileSystemInfo> _items = [];

    private readonly List<PSTreeFile> _files = [];

    internal void AddFile(PSTreeFile file) => _files.Add(file);

    internal void Add(PSTreeFileSystemInfo item) => _items.Add(item);

    internal void TryAddFiles()
    {
        if (_files.Count > 0)
        {
            _items.AddRange([.. _files]);
            _files.Clear();
        }
    }

    internal PSTreeFileSystemInfo[] GetTree() => _items.ToArray().ConvertToTree();

    internal void Clear()
    {
        _files.Clear();
        _items.Clear();
    }
}
