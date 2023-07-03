﻿using System.Collections.Generic;
using System.IO;

namespace PSTree;

internal class PSTreeHelper
{
    private readonly List<PSTreeFileSystemInfo> _items;

    private readonly List<PSTreeFile> _files;

    internal PSTreeHelper()
    {
        _items = new();
        _files = new();
    }

    internal void AddFile(FileInfo file, int depth, string source) =>
        _files.Add(new PSTreeFile(file, depth, source));

    internal void Add(PSTreeFileSystemInfo item) =>
        _items.Add(item);

    internal void TryAddFiles()
    {
        if (_files.Count > 0)
        {
            _items.AddRange(_files.ToArray());
            _files.Clear();
        }
    }

    internal PSTreeFileSystemInfo[] GetResult() =>
        _items.ToArray().ConvertToTree();

    internal void Clear()
    {
        _files.Clear();
        _items.Clear();
    }
}
