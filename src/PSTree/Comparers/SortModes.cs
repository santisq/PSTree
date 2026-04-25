namespace PSTree.Comparers;

public enum FileSystemSortMode
{
    FilesFirst,
    DirectoriesFirst,
    Size
}

#if WINDOWS
public enum RegistrySortMode
{
    ValuesFirst,
    KeysFirst
}
#endif
