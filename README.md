<h1 align="center">PSTree</h1>

<div align="center">
   <sub>

   `tree` like cmdlets for PowerShell!
   </sub>
<br/><br/>

[![build](https://github.com/santisq/PSTree/actions/workflows/ci.yml/badge.svg)](https://github.com/santisq/PSTree/actions/workflows/ci.yml)
[![codecov](https://codecov.io/gh/santisq/PSTree/branch/main/graph/badge.svg?token=b51IOhpLfQ)](https://codecov.io/gh/santisq/PSTree)
[![PowerShell Gallery](https://img.shields.io/powershellgallery/dt/PSTree?color=%23008FC7)](https://www.powershellgallery.com/packages/PSTree)
[![LICENSE](https://img.shields.io/github/license/santisq/PSTree)](https://github.com/santisq/PSTree/blob/main/LICENSE)

</div>

PSTree is a PowerShell module that extends tree-style navigation to both file systems and the Windows Registry through two versatile cmdlets. Designed for administrators, developers, and power users, it combines hierarchical visualization with practical insights like folder sizes and registry traversal.

## Cmdlets

- **`Get-PSTree`**  
  Inspired by the classic [`tree` command](https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/tree), this cmdlet displays your file system in a structured hierarchy. It goes further by calculating folder sizes—both individual and recursive—making it a powerful tool for disk usage analysis.

- **`Get-PSTreeRegistry`** *(Windows only)*  
  Explore the Windows Registry with a tree-like view of keys and values. This cmdlet simplifies navigation and troubleshooting by presenting registry structures in an intuitive format, ideal for system configuration tasks.

## Documentation

- Learn how to use the cmdlets in the [official documentation](./docs/en-US/).

- To Customize output rendering, see [about_TreeStyle](./docs/en-US/about_TreeStyle.md).

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

- **Windows PowerShell v5.1** or [**PowerShell 7+**](https://github.com/PowerShell/PowerShell).
- **Windows OS** (for `Get-PSTreeRegistry`).

## Usage

### `Get-PSTree`

#### Get the current directory tree with default parameters values

```powershell
PS ..\PSTree> Get-PSTree | Select-Object -First 20

   Source: C:\User\Documents\PSTree

Mode            Length Hierarchy
----            ------ ---------
d----         36.78 KB PSTree
-a---          4.75 KB ├── .gitignore        
-a---        137.00  B ├── .markdownlint.json
-a---          1.37 KB ├── build.ps1
-a---         19.34 KB ├── CHANGELOG.md
-a---          1.07 KB ├── LICENSE
-a---         10.12 KB ├── README.md
d----          0.00  B ├── .github
d----          4.10 KB │   └── workflows
-a---          4.10 KB │       └── ci.yml
d----          4.18 KB ├── .vscode
-a---        275.00  B │   ├── extensions.json
-a---          1.39 KB │   ├── launch.json
-a---          1.09 KB │   ├── settings.json
-a---          1.43 KB │   └── tasks.json
d----        266.71 KB ├── assets
-a---         10.00 KB │   ├── EscapeSequence.png
-a---         78.08 KB │   ├── Get-PSTree.After.png
-a---         73.89 KB │   ├── Get-PSTree.Before.png
-a---         34.54 KB │   ├── Get-PSTreeRegistry.After.png
```

#### Excludes items starting with `.g`, `.v`, `.m` and `assets`

```powershell
PS ..\PSTree> Get-PSTree -Exclude .[gvm]*, assets | Select-Object -First 20

   Source: C:\User\Documents\PSTree

Mode            Length Hierarchy
----            ------ ---------
d----         31.90 KB PSTree
-a---          1.37 KB ├── build.ps1   
-a---         19.34 KB ├── CHANGELOG.md
-a---          1.07 KB ├── LICENSE
-a---         10.12 KB ├── README.md
d----          0.00  B ├── docs
d----         31.99 KB │   └── en-US
-a---          7.25 KB │       ├── about_TreeStyle.md
-a---         13.25 KB │       ├── Get-PSTree.md
-a---         10.49 KB │       ├── Get-PSTreeRegistry.md
-a---          1.00 KB │       └── Get-PSTreeStyle.md
d----         22.10 KB ├── module
-a---         17.08 KB │   ├── PSTree.Format.ps1xml
-a---          5.01 KB │   └── PSTree.psd1
d----        513.42 KB ├── output
-a---         22.70 KB │   ├── PSTree.2.1.11.nupkg
-a---         24.21 KB │   ├── PSTree.2.1.12.nupkg
-a---         24.22 KB │   ├── PSTree.2.1.13.nupkg
-a---         24.42 KB │   ├── PSTree.2.1.14.nupkg
-a---         25.07 KB │   ├── PSTree.2.1.15.nupkg
```

#### Includes `.ps1` and `.cs` files and excludes `tools` folder

```powershell
PS ..\PSTree> Get-PStree -Include *.ps1, *.cs -Exclude tools

   Source: C:\User\Documents\PSTree

Mode            Length Hierarchy
----            ------ ---------
d----          1.37 KB PSTree
-a---          1.37 KB ├── build.ps1
d----          0.00  B ├── src
d----         15.87 KB │   └── PSTree
-a---        839.00  B │       ├── Cache.cs
-a---        169.00  B │       ├── ITree.cs
-a---        674.00  B │       ├── RegistryMappings.cs
-a---          1.02 KB │       ├── TreeBase.cs
-a---          4.00 KB │       ├── TreeCommandBase.cs
-a---        438.00  B │       ├── TreeComparer.cs
-a---          2.27 KB │       ├── TreeDirectory.cs
-a---          1.30 KB │       ├── TreeFile.cs
-a---          1.68 KB │       ├── TreeFileSystemInfo_T.cs
-a---        311.00  B │       ├── TreeFileSystemInfo.cs
-a---        942.00  B │       ├── TreeRegistryBase.cs
-a---          1.10 KB │       ├── TreeRegistryKey.cs
-a---          1.21 KB │       └── TreeRegistryValue.cs
d----         27.48 KB └── tests
-a---        985.00  B     ├── FormattingInternals.tests.ps1
-a---          6.13 KB     ├── GetPSTreeCommand.tests.ps1
-a---          6.44 KB     ├── GetPSTreeRegistryCommand.tests.ps1
-a---          1.77 KB     ├── TreeDirectory.tests.ps1
-a---        914.00  B     ├── TreeFile.tests.ps1
-a---          2.62 KB     ├── TreeFileSystemInfo_T.tests.ps1
-a---          8.67 KB     └── TreeStyle.tests.ps1
```

#### Get the recursive size of the folders

```powershell
PS ..\PSTree> Get-PSTree .\src\ -Depth 2 -Directory -RecursiveSize

   Source: C:\User\Documents\PSTree\src

Mode            Length Hierarchy
----            ------ ---------
d----          1.04 MB src
d----          1.04 MB └── PSTree
d----        670.05 KB     ├── bin
d----          7.53 KB     ├── CodeAnalysis
d----         10.58 KB     ├── Commands
d----          8.01 KB     ├── Extensions
d----          1.29 KB     ├── Internal
d----        341.03 KB     ├── obj
d----         11.69 KB     └── Style
```

### `Get-PSTreeRegistry`

#### Get the tree-view of `HKCU:\System`

```powershell
PS ..\PSTree> Get-PSTreeRegistry HKCU:\System -Depth 2

   Hive: HKEY_CURRENT_USER\System

Kind         Hierarchy
----         ---------
RegistryKey  System
RegistryKey  ├── GameConfigStore
DWord        │   ├── GameDVR_Enabled
DWord        │   ├── GameDVR_FSEBehaviorMode
Binary       │   ├── Win32_AutoGameModeDefaultProfile
Binary       │   ├── Win32_GameModeRelatedProcesses
DWord        │   ├── GameDVR_HonorUserFSEBehaviorMode
DWord        │   ├── GameDVR_DXGIHonorFSEWindowsCompatible
DWord        │   ├── GameDVR_EFSEFeatureFlags
RegistryKey  │   ├── Parents
RegistryKey  │   └── Children
RegistryKey  └── CurrentControlSet
RegistryKey      ├── Policies
RegistryKey      └── Control
```

#### Filter Out Specific Items from the `HKCU:\System` Tree

```powershell
PS ..\PSTree> Get-PSTreeRegistry HKCU:\System -Depth 2 -Exclude CurrentControlSet, GameDV*

   Hive: HKEY_CURRENT_USER\System

Kind         Hierarchy
----         ---------
RegistryKey  System
RegistryKey  └── GameConfigStore
Binary           ├── Win32_AutoGameModeDefaultProfile
Binary           ├── Win32_GameModeRelatedProcesses
RegistryKey      ├── Parents
RegistryKey      └── Children
```

#### Select GameDVR-Related Values in the `HKCU:\System` Tree

```powershell
PS ..\PSTree> Get-PSTreeRegistry HKCU:\System -Depth 2 -Include GameDVR*

   Hive: HKEY_CURRENT_USER\System

Kind         Hierarchy
----         ---------
RegistryKey  System
RegistryKey  └── GameConfigStore
DWord            ├── GameDVR_Enabled
DWord            ├── GameDVR_FSEBehaviorMode
DWord            ├── GameDVR_HonorUserFSEBehaviorMode
DWord            ├── GameDVR_DXGIHonorFSEWindowsCompatible
DWord            └── GameDVR_EFSEFeatureFlags
```

#### Show only Keys

```powershell
PS ..\PSTree> Get-PSTreeRegistry HKCU:\System -Depth 2 -KeysOnly

   Hive: HKEY_CURRENT_USER\System

Kind         Hierarchy
----         ---------
RegistryKey  System
RegistryKey  ├── GameConfigStore
RegistryKey  │   ├── Parents
RegistryKey  │   └── Children
RegistryKey  └── CurrentControlSet
RegistryKey      ├── Policies
RegistryKey      └── Control
```

#### Get the value of a `TreeRegistryValue` item

```powershell
PS ..\PSTree> $items = Get-PSTreeRegistry HKCU:\Environment\ -Depth 2
PS ..\PSTree> $values = $items | Where-Object { $_ -is [PSTree.TreeRegistryValue] }
PS ..\PSTree> $values

   Hive: HKEY_CURRENT_USER\Environment

Kind         Hierarchy
----         ---------
ExpandString ├── Path
ExpandString ├── TEMP
ExpandString └── TMP

PS ..\PSTree> $values[1].GetValue()
C:\Users\User\AppData\Local\Temp
```

## Changelog

- [CHANGELOG.md](CHANGELOG.md)
- [Releases](https://github.com/santisq/PSTree/releases)

## Contributing

Contributions are welcome, if you wish to contribute, fork this repository and submit a pull request with the changes.
