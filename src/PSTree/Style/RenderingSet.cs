namespace PSTree.Style;

internal readonly struct RenderingSet
{
    internal static RenderingSet Fancy = new("│   ", "├── ", "└── ");

    internal readonly string Space = "    ";
    internal readonly string Vertical;
    internal readonly string Branch;
    internal readonly string LastBranch;

    private RenderingSet(string vertical, string branch, string lastBranch)
    {
        Vertical = vertical;
        Branch = branch;
        LastBranch = lastBranch;
    }
}
