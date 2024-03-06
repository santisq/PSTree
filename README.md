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

   Source: C:\path\to\PSTree

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
PS ..\PSTree> Get-PSTree -Exclude *tools, *tests  | Select-Object -First 20

   Source: C:\path\to\PSTree

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
d----          0.00  B ├── src
d----         10.30 KB │   └── PSTree
-a---        931.00  B │       ├── ExceptionHelpers.cs
-a---        439.00  B │       ├── PSTree.csproj
-a---          1.06 KB │       ├── PSTreeDirectory.cs
-a---          4.01 KB │       ├── PSTreeExtensions.cs
-a---        517.00  B │       ├── PSTreeFile.cs
-a---        399.00  B │       ├── PSTreeFileSystemInfo.cs
-a---          1.51 KB │       ├── PSTreeFileSystemInfo_T.cs
-a---        897.00  B │       ├── PSTreeHelper.cs
-a---        619.00  B │       ├── PSTreeIndexer.cs
```

### Include `.ps1` and `.cs` files and exclude some folders

```powershell
PS ..\PSTree> Get-PStree -Include *.ps1, *.cs -Exclude *output, *tools, *docs, *module

   Source: C:\path\to\PSTree

Mode            Length Hierarchy
----            ------ ---------
d----         33.15 KB PSTree
-a---          2.35 KB ├── build.ps1
-a---          8.10 KB ├── PSTree.build.ps1
d----         13.29 KB ├── tests
-a---        765.00  B │   ├── FormattingInternals.tests.ps1
-a---          5.89 KB │   ├── GetPSTreeCommand.tests.ps1
-a---          1.51 KB │   ├── PathExtensions.tests.ps1
-a---          1.38 KB │   ├── PSTreeDirectory.ps1
-a---        920.00  B │   ├── PSTreeFile.tests.ps1
-a---          2.09 KB │   └── PSTreeFileSystemInfo_T.tests.ps1
d----          0.00  B ├── src
d----         12.15 KB │   └── PSTree
-a---        931.00  B │       ├── ExceptionHelpers.cs
-a---          4.09 KB │       ├── PathExtensions.cs
-a---        900.00  B │       ├── PSTreeCache.cs
-a---          1.06 KB │       ├── PSTreeDirectory.cs
-a---          1.66 KB │       ├── PSTreeExtensions.cs
-a---        517.00  B │       ├── PSTreeFile.cs
-a---        399.00  B │       ├── PSTreeFileSystemInfo.cs
-a---          1.61 KB │       ├── PSTreeFileSystemInfo_T.cs
-a---        626.00  B │       ├── PSTreeIndexer.cs
d----         16.53 KB │       ├── obj
d----          1.15 KB │       ├── Internal
d----          6.43 KB │       ├── Commands
d----          0.00  B │       └── bin
d----          4.07 KB ├── .vscode
d----          0.00  B └── .github
d----          4.17 KB     └── workflows
```

### Get the `src` tree recursively displaying only folders

```powershell
PS ..\PSTree> Get-PSTree .\src\ -Recurse -Directory

   Source: C:\path\to\PSTree\src

Mode            Length Hierarchy
----            ------ ---------
d----          0.00  B src
d----         10.30 KB └── PSTree
d----         16.53 KB     ├── obj
d----          0.00  B     │   └── Debug
d----         88.02 KB     │       └── netstandard2.0
d----          1.13 KB     ├── Internal
d----          5.68 KB     ├── Commands
d----          0.00  B     └── bin
d----          0.00  B         └── Debug
d----         33.31 KB             └── netstandard2.0
d----         33.11 KB                 └── publish
```

### Display subdirectories only 2 levels deep

```powershell
PS ..\PSTree> Get-PSTree .\src\ -Depth 2 -Directory

   Source: C:\path\to\PSTree\src

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

   Source: C:\path\to\PSTree\src

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

Contributions are more than welcome, if you wish to contribute, fork this repository and submit a pull request with the changes.
