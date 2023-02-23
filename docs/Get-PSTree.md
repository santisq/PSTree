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

Controls the recursion limit

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

Traverse all Directory hierarchy

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

Displays hidden Files and Folders

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

Displays Folders only

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

Displays the recursive Folders Size

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