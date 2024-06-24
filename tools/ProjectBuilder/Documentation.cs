using System.Collections;

namespace ProjectBuilder;

public record struct Documentation(string Source, string Output)
{
    public readonly Hashtable GetParams() => new()
    {
        ["Path"] = Source,
        ["OutputPath"] = Output
    };
}
