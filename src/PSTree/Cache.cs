using System.Collections.Generic;
using System.Linq;
using PSTree.Extensions;

namespace PSTree;

internal sealed class Cache
{
    private readonly List<PSTreeFileSystemInfo> _items = [];

    private readonly List<PSTreeFile> _files = [];

    internal void Add(PSTreeFile file) => _files.Add(file);

    internal void Add(PSTreeDirectory directory) => _items.Add(directory);

    internal void Flush()
    {
        if (_files.Count > 0)
        {
            _items.AddRange([.. _files]);
            _files.Clear();
        }
    }

    internal PSTreeFileSystemInfo[] GetTree(bool condition) =>
        condition
            ? _items.Where(IsIncluded).ToArray().Format()
            : _items.ToArray().Format();

    private static bool IsIncluded(PSTreeFileSystemInfo item)
    {
        if (item.ShouldInclude && item is PSTreeDirectory dir)
        {
            dir.IncrementItemCount();
        }

        return item.ShouldInclude;
    }

    internal void Clear()
    {
        _files.Clear();
        _items.Clear();
    }
}
