using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace ProjectBuilder;

public sealed class Project
{
    public DirectoryInfo Source { get; }

    public string Build { get; }

    public string? Release { get; internal set; }

    public string[]? TargetFrameworks { get; internal set; }

    public string? TestFramework { get => TargetFrameworks.FirstOrDefault(); }

    private Configuration Configuration { get => _info.Configuration; }

    private readonly ProjectInfo _info;

    internal Project(DirectoryInfo source, string build, ProjectInfo info)
    {
        Source = source;
        Build = build;
        _info = info;
    }

    public void CopyToRelease()
    {
        if (TargetFrameworks is null)
        {
            throw new ArgumentNullException(
                "TargetFrameworks is null.",
                nameof(TargetFrameworks));
        }

        foreach (string framework in TargetFrameworks)
        {
            DirectoryInfo buildFolder = new(Path.Combine(
                Source.FullName,
                "bin",
                Configuration.ToString(),
                framework,
                "publish"));

            string binFolder = Path.Combine(Release, "bin", framework);
            buildFolder.CopyRecursive(binFolder);
        }
    }

    public Hashtable GetPSRepoParams() => new()
    {
        ["Name"] = "LocalRepo",
        ["SourceLocation"] = Build,
        ["PublishLocation"] = Build,
        ["InstallationPolicy"] = "Trusted"
    };

    public void ClearNugetPackage()
    {
        string nugetPath = Path.Combine(
            Build,
            $"{_info.Module.Name}.{_info.Module.Version}.nupkg");

        if (File.Exists(nugetPath))
        {
            File.Delete(nugetPath);
        }
    }
}
