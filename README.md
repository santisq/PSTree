<h1 align="center">PSTree</h1>

`Get-PSTree` is a PowerShell cmdlet that intends to emulate the [`tree` command](https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/tree) with added functionality to calculate the __folders size__ as well as __recursive folders size__.

## Installation

The module is available through the [PowerShell Gallery](https://www.powershellgallery.com/packages/PSTree):

```powershell
Install-Module PSTree -Scope CurrentUser
```

## Requirements

Compatible with __Windows PowerShell v5.1__ and __PowerShell 7+__.

## Usage

### Get the hierarchy of the current Directory with default parameters values

```powershell
PS ..\PSTree> Get-PSTree

Mode  Hierarchy                             Size
----  ---------                             ----
d---- PSTree                            21.65 Kb
-a--- ├── .gitignore                   101 Bytes
-a--- ├── build.ps1                      2.32 Kb
-a--- ├── CHANGELOG.md                   5.13 Kb
-a--- ├── LICENSE                        1.07 Kb
-a--- ├── PSTree.build.ps1               8.91 Kb
-a--- ├── publish.ps1                  150 Bytes
-a--- ├── README.md                      3.97 Kb
d---- ├── tools                         74 Bytes
-a--- │   └── requiredModules.psd1      74 Bytes
d---- ├── src                            0 Bytes
d---- │   └── PSTree                    10.58 Kb
-a--- │       ├── PSTree.cs             10.16 Kb
-a--- │       └── PSTree.csproj        439 Bytes
d---- ├── PSTree                         5.83 Kb
-a--- │   ├── PSTree.Format.ps1xml       1.23 Kb
-a--- │   └── PSTree.psd1                 4.6 Kb
d---- ├── docs                           0 Bytes
d---- │   └── en-US                      3.68 Kb
-a--- │       └── Get-PSTree.md          3.68 Kb
d---- ├── .vscode                        1.89 Kb
-a--- │   ├── launch.json                1.35 Kb
-a--- │   └── tasks.json               550 Bytes
d---- └── .github                        0 Bytes
d----     └── workflows                  1.01 Kb
-a---         ├── ondemand publish.yml 465 Bytes
-a---         └── release publish.yml  565 Bytes
```

### Get the hierarchy of `C:\Windows\System32` recursively displaying only folders

```powershell
PS ..\PSTree> Get-PSTree C:\Windows\System32\ -Directory -Recurse -EA 0 | Select-Object -First 20

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

### Get the hierarchy of `C:\Windows\System32` 2 levels deep displaying hidden files and folders

```
PS ..\PSTree> Get-PSTree -Depth 2 -Force

Mode  Hierarchy                   Size
----  ---------                   ----
d---- PSTree                   8.46 Kb
-a--- ├── LICENSE              1.07 Kb
-a--- ├── README.md             7.4 Kb
d---- ├── PSTree               4.83 Kb
-a--- │   ├── PSTree.psd1      4.57 Kb
-a--- │   ├── PSTree.psm1    270 Bytes
d---- │   ├── public           5.96 Kb
d---- │   ├── private          0 Bytes
d---- │   └── Format           1.83 Kb
d--h- └── .git                 1.69 Kb
-a---     ├── COMMIT_EDITMSG   2 Bytes
-a---     ├── config         296 Bytes
-a---     ├── description     73 Bytes
-a---     ├── FETCH_HEAD     198 Bytes
-a---     ├── HEAD            21 Bytes
-a---     ├── index          926 Bytes
-a---     ├── ORIG_HEAD       41 Bytes
-a---     ├── packed-refs    177 Bytes
d----     ├── refs             0 Bytes
d----     ├── objects          0 Bytes
d----     ├── logs              2.2 Kb
d----     ├── info           240 Bytes
d----     └── hooks           22.89 Kb
```

### Get hierarchy 2 levels deep displaying only Folders with their recursive size

```
PS ..\PSTree> Get-PSTree -Depth 2 -RecursiveSize -Directory

Mode  Hierarchy            Size
----  ---------            ----
d---- PSTree          181.76 Kb
d---- └── PSTree       15.91 Kb
d----     ├── public    5.96 Kb
d----     ├── private   3.29 Kb
d----     └── Format    1.83 Kb
```

## Documentation

See the [`Get-PSTree` doc](/docs/Get-PSTree.md) for parameter details and syntax.

## Changelog

- [CHANGELOG.md](CHANGELOG.md)
- [Releases](https://github.com/santisq/PSTree/releases)

## Contributing

Contributions are more than welcome, if you wish to contribute, fork this repository and submit a pull request with the changes.
