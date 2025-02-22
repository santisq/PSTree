---
external help file: PSTree.dll-Help.xml
Module Name: PSTree
online version: https://github.com/santisq/PSTree/blob/main/docs/en-US/Get-PSTreeRegistry.md
schema: 2.0.0
---

# Get-PSTreeRegistry

## SYNOPSIS

Retrieves registry keys and values from the Windows Registry in a hierarchical, tree-like format, allowing exploration of registry structures.

## SYNTAX

### Path (Default)

```powershell
Get-PSTreeRegistry
    [[-Path] <String[]>]
    [-Depth <Int32>]
    [-Recurse]
    [-KeysOnly]
    [<CommonParameters>]
```

### LiteralPath

```powershell
Get-PSTreeRegistry
    [-LiteralPath <String[]>]
    [-Depth <Int32>]
    [-Recurse]
    [-KeysOnly]
    [<CommonParameters>]
```

## DESCRIPTION

The `Get-PSTreeRegistry` cmdlet provides a tree-style view of the Windows Registry, displaying registry keys and their associated values in a hierarchical format similar to the `tree` command for file systems. This Windows-only cmdlet is designed for navigating and analyzing registry configurations, making it easier to inspect, troubleshoot, or document registry structures.

## EXAMPLES

### Example 1

```powershell
PS C:\> {{ Add example code here }}
```

{{ Add example description here }}

## PARAMETERS

### -Depth

Specifies the maximum depth of the registry traversal.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -KeysOnly

Limits output to `TreeRegistryKey` objects only, excluding `TreeRegistryValue` objects. When specified, the cmdlet displays only registry keys and their subkeys, omitting any values associated with those keys.

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

Specifies the literal registry paths to traverse, without wildcard expansion. This parameter is mandatory and accepts input from the pipeline by property name (using the `PSPath` alias). Use this for exact path matching, bypassing wildcard interpretation.  

> [!TIP]
> For registry base keys not mapped as PowerShell drives (e.g., `HKCU:\` and `HKLM:\`), you can use the provider path format by prefixing the path with `Registry::`. For example, to traverse all keys under `HKEY_USERS` exactly as specified, use: `Get-PSTreeRegistry -LiteralPath Registry::HKEY_USERS`.

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

Specifies the registry paths to traverse. Accepts one or more registry paths (e.g., `HKLM:\Software`, `HKCU:\Software`), which can include wildcards. This parameter is mandatory and can accept input from the pipeline. Paths are resolved using PowerShell's registry provider.  

> [!TIP]
> For registry base keys not mapped as PowerShell drives (e.g., `HKCU:\` and `HKLM:\`), you can use the provider path format by prefixing the path with `Registry::`. For example, to traverse each key under `HKEY_USERS`, use: `Get-PSTreeRegistry -Path Registry::HKEY_USERS\*`.

```yaml
Type: String[]
Parameter Sets: Path
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: True
```

### -Recurse

{{ Fill Recurse Description }}

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

This cmdlet supports the common parameters. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### String

## OUTPUTS

### TreeRegistryKey

### TreeRegistryValue

## NOTES

## RELATED LINKS
