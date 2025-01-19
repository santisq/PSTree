<h1 align="center">PSTree</h1>

<div align="center">
<sub>Tree like cmdlet for PowerShell!</sub>
<br/><br/>

[![build](https://github.com/santisq/PSTree/actions/workflows/ci.yml/badge.svg)](https://github.com/santisq/PSTree/actions/workflows/ci.yml)
[![codecov](https://codecov.io/gh/santisq/PSTree/branch/main/graph/badge.svg?token=b51IOhpLfQ)](https://codecov.io/gh/santisq/PSTree)
[![PowerShell Gallery](https://img.shields.io/powershellgallery/dt/PSTree?color=%23008FC7)](https://www.powershellgallery.com/packages/PSTree)
[![LICENSE](https://img.shields.io/github/license/santisq/PSTree)](https://github.com/santisq/PSTree/blob/main/LICENSE)

</div>

PSTree is a PowerShell Module that includes the `Get-PSTree` cmdlet that intends to emulate the [`tree` command](https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/tree) with added functionalities to calculate the __folders size__ as well as __recursive folders size__.

## Documentation

Check out [__the docs__](./docs/en-US/Get-PSTree.md) for information about how to use this Module.

For customizing the cmdlet's rendering output see [__about_TreeStyle__](./docs/en-US/about_TreeStyle.md).

## Installation

### Gallery

The module is available through the [PowerShell Gallery](https://www.powershellgallery.com/packages/PSTree):

```powershell
Install-Module PSTree -Scope CurrentUser
```

### Source

```powershell
git clone 'https://github.com/santisq/PSTree.git'
Set-Location ./PSTree
./build.ps1
```

## Requirements

Compatible with __Windows PowerShell v5.1__ and [__PowerShell 7+__](https://github.com/PowerShell/PowerShell).

## Usage

### Get the current directory tree with default parameters values

```powershell
PS ..\PSTree> Get-PSTree | Select-Object -First 20

   Source: C:\User\Documents\PSTree

Mode            Length Hierarchy
----            ------ ---------
d----         31.20 KB PSTree
-a---          4.64 KB ├── .gitignore
-a---        137.00  B ├── .markdownlint.json
-a---          2.16 KB ├── build.ps1
-a---          7.90 KB ├── CHANGELOG.md
-a---          1.07 KB ├── LICENSE
-a---          8.10 KB ├── PSTree.build.ps1
-a---          5.96 KB ├── README.md
-a---          1.23 KB ├── ScriptAnalyzerSettings.psd1
d----          1.74 KB ├── tools
-a---          1.60 KB │   ├── PesterTest.ps1
-a---        141.00  B │   ├── requiredModules.psd1
d----          0.00  B │   └── Modules
d----          1.44 MB │       ├── PSScriptAnalyzer
d----        401.83 KB │       ├── platyPS
d----        771.55 KB │       ├── Pester
d----        143.43 KB │       └── InvokeBuild
d----          6.76 KB ├── tests
-a---          6.76 KB │   └── PSTree.Tests.ps1
d----          0.00  B ├── src
```

### Exclude `tools` and `tests` folders

```powershell
PS ..\PSTree> Get-PSTree -Exclude tools, tests  | Select-Object -First 20

   Source: C:\User\Documents\PSTree

Mode            Length Hierarchy
----            ------ ---------
d----         33.23 KB PSTree
-a---          4.75 KB ├── .gitignore
-a---        137.00  B ├── .markdownlint.json
-a---          1.34 KB ├── build.ps1
-a---         18.08 KB ├── CHANGELOG.md
-a---          1.07 KB ├── LICENSE
-a---          7.85 KB ├── README.md
d----          0.00  B ├── .github
d----          4.10 KB │   └── workflows
-a---          4.10 KB │       └── ci.yml
d----          4.11 KB ├── .vscode
-a---        275.00  B │   ├── extensions.json
-a---          1.39 KB │   ├── launch.json
-a---          1.02 KB │   ├── settings.json
-a---          1.43 KB │   └── tasks.json
d----        229.32 KB ├── assets
-a---         10.00 KB │   ├── EscapeSequence.png
-a---         78.08 KB │   ├── Example.After.png
-a---         73.89 KB │   ├── Example.Before.png
-a---         67.35 KB │   └── TreeStyle.png
```

### Include `.ps1` and `.cs` files and exclude `tools` folder

```powershell
PS ..\PSTree> Get-PStree -Include *.ps1, *.cs -Exclude tools

   Source: C:\User\Documents\PSTree

Mode            Length Hierarchy
----            ------ ---------
d----          1.34 KB PSTree
-a---          1.34 KB ├── build.ps1
d----          0.00  B ├── src
d----         10.70 KB │   └── PSTree
-a---          1.06 KB │       ├── Cache.cs
-a---          2.65 KB │       ├── CommandWithPathBase.cs
-a---          2.98 KB │       ├── PSTreeDirectory.cs
-a---          1.42 KB │       ├── PSTreeFile.cs
-a---          1.69 KB │       ├── PSTreeFileSystemInfo_T.cs
-a---        524.00  B │       ├── PSTreeFileSystemInfo.cs
-a---        404.00  B │       └── TreeComparer.cs
d----         17.10 KB └── tests
-a---        765.00  B     ├── FormattingInternals.tests.ps1
-a---          6.15 KB     ├── GetPSTreeCommand.tests.ps1
-a---          1.77 KB     ├── PSTreeDirectory.tests.ps1
-a---        920.00  B     ├── PSTreeFile.tests.ps1
-a---          2.63 KB     ├── PSTreeFileSystemInfo_T.tests.ps1
-a---          4.90 KB     └── TreeStyle.tests.ps1
```

### Get the `src` tree recursively displaying only folders

```powershell
PS ..\PSTree> Get-PSTree .\src\ -Recurse -Directory

   Source: C:\User\Documents\PSTree\src

Mode            Length Hierarchy
----            ------ ---------
d----          0.00  B src
d----         11.50 KB └── PSTree
d----          0.00  B     ├── bin
d----          0.00  B     │   └── Debug
d----         56.49 KB     │       └── netstandard2.0
d----         56.29 KB     │           └── publish
d----          6.54 KB     ├── Commands
d----          3.63 KB     ├── Extensions
d----          1.14 KB     ├── Internal
d----         16.83 KB     ├── obj
d----          0.00  B     │   └── Debug
d----        112.44 KB     │       └── netstandard2.0
d----          9.28 KB     └── Style
```

### Display subdirectories only 2 levels deep

```powershell
PS ..\PSTree> Get-PSTree .\src\ -Depth 2 -Directory

   Source: C:\User\Documents\PSTree\src

Mode            Length Hierarchy
----            ------ ---------
d----          0.00  B src
d----         10.30 KB └── PSTree
d----         16.53 KB     ├── obj
d----          1.13 KB     ├── Internal
d----          5.68 KB     ├── Commands
d----          0.00  B     └── bin
```

### Get the recursive size of the folders

```powershell
PS ..\PSTree> Get-PSTree .\src\ -Depth 2 -Directory -RecursiveSize

   Source: C:\User\Documents\PSTree\src

Mode            Length Hierarchy
----            ------ ---------
d----        188.08 KB src
d----        188.08 KB └── PSTree
d----        104.55 KB     ├── obj
d----          1.13 KB     ├── Internal
d----          5.68 KB     ├── Commands
d----         66.42 KB     └── bin
```

## Changelog

- [CHANGELOG.md](CHANGELOG.md)
- [Releases](https://github.com/santisq/PSTree/releases)

## Contributing

Contributions are welcome, if you wish to contribute, fork this repository and submit a pull request with the changes.
