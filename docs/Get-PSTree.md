## SYNTAX

### Depth (Default)

```powershell
Get-PSTree [[-LiteralPath] <String>] [[-Depth] <Int32>] [-Force] [-Directory] [-RecursiveSize] [<CommonParameters>]
```

### Recurse

```powershell
Get-PSTree [[-LiteralPath] <String>] [-Recurse] [-Force] [-Directory] [-RecursiveSize] [<CommonParameters>]
```

## PARAMETERS

### -LiteralPath

Absolute or relative folder path.

```yaml
Type: String
Parameter Sets: (All)
Aliases: FullName

Required: False
Position: 1
Default value: $PWD
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Depth

Determines the number of subdirectory levels that are included in the recursion.

```yaml
Type: Int32
Parameter Sets: Depth
Aliases:

Required: False
Position: 2
Default value: 3
Accept pipeline input: False
Accept wildcard characters: False
```

### -Recurse

Gets the items in the specified locations and in all child items of the locations.

```yaml
Type: SwitchParameter
Parameter Sets: Recurse
Aliases:

Required: False
Position: 2
Default value: False
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
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -Directory

Displays Directories only.

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

Displays the recursive Size of Folders.

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

This cmdlet supports the common parameters. See [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).