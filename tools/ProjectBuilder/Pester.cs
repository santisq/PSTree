using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace ProjectBuilder;

public sealed class Pester
{
    public string? PesterScript
    {
        get
        {
            _pesterPath ??= Path.Combine(
                _info.Root.FullName,
                "tools",
                "PesterTest.ps1");

            if (_testsExist = File.Exists(_pesterPath))
            {
                return _pesterPath;
            }

            return null;
        }
    }

    public string? ResultPath { get => _testsExist ? Path.Combine(_info.Project.Build, "TestResults") : null; }

    public string? ResultFile { get => _testsExist ? Path.Combine(ResultPath, "Pester.xml") : null; }

    private readonly ProjectInfo _info;

    private string? _pesterPath;

    private bool _testsExist;

    internal Pester(ProjectInfo info) => _info = info;

    private void CreateResultPath()
    {
        if (!Directory.Exists(ResultPath))
        {
            Directory.CreateDirectory(ResultPath);
        }
    }

    public void ClearResultFile()
    {
        if (File.Exists(ResultFile))
        {
            File.Delete(ResultFile);
        }
    }

    public string[] GetTestArgs(Version version)
    {
        CreateResultPath();

        List<string> arguments = [
            "-NoProfile",
            "-NonInteractive",
        ];

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            arguments.AddRange([ "-ExecutionPolicy", "Bypass" ]);
        }

        arguments.AddRange([
            "-File", PesterScript!,
            "-TestPath", Path.Combine(_info.Root.FullName, "tests"),
            "-OutputFile", ResultFile!
        ]);

        Regex re = new("^|$", RegexOptions.Compiled);
        string targetArgs = re.Replace(string.Join("\" \"", [.. arguments]), "\"");
        string pwsh = Regex.Replace(Environment.GetCommandLineArgs().First(), @"\.dll$", string.Empty);
        string unitCoveragePath = Path.Combine(ResultPath, "UnitCoverage.json");
        string watchFolder = Path.Combine(_info.Project.Release, "bin", _info.Project.TestFramework);
        string sourceMappingFile = Path.Combine(ResultPath, "CoverageSourceMapping.txt");

        if (version is not { Major: >= 7, Minor: > 0 })
        {
            targetArgs = re.Replace(targetArgs, "\"");
            watchFolder = re.Replace(watchFolder, "\"");
        }

        arguments.Clear();
        arguments.AddRange([
            watchFolder,
            "--target", pwsh,
            "--targetargs", targetArgs,
            "--output", Path.Combine(ResultPath, "Coverage.xml"),
            "--format", "cobertura"
        ]);

        if (File.Exists(unitCoveragePath))
        {
            arguments.AddRange([ "--merge-with", unitCoveragePath ]);
        }

        if (Environment.GetEnvironmentVariable("GITHUB_ACTIONS") is "true")
        {
            arguments.AddRange([ "--source-mapping-file", sourceMappingFile ]);
            File.WriteAllText(
                sourceMappingFile,
                $"|{_info.Root.FullName}{Path.DirectorySeparatorChar}=/_/");
        }

        return [.. arguments];
    }
}
