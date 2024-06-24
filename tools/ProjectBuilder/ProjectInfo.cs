using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Xml;

namespace ProjectBuilder;

public sealed class ProjectInfo
{
    public DirectoryInfo Root { get; }

    public Module Module { get; }

    public Configuration Configuration { get; internal set; }

    public Documentation Documentation { get; internal set; }

    public Project Project { get; }

    public Pester Pester { get; }

    public string? AnalyzerPath
    {
        get
        {
            _analyzerPath ??= Path.Combine(
                Root.FullName,
                "ScriptAnalyzerSettings.psd1");

            if (File.Exists(_analyzerPath))
            {
                return _analyzerPath;
            }

            return null;
        }
    }

    private string? _analyzerPath;

    private ProjectInfo(string path)
    {
        Root = AssertDirectory(path);

        Module = new Module(
            directory: AssertDirectory(GetModulePath(path)),
            name: Path.GetFileNameWithoutExtension(path),
            info: this);

        Project = new Project(
            source: AssertDirectory(GetSourcePath(path, Module.Name)),
            build: GetBuildPath(path),
            info: this);

        Pester = new(this);
    }

    public static ProjectInfo Create(
        string path,
        Configuration configuration)
    {
        ProjectInfo builder = new(path)
        {
            Configuration = configuration
        };
        builder.Module.Manifest = GetManifest(builder);
        builder.Module.Version = GetManifestVersion(builder);
        builder.Project.Release = GetReleasePath(
            builder.Project.Build,
            builder.Module.Name,
            builder.Module.Version!);
        builder.Project.TargetFrameworks = GetTargetFrameworks(GetProjectFile(builder));
        builder.Documentation = new Documentation
        {
            Source = Path.Combine(builder.Root.FullName, "docs", "en-US"),
            Output = Path.Combine(builder.Project.Release, "en-US")
        };

        return builder;
    }

    public IEnumerable<string> GetRequirements()
    {
        string req = Path.Combine(Root.FullName, "tools", "requiredModules.psd1");
        if (!File.Exists(req))
        {
            return [];
        }
        return Module.GetRequirements(req);
    }

    public void CleanRelease()
    {
        if (Directory.Exists(Project.Release))
        {
            Directory.Delete(Project.Release, recursive: true);
        }
        Directory.CreateDirectory(Project.Release);
    }

    public string[] GetBuildArgs() =>
    [
        "publish",
        "--configuration", Configuration.ToString(),
        "--verbosity", "q",
        "-nologo",
        $"-p:Version={Module.Version}"
    ];

    public Hashtable GetAnalyzerParams() => new()
    {
        ["Path"] = Project.Release,
        ["Settings"] = AnalyzerPath,
        ["Recurse"] = true,
        ["ErrorAction"] = "SilentlyContinue"
    };

    private static string[] GetTargetFrameworks(string path)
    {
        XmlDocument xmlDocument = new();
        xmlDocument.Load(path);
        return xmlDocument
            .SelectSingleNode("Project/PropertyGroup/TargetFrameworks")
            .InnerText
            .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
    }

    private static string GetBuildPath(string path) =>
        Path.Combine(path, "output");

    private static string GetSourcePath(string path, string moduleName) =>
        Path.Combine(path, "src", moduleName);

    private static string GetModulePath(string path) =>
        Path.Combine(path, "module");

    private static string GetReleasePath(
        string buildPath,
        string moduleName,
        Version version) => Path.Combine(
            buildPath,
            moduleName,
            LanguagePrimitives.ConvertTo<string>(version));

    private static DirectoryInfo AssertDirectory(string path)
    {
        DirectoryInfo directory = new(path);
        return directory.Exists ? directory
            : throw new ArgumentException(
                $"Path '{path}' could not be found or is not a Directory.",
                nameof(path));
    }

    private static FileInfo GetManifest(ProjectInfo builder) =>
        builder.Module.Root.EnumerateFiles("*.psd1").FirstOrDefault()
            ?? throw new FileNotFoundException(
                $"Manifest file could not be found in '{builder.Root.FullName}'");

    private static string GetProjectFile(ProjectInfo builder) =>
        builder.Project.Source.EnumerateFiles("*.csproj").FirstOrDefault()?.FullName
            ?? throw new FileNotFoundException(
                $"Project file could not be found in ''{builder.Project.Source.FullName}'");

    private static Version? GetManifestVersion(ProjectInfo builder)
    {
        using PowerShell powershell = PowerShell.Create(RunspaceMode.CurrentRunspace);
        Hashtable? moduleInfo = powershell
            .AddCommand("Import-PowerShellDataFile")
            .AddArgument(builder.Module.Manifest?.FullName)
            .Invoke<Hashtable>()
            .FirstOrDefault();

        return powershell.HadErrors
            ? throw powershell.Streams.Error.First().Exception
            : LanguagePrimitives.ConvertTo<Version>(moduleInfo?["ModuleVersion"]);
    }
}
