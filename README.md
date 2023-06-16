<h1 align="center">PSTree</h1>

<p align="center">
    <sub>
        Tree like cmdlet for PowerShell!
    </sub>
    <br /><br />
    <a title="Commits" href="https://github.com/santisq/PSTree/actions/workflows/ci.yml">
        <img alt="Build Status" src="https://github.com/santisq/PSTree/actions/workflows/ci.yml/badge.svg" />
    </a>
    <a title="PSTree on PowerShell Gallery" href="https://www.powershellgallery.com/packages/PSTree">
        <img alt="PowerShell Gallery Version" src="https://img.shields.io/powershellgallery/v/PSTree?label=gallery">
    </a>
    <a title="LICENSE" href="https://github.com/santisq/PSTree/blob/main/LICENSE">
        <img alt="GitHub" src="https://img.shields.io/github/license/santisq/PSTree">
    </a>
</p>

`Get-PSTree` is a PowerShell cmdlet that intends to emulate the [`tree` command](https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/tree) with added functionality to calculate the __folders size__ as well as __recursive folders size__.

## Installation

The module is available through the [PowerShell Gallery](https://www.powershellgallery.com/packages/PSTree):

```powershell
Install-Module PSTree -Scope CurrentUser
```

## Requirements

Compatible with __Windows PowerShell v5.1__ and __PowerShell 7+__.

## Usage

### Get the current directory hierarchy with default parameters values

```powershell
PS ..\PSTree> Get-PSTree

Mode  Hierarchy                         Size
----  ---------                         ----
d---- PSTree                        27.27 Kb
-a--- ├── .gitignore                 4.64 Kb
-a--- ├── build.ps1                  2.16 Kb
-a--- ├── CHANGELOG.md               5.44 Kb
-a--- ├── LICENSE                    1.07 Kb
-a--- ├── PSTree.build.ps1            8.1 Kb
-a--- ├── README.md                  5.87 Kb
d---- ├── tools                    107 Bytes
-a--- │   ├── requiredModules.psd1 107 Bytes
d---- │   └── Modules                0 Bytes
d---- │       ├── PSScriptAnalyzer   1.44 Mb
d---- │       ├── platyPS          401.83 Kb
d---- │       └── InvokeBuild      143.43 Kb
d---- ├── src                        0 Bytes
d---- │   └── PSTree                  5.3 Kb
-a--- │       ├── PSTree.csproj    439 Bytes
-a--- │       ├── PSTreeStatic.cs     1.8 Kb
-a--- │       ├── PSTreeTypes.cs     3.07 Kb
d---- │       ├── obj               16.53 Kb
d---- │       ├── Commands           5.98 Kb
d---- │       └── bin                0 Bytes
d---- ├── output                    20.34 Kb
-a--- │   ├── PSTree.2.1.9.nupkg    20.34 Kb
d---- │   └── PSTree                 0 Bytes
d---- │       └── 2.1.9              5.85 Kb
d---- ├── module                     5.85 Kb
-a--- │   ├── PSTree.Format.ps1xml   1.23 Kb
-a--- │   └── PSTree.psd1            4.62 Kb
d---- ├── docs                       0 Bytes
d---- │   └── en-US                   4.3 Kb
-a--- │       └── Get-PSTree.md       4.3 Kb
d---- ├── .vscode                    4.07 Kb
-a--- │   ├── extensions.json      275 Bytes
-a--- │   ├── launch.json            1.35 Kb
-a--- │   ├── settings.json          1.02 Kb
-a--- │   └── tasks.json             1.43 Kb
d---- └── .github                    0 Bytes
d----     └── workflows              1.77 Kb
-a---         └── ci.yml             1.77 Kb
```

### Exclude `.vscode` and `.github` folders

```powershell
PS ..\PSTree> Get-PSTree -Exclude *.vscode, *.github

Mode  Hierarchy                         Size
----  ---------                         ----
d---- PSTree                        27.82 Kb
-a--- ├── .gitignore                 4.64 Kb
-a--- ├── build.ps1                  2.16 Kb
-a--- ├── CHANGELOG.md               5.44 Kb
-a--- ├── LICENSE                    1.07 Kb
-a--- ├── PSTree.build.ps1            8.1 Kb
-a--- ├── README.md                  6.42 Kb
d---- ├── tools                    107 Bytes
-a--- │   ├── requiredModules.psd1 107 Bytes
d---- │   └── Modules                0 Bytes
d---- │       ├── PSScriptAnalyzer   1.44 Mb
d---- │       ├── platyPS          401.83 Kb
d---- │       └── InvokeBuild      143.43 Kb
d---- ├── src                        0 Bytes
d---- │   └── PSTree                  5.3 Kb
-a--- │       ├── PSTree.csproj    439 Bytes
-a--- │       ├── PSTreeStatic.cs     1.8 Kb
-a--- │       ├── PSTreeTypes.cs     3.07 Kb
d---- │       ├── obj               16.53 Kb
d---- │       ├── Commands           5.98 Kb
d---- │       └── bin                0 Bytes
d---- ├── output                    20.34 Kb
-a--- │   ├── PSTree.2.1.9.nupkg    20.34 Kb
d---- │   └── PSTree                 0 Bytes
d---- │       └── 2.1.9              5.85 Kb
d---- ├── module                     5.85 Kb
-a--- │   ├── PSTree.Format.ps1xml   1.23 Kb
-a--- │   └── PSTree.psd1            4.62 Kb
d---- └── docs                       0 Bytes
d----     └── en-US                   4.3 Kb
-a---         └── Get-PSTree.md       4.3 Kb
```

### Get the hierarchy of `C:\Windows\System32` recursively displaying only folders

```powershell
PS ..\PSTree> $tree = Get-PSTree C:\Windows\System32\ -Directory -Recurse -EA 0
PS ..\PSTree> $tree | Select-Object -First 20

Mode  Hierarchy                                  Size
----  ---------                                  ----
d---- System32                                2.12 Gb
d---- ├── zh-TW                              204.5 Kb
d---- ├── zh-CN                             234.99 Kb
d---- ├── winrm                               0 Bytes
d---- │   └── 0409                          100.12 Kb
d---- ├── WinMetadata                         6.18 Mb
d---- ├── winevt                              0 Bytes
d---- │   ├── TraceFormat                     0 Bytes
d---- │   └── Logs                          296.28 Mb
d---- ├── WindowsPowerShell                   0 Bytes
d---- │   └── v1.0                            1.73 Mb
d---- │       ├── SessionConfig               0 Bytes
d---- │       ├── Schemas                     0 Bytes
d---- │       │   └── PSMaml                532.76 Kb
d---- │       ├── Modules                     0 Bytes
d---- │       │   ├── WindowsUpdateProvider  16.51 Kb
d---- │       │   ├── WindowsUpdate          16.09 Kb
d---- │       │   ├── WindowsSearch          17.83 Kb
d---- │       │   │   └── en                   5.5 Kb
d---- │       │   ├── WindowsErrorReporting    7.4 Kb
```

### Recurse subdirectories only 2 levels in Depth

```powershell
PS ..\PSTree> $tree = Get-PSTree C:\Windows\System32\ -Directory -Depth 2 -EA 0
PS ..\PSTree> $tree | Select-Object -First 20

Mode  Hierarchy                  Size
----  ---------                  ----
d---- System32                2.12 Gb
d---- ├── zh-TW              204.5 Kb
d---- ├── zh-CN             234.99 Kb
d---- ├── winrm               0 Bytes
d---- │   └── 0409          100.12 Kb
d---- ├── WinMetadata         6.18 Mb
d---- ├── winevt              0 Bytes
d---- │   ├── TraceFormat     0 Bytes
d---- │   └── Logs          296.28 Mb
d---- ├── WindowsPowerShell   0 Bytes
d---- │   └── v1.0            1.73 Mb
d---- ├── WinBioPlugIns       1.86 Mb
d---- │   ├── FaceDriver    717.47 Kb
d---- │   └── en-US           0 Bytes
d---- ├── WinBioDatabase      1.12 Kb
d---- ├── WDI                 0 Bytes
d---- ├── WCN                 0 Bytes
d---- │   └── en-US           0 Bytes
d---- ├── wbem                66.4 Mb
d---- │   ├── xml            99.87 Kb
```

### Get the recursive size of the folders

```powershell
PS ..\PSTree> $tree = Get-PSTree C:\Windows\System32\ -Directory -Depth 2 -RecursiveSize -EA 0
PS ..\PSTree> $tree | Select-Object -First 20

Mode  Hierarchy                  Size
----  ---------                  ----
d---- System32                7.73 Gb
d---- ├── zh-TW              204.5 Kb
d---- ├── zh-CN             234.99 Kb
d---- ├── winrm             100.12 Kb
d---- │   └── 0409          100.12 Kb
d---- ├── WinMetadata         6.18 Mb
d---- ├── winevt            296.28 Mb
d---- │   ├── TraceFormat     0 Bytes
d---- │   └── Logs          296.28 Mb
d---- ├── WindowsPowerShell  10.55 Mb
d---- │   └── v1.0           10.55 Mb
d---- ├── WinBioPlugIns      56.33 Mb
d---- │   ├── FaceDriver     54.48 Mb
d---- │   └── en-US           0 Bytes
d---- ├── WinBioDatabase      1.12 Kb
d---- ├── WDI                 0 Bytes
d---- ├── WCN                 0 Bytes
d---- │   └── en-US           0 Bytes
d---- ├── wbem               110.6 Mb
d---- │   ├── xml            99.87 Kb
```

## Documentation

See the [`Get-PSTree` doc](/docs/en-US/Get-PSTree.md) for parameter details and syntax.

## Changelog

- [CHANGELOG.md](CHANGELOG.md)
- [Releases](https://github.com/santisq/PSTree/releases)

## Contributing

Contributions are more than welcome, if you wish to contribute, fork this repository and submit a pull request with the changes.
