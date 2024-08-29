using System.Collections.Generic;
using System.IO;

namespace PSTree;

public sealed partial class GetPSTreeCommand
{
    class FileSystemEntryComparer : IComparer<FileSystemInfo>
    {
        public int Compare(FileSystemInfo x, FileSystemInfo y)
        {
            if (x is DirectoryInfo && y is DirectoryInfo)
            {
                return string.Compare(y.Name, x.Name); // Directories in descending order
            }

            return string.Compare(x.Name, y.Name); // Files in ascending order
        }
    }
}
