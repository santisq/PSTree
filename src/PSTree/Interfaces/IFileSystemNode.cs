namespace PSTree.Interfaces;

public interface IFileSystemNode : ITree
{
    long Length { get; }
}
