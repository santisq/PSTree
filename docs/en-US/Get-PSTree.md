---
external help file: PSTree.dll-Help.xml
Module Name: PSTree
online version: https://github.com/santisq/PSTree/blob/main/docs/en-US/Get-PSTree.md
schema: 2.0.0
---

# Get-PSTree

## SYNOPSIS

A PowerShell cmdlet that displays a hierarchical, tree-like view of folder contents, including folder size calculations, inspired by the classic `tree` command.

## SYNTAX

### Path (Default)

```powershell
Get-PSTree
    [[-Path] <String[]>]
    [-Depth <Int32>]
    [-Recurse]
    [-Force]
    [-Directory]
    [-RecursiveSize]
    [-Exclude <String[]>]
    [-Include <String[]>]
    [<CommonParameters>]
```

### LiteralPath

```powershell
Get-PSTree
    [-LiteralPath <String[]>]
    [-Depth <Int32>]
    [-Recurse]
    [-Force]
    [-Directory]
    [-RecursiveSize]
    [-Exclude <String[]>]
    [-Include <String[]>]
    [<CommonParameters>]
```

## DESCRIPTION

The `Get-PSTree` cmdlet offers a tree-style visualization of a folder's contents, drawing inspiration from the classic `tree` command. It displays the file system hierarchy, including directories and files, with enhanced functionality to calculate folder sizes—both individual and recursive—making it ideal for analyzing disk usage and organizing file structures. This cmdlet supports filtering by depth and enabling recursive traversal, enabling efficient exploration of file systems on any supported platform. Use it to identify large folders, manage directory layouts, or gain insights into file system organization.

## EXAMPLES

### Example 1: Get the current Directory Tree with Default Parameters

```powershell
PS ..\PSTree> Get-PSTree
```

This example retrieves the directory and file structure of the current directory, using the default parameters: __a depth of 3 levels__ and no recursive traversal beyond that limit. It displays both directories and files, including their sizes, but does not include hidden or system items unless explicitly requested through other means.

### Example 2: Display the `$HOME` Directory Tree Recursively, Showing Only Directories

```powershell
PS ..\PSTree> Get-PSTree $HOME -Directory -Recurse
```

This example retrieves the complete directory hierarchy under the `$HOME` path, traversing all subfolders recursively and displaying only directories (excluding files).

### Example 3: Display the `$HOME` Directory Tree, Limited to 2 Levels, Including Hidden Items

```powershell
PS ..\PSTree> Get-PSTree $HOME -Depth 2 -Force
```

> [!TIP]
> The `-Force` switch is required to include hidden files and folders in the output. Additionally, hidden items contribute to folder sizes (including recursive sizes with [`-RecursiveSize`](#-recursivesize), if specified), ensuring a comprehensive view of disk usage.

### Example 4: Display the `C:\` Drive Tree, Limited to 2 Levels, Showing Only Directories with Recursive Sizes

```powershell
PS ..\PSTree> Get-PSTree C:\ -Depth 2 -RecursiveSize -Directory
```

This example retrieves the directory structure under the `C:\` drive, limited to 2 levels of depth, displaying only directories (excluding files) and calculating their recursive sizes. It provides a controlled view for analyzing disk usage, summing the sizes of all files within each directory and its subdirectories.

### Example 5: Display the `$HOME` Directory Tree Recursively, Excluding `.jpg` and `.png` Files

```powershell
PS ..\PSTree> Get-PSTree $HOME -Recurse -Exclude *.jpg, *.png
```

> [!NOTE]
>
> - The `-Exclude` parameter supports [wildcard patterns](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_wildcards?view=powershell-7.3), and patterns are evaluated using the items’ `.Name` property.  
> - Excluded items, such as `.jpg` and `.png` files in this case, do not contribute to folder sizes, including recursive sizes calculated by [`-RecursiveSize`](#-recursivesize).

This example retrieves the complete directory and file hierarchy under the `$HOME` path, traversing all subfolders recursively, while excluding files matching the patterns `*.jpg` and `*.png`. It displays remaining directories and files with their sizes, useful for focusing on specific file types or managing disk space.

### Example 6: Display the Tree of All Directories in the Current Location

```powershell
PS ..\PSTree> Get-ChildItem -Directory | Get-PSTree
```

> [!TIP]
> Output from cmdlets that work with the file system and return `System.IO.FileSystemInfo` objects (e.g., `Get-ChildItem`, `Get-Item`) can be piped to `Get-PSTree`. Pipeline input is bound to the `-LiteralPath` parameter if the objects have a `PSPath` property, enabling seamless traversal of multiple directory structures with default settings (depth of 3, no recursion unless specified).

This example pipes the output of `Get-ChildItem` to `Get-PSTree`, retrieving the directory and file structure for each directory in the current location. It displays hierarchies with default depth (3 levels) and folder sizes, excluding hidden or system items unless [`-Force`](#-force) is used.

### Example 7: Display the Tree of All Directories in the Current Location, Including Only `.ps1` Files

```powershell
PS ..\PSTree> Get-ChildItem -Directory | Get-PSTree -Include *.ps1
```

> [!IMPORTANT]
>
> - Similar to `-Exclude`, the `-Include` parameter supports [wildcard patterns](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_wildcards?view=powershell-7.3), __but it applies only to files, not directories__.  
> - Only files matching the pattern (e.g., `*.ps1`) are included in the output, while directories are always shown to maintain the hierarchy.

This example pipes the output of `Get-ChildItem` to `Get-PSTree`, retrieving the directory structure for each directory in the current location, but including only `.ps1` files within those directories. It displays the hierarchy with default depth (3 levels) and folder sizes, useful for focusing on PowerShell scripts while preserving directory context.

## PARAMETERS

### -Depth

Specifies the maximum depth of the folder traversal. The default value is 3, limiting the hierarchy to three levels. Use this parameter to control the depth for performance or readability when exploring large directory structures.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 3
Accept pipeline input: False
Accept wildcard characters: False
```

### -Directory

Limits output to directories only, excluding files. When specified, the cmdlet displays only `TreeDirectory` objects, omitting `TreeFile` objects, for a focused view of folder hierarchies.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -Exclude

Specifies an array of one or more string patterns to exclude items from the output as the cmdlet traverses the folder structure. Matching items are omitted from the results. [Wildcard characters](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_wildcards?view=powershell-7.5) are accepted.  

> [!NOTE]
>
> - Patterns are evaluated against the `.Name` property of each item.  
> - The `-Include` and `-Exclude` parameters can be used together, but exclusions are applied before inclusions.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: True
```

### -Force

Allows the cmdlet to access items that would otherwise be inaccessible, such as hidden or system files and folders. When specified, hidden and system items are included in the output and contribute to recursive folder sizes calculated by [`-RecursiveSize`](#-recursivesize).

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -Include

Specifies an array of one or more string patterns to include only matching items in the output as the cmdlet traverses the folder structure. Matching items are included, while others are omitted. [Wildcard characters](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_wildcards?view=powershell-7.5) are accepted.  

> [!NOTE]
>
> - __This parameter works only on files, not directories.__
> - Patterns are evaluated against the `.Name` property of each item.
> - The `-Include` and `-Exclude` parameters can be used together, but exclusions are applied before inclusions.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: True
```

### -LiteralPath

Specifies the literal path to the folder to traverse, without interpreting wildcard characters. This parameter accepts an array of paths and can receive input from the pipeline by property name (using the `PSPath` alias). Use this for exact path matching, bypassing wildcard expansion.

```yaml
Type: String[]
Parameter Sets: LiteralPath
Aliases: PSPath

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Path

Specifies the path to one or more folders to traverse. Accepts an array of paths, which can include [wildcard characters](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_wildcards?view=powershell-7.5). The default location is the current directory (`$PWD`) if no path is provided.

```yaml
Type: String[]
Parameter Sets: Path
Aliases:

Required: False
Position: 0
Default value: $PWD
Accept pipeline input: True (ByValue)
Accept wildcard characters: True
```

### -Recurse

Enables recursive traversal of all subfolders under the specified path. Use this switch to explore the complete folder hierarchy, but note that it may impact performance on large directories—consider ][`-Depth`](#-depth) for smaller subsets.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -RecursiveSize

Enables the cmdlet to calculate the recursive size of folders across the entire hierarchy, summing the sizes of all files within each directory and its subdirectories. By default, the cmdlet calculates only the size of files directly within each directory. This is a more resource-intensive operation, as it requires traversing all items in the hierarchy.  

By default, hidden and system items are excluded from recursive size calculations unless [`-Force`](#-force) is specified. Items excluded by the [`-Exclude`](#-exclude) parameter do not contribute to recursive sizes.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String

You can pipe a strings containing file system paths to this cmdlet.  
Output from cmdlets that return `System.IO.FileSystemInfo` objects (e.g., `Get-Item`, `Get-ChildItem`) can be piped to this cmdlet, with pipeline input bound to `-LiteralPath` if the objects have a `PSPath` property.

## OUTPUTS

### PSTree.TreeDirectory

Returns objects of type `TreeDirectory` representing directories in a hierarchical structure, with `TreeFile` objects included as children. Each `TreeDirectory` object includes properties such as `Name`, `FullName`, `Size`, and `Depth`, reflecting the file system organization and enabling size-based analysis. This prioritizes directories as the primary output type for tree navigation, with files nested within for comprehensive exploration.

### PSTree.TreeFile

Returns objects of type `TreeFile` representing files within directories, included in the output unless the `-Directory` parameter is specified. Each object includes properties such as `Name`, `FullName`, `Size`, and `Depth`, allowing detailed file analysis within the hierarchical structure. These objects are nested under `TreeDirectory` objects, supporting disk usage tracking and file system navigation.

## NOTES

This cmdlet requires PowerShell 5.1 or later and supports cross-platform file system exploration on Windows, Linux, and macOS. It may require elevated permissions for certain system folders. Use `-Recurse` or `-RecursiveSize` cautiously with large directories to avoid performance issues, and refer to the [`about_TreeStyle`](./about_TreeStyle.md) help topic for customizing the output format. For Windows-specific registry navigation, see the [`Get-PSTreeRegistry`](./Get-PSTreeRegistry.md) cmdlet

## RELATED LINKS

[__tree Command__](https://learn.microsoft.com/en-us/windows-server/administration/windows-commands/tree)

[__`about_TreeStyle`__](./about_TreeStyle.md)

[__`Get-PSTree` Source__](../../src/PSTree/Commands/GetPSTreeCommand.cs)

[__`Get-PSTree` Tests__](../../tests/GetPSTreeCommand.tests.ps1)
