---
external help file: PSTree.dll-Help.xml
Module Name: PSTree
online version: https://github.com/santisq/PSTree/blob/main/docs/en-US/Get-PSTree.md
schema: 2.0.0
---

# Get-PSTree

## SYNOPSIS
\`tree\` like cmdlet for PowerShell.

## SYNTAX



## DESCRIPTION
\`Get-PSTree\` is a PowerShell cmdlet that intends to emulate the \`tree\` command with added functionality to calculate the folders size as well as recursive folders size.

## EXAMPLES

### Example 1: Get the current directory hierarchy with default parameter values
```
PS C:\> Get-PSTree
```

The default parameter set uses \`-Depth\` with a value of 3.
No hidden and system files folder are displayed and recursive folder size is not calculated.

### Example 2: Get the hierarchy of the `$HOME` directory recursively displaying only folders
```
PS C:\> Get-PSTree $HOME -Directory -Recurse
```

### Example 3: Recurse `$HOME` subdirectories 2 levels in depth displaying hidden files and folders
```
PS C:\> Get-PSTree -Depth 2 -Force
```

### Example 4: Recurse the `C:\` drive 2 levels in depth displaying only folders with their recursive size
```
PS C:\> Get-PSTree C:\ -Depth 2 -RecursiveSize -Directory
```

### Example 5: Get the `$HOME` directory hierarchy recursively excluding all `.jpg` and `.png` files
```
PS C:\> Get-PSTree $HOME -Recurse -Exclude *.jpg, *.png
```

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
Patterns are tested with the object's \`FullName\` property.

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

### -LiteralPath
Absolute or relative folder path.
Note that the value is used exactly as it's typed.
No characters are interpreted as wildcards.

```yaml
Type: String[]
Parameter Sets: LiteralPath
Aliases: PSPath

Required: False
Position: 0
Default value: $PWD
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Path
{{ Fill Path Description }}

```yaml
Type: String[]
Parameter Sets: Path
Aliases:

Required: False
Position: Named
Default value: None
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

By default, the size of hidden and system items is not added to the recursive size, for this you must use the \`-Force\` parameter.
Excluded items with the \`-Exclude\` parameter do not add to the recursive size.

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
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String
## OUTPUTS

### PSTreeDirectory, PSTreeFile
## NOTES

## RELATED LINKS