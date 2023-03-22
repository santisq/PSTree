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

### Depth (Default)

```powershell
Get-PSTree [[-LiteralPath] <String>] [[-Depth] <Int32>] [-Force] [-Directory] [-RecursiveSize] [<CommonParameters>]
```

### Recurse

```powershell
Get-PSTree [[-LiteralPath] <String>] [-Recurse] [-Force] [-Directory] [-RecursiveSize] [<CommonParameters>]
```

## DESCRIPTION

`Get-PSTree` is a PowerShell cmdlet that intends to emulate the `tree` command with added functionality to calculate the __folders size__ as well as __recursive folders size__.

## EXAMPLES

### Example 1: Get the hierarchy of the current Directory with default parameter values

```powershell
PS C:\> Get-PSTree
```

### Example 2: Get the hierarchy of the `$HOME` Directory recursively displaying only folders

```powershell
PS C:\> Get-PSTree $HOME -Directory -Recurse
```

### Example 3: Get hierarchy of the `$HOME` Directory 2 levels deep displaying hidden files and folders

```powershell
PS C:\> Get-PSTree -Depth 2 -Force
```

### Example 4: Get the hierarchy of the `C:\` drive 1 level deep displaying only folders with their recursive size

```powershell
PS C:\> Get-PSTree C:\ -Depth 2 -RecursiveSize -Directory
```

## PARAMETERS

### -Depth

Determines the number of subdirectory levels that are included in the recursion.

```yaml
Type: Int32
Parameter Sets: Depth
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
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Force

Gets items that otherwise can't be accessed by the user, such as hidden or system files.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -LiteralPath

Absolute or relative folder path. Note that the value is used exactly as it's typed. No characters are interpreted as wildcards.

```yaml
Type: String
Parameter Sets: (All)
Aliases: PSPath

Required: False
Position: 0
Default value: $PWD
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Recurse

Gets the items in the specified locations and in all child items of the locations.

```yaml
Type: SwitchParameter
Parameter Sets: Recurse
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -RecursiveSize

This switch enables the cmdlet to calculated the recursive size of folders in a hierarchy. By default, the cmdlet only displays the size of folders based on the sum of the files length in each Directory. It's important to note that this is a more expensive operation, in order to calculate the recursive size, all folders in the hierarchy need to be traversed.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters. See [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String

## OUTPUTS

### PSTree.PSTreeDirectory

### PSTree.PSTreeFile