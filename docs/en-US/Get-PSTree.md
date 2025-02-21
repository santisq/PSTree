---
external help file: PSTree.dll-Help.xml
Module Name: PSTree
online version: https://github.com/santisq/PSTree/blob/main/docs/en-US/Get-PSTree.md
schema: 2.0.0
---

# Get-PSTree

## SYNOPSIS

`tree` like cmdlet for PowerShell.

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

`Get-PSTree` is a PowerShell cmdlet that intends to emulate the `tree` command with added functionalities to calculate the folders size as well as recursive folders size.

## EXAMPLES

### Example 1: Get the current directory tree with default parameters values

```powershell
PS ..\PSTree> Get-PSTree
```

The default parameter set uses `-Depth` with a value of 3. No hidden and system files folder are displayed and recursive folder size is not calculated.

### Example 2: Get the `$HOME` tree recursively displaying only folders

```powershell
PS ..\PSTree> Get-PSTree $HOME -Directory -Recurse
```

In this example `$HOME` is bound positionally to the `-Path` parameter.

### Example 3: Get the `$HOME` tree 2 levels deep displaying hidden files and folders

```powershell
PS ..\PSTree> Get-PSTree -Depth 2 -Force
```

> [!TIP]
> The `-Force` switch is needed to display hidden files and folders. In addition, hidden child items do not add up to the folders size without this switch.

### Example 4: Get the `C:\` drive tree 2 levels in depth displaying only folders calculating the recursive size

```powershell
PS ..\PSTree> Get-PSTree C:\ -Depth 2 -RecursiveSize -Directory
```

### Example 5: Get the `$HOME` tree recursively excluding all `.jpg` and `.png` files

```powershell
PS ..\PSTree> Get-PSTree $HOME -Recurse -Exclude *.jpg, *.png
```

> [!NOTE]
>
> - The `-Exclude` parameter supports [wildcard patterns](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_wildcards?view=powershell-7.3), exclusion patterns are evaluated using the items `.Name` property.
> - __Excluded items do not do not add to the folders size.__

### Example 6: Get the tree of all folders in a location

```powershell
PS ..\PSTree> Get-ChildItem -Directory | Get-PSTree
```

> [!TIP]
> Output from `Get-ChildItem` can be piped to this cmdlet. Pipeline input is bound to `-LiteralPath` parameter if the items have a `PSPath` property.

### Example 7: Get the tree of all folders in a location including only `*.ps1` files

```powershell
PS ..\PSTree> Get-ChildItem -Directory | Get-PSTree -Include *.ps1
```

> [!IMPORTANT]
> Similar to `-Exclude`, the `-Include` parameter supports [wildcard patterns](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_wildcards?view=powershell-7.3), however, __this parameter works only with Files__.

## PARAMETERS

### -Depth

Determines the number of subdirectory levels that are included in the recursion.

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

Use this switch to display Directories only.

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

Specifies an array of one or more string patterns to be matched as the cmdlet gets child items.
Any matching item is excluded from the output.
Wildcard characters are accepted.

Excluded items do not add to the recursive folders size.

> [!NOTE]
>
> - Patterns are evaluated using the object's `.Name` property.
> - The `-Include` and `-Exclude` parameters can be used together, however the exclusions are applied before the inclusions.

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

Gets items that otherwise can't be accessed by the user, such as hidden or system files.

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

Specifies an array of one or more string patterns to be matched as the cmdlet gets child items.
Any matching item is included in the output.
Wildcard characters are accepted.

> [!NOTE]
>
> - __This parameter works only on files.__
> - Patterns are evaluated using the object's `.Name` property.
> - The `-Include` and `-Exclude` parameters can be used together, however the exclusions are applied before the inclusions.

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

Absolute or relative folder path.
Note that the value is used exactly as it's typed.
No characters are interpreted as wildcards.

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

Specifies a path to one or more locations. Wildcards are accepted.
The default location is the current directory (`$PWD`).

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

Gets the items in the specified location and in all child items of the location.

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

This switch enables the cmdlet to calculate the recursive size of folders in a hierarchy.
By default, the cmdlet only displays the size of folders based on the sum of the file's Length in each directory.
It's important to note that this is a more expensive operation, in order to calculate the recursive size, all items in the hierarchy needs to be traversed.

By default, the size of hidden and system items is not added to the recursive size, for this you must use the `-Force` parameter.
Excluded items with the `-Exclude` parameter do not add to the recursive size.

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

### String

You can pipe a string that contains a path to this cmdlet. Output from `Get-Item` and `Get-ChildItem` can be piped to this cmdlet.

## OUTPUTS

### TreeDirectory

### TreeFile
