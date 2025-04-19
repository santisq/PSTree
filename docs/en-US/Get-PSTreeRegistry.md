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

### Example 1: Display the `Software` registry hive with full recursion

```powershell
PS ..\PSTree> Get-PSTreeRegistry HKLM:\Software -Recurse
```

This example retrieves all keys under `HKLM:\Software`, showing the complete hierarchy recursively.

### Example 2: List only registry keys under `HKLM:\Software`, up to 2 levels deep, excluding values

```powershell
PS ..\PSTree> Get-PSTreeRegistry HKLM:\Software -Depth 2 -KeysOnly
```

This example restricts output to keys only (no values) and limits the depth to 2 levels for better readability.

### Example 3: Retrieve and Display a Value from a `TreeRegistryValue` Object

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

This example demonstrates how to use `Get-PSTreeRegistry` to retrieve registry values, filter for `TreeRegistryValue` objects, and access a specific value using the `.GetValue()` method. It targets the `HKCU:\Environment` hive, limiting depth to 2 levels, and shows how to extract a value like `TEMP`.  

### Example 4: Traverse `HKEY_USERS` Using the Provider Path

```powershell
PS ..\PSTree> Get-PSTreeRegistry -Path Registry::HKEY_USERS -Depth 1 -EA 0

   Hive: HKEY_USERS

Kind         Hierarchy
----         ---------
RegistryKey  HKEY_USERS
RegistryKey  ├── S-1-5-18
RegistryKey  ├── S-1-5-21-3616279808-3400134814-4233402850-1002_Classes
RegistryKey  ├── S-1-5-21-3616279808-3400134814-4233402850-1002
RegistryKey  └── .DEFAULT
```

This example uses the registry provider path to explore all keys under `HKEY_USERS`, limited to 1 level deep.  

### Example 5: Filter Out Microsoft and Log-Related Items in `HKCU:\SOFTWARE` Tree

```powershell
PS ..\PSTree> Get-PSTreeRegistry HKCU:\SOFTWARE\ -Exclude Microsoft, *Log*
```

Excludes registry keys named "Microsoft" (e.g., `HKCU:\SOFTWARE\Microsoft`) and any keys or values with "Log" in their name (e.g., `HKCU:\SOFTWARE\UpdateLog` or a value named `ErrorLog`).

### Example 6: Select Windows-Related Registry Values in `HKCU:\SOFTWARE` Tree

```powershell
PS ..\PSTree> Get-PSTreeRegistry HKCU:\SOFTWARE\ -Include Win*
```

Includes only registry values whose names start with "Win" (e.g., `WindowsVersion=10.0`) from the `HKCU:\SOFTWARE` tree, omitting all keys and non-matching values.

## PARAMETERS

### -Depth

Specifies the maximum depth of the registry traversal. Default value is 3. Use this parameter to control the depth for performance or readability when exploring large registry hives.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases: p, dp

Required: False
Position: Named
Default value: 3
Accept pipeline input: False
Accept wildcard characters: False
```

### -KeysOnly

Limits output to `TreeRegistryKey` objects only, excluding `TreeRegistryValue` objects. When specified, the cmdlet displays only registry keys and their subkeys, omitting any values associated with those keys.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: k, key

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -LiteralPath

Specifies the literal registry paths to traverse, without wildcard expansion. This parameter accepts input from the pipeline by property name (using the `PSPath` alias). Use this for exact path matching, bypassing wildcard interpretation.  

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

Specifies the registry paths to traverse. Accepts one or more registry paths (e.g., `HKLM:\Software`, `HKCU:\Software`), which can include [wildcard characters](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_wildcards?view=powershell-7.5). This parameter can accept input from the pipeline. Paths are resolved using [PowerShell's registry provider](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_registry_provider?view=powershell-7.5).  

> [!TIP]
> For registry base keys not mapped as PowerShell drives (e.g., `HKCU:\` and `HKLM:\`), you can use the provider path format by prefixing the path with `Registry::`. For example, to traverse each key under `HKEY_USERS`, use: `Get-PSTreeRegistry -Path Registry::HKEY_USERS\*`.

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

Enables recursive traversal of all subkeys under the specified registry path. Use this switch to ensure complete hierarchy exploration without depth restrictions.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: r, rec

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Exclude

Specifies an array of one or more string patterns to exclude registry keys or values from the output as the cmdlet traverses the registry. Matching items are omitted from the results. [Wildcard characters](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_wildcards?view=powershell-7.5) are accepted.  

> [!NOTE]
>
> - For registry keys, patterns are evaluated against the leaf name of the registry path (e.g., `KeyName` in `HKLM:\Software\KeyName`).
> - For registry values, patterns are evaluated against the value name (e.g., `ValueName` in a key’s `ValueName=Data` pair).
> The `-Include` and `-Exclude` parameters can be used together, but exclusions are applied before inclusions.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases: exc

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: True
```

### -Include

Specifies an array of one or more string patterns to include only matching registry values in the output as the cmdlet traverses the registry. Matching registry values are included, while others (including all keys) are omitted. [Wildcard characters](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_wildcards?view=powershell-7.5) are accepted.  

> [!NOTE]
>
> - __This parameter works only on registry values, not keys.__
> - Patterns are evaluated against the value name (e.g., `ValueName` in a key’s `ValueName=Data` pair).
> - The `-Include` and `-Exclude` parameters can be used together, but exclusions are applied before inclusions.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases: inc

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: True
```

### CommonParameters

This cmdlet supports the common parameters. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String

You can pipe strings containing registry paths to this cmdlet.  
Output from cmdlets that return `Microsoft.Win32.RegistryKey` objects (e.g., `Get-Item`, `Get-ChildItem`) can be piped to this cmdlet, with pipeline input bound to `-LiteralPath` if the objects have a `PSPath` property.

## OUTPUTS

### PSTree.TreeRegistryKey

Returns objects of type `TreeRegistryKey` representing registry keys in a hierarchical structure. If `-KeysOnly` is specified, only `TreeRegistryKey` objects are returned; otherwise, `TreeRegistryValue` objects may also be included, but the primary output type remains `TreeRegistryKey` for consistency with the tree-like organization.

### PSTree.TreeRegistryValue

Returns objects of type `TreeRegistryValue` representing registry values associated with keys, included in the output unless the `-KeysOnly` parameter is specified. Each object includes properties such as `Name`, `Kind`, and `Depth`, allowing access to value data (e.g., via the `.GetValue()` method). These objects are nested under `TreeRegistryKey` objects in the hierarchical output, providing detailed value information for registry exploration.

## NOTES

This cmdlet is Windows-only and requires PowerShell 5.1 or later. It may require elevated permissions for certain registry hives. For file system navigation, see the [`Get-PSTree`](./Get-PSTree.md) cmdlet.

## RELATED LINKS

[__Microsoft.Win32 Namespace__](https://learn.microsoft.com/en-us/dotnet/api/microsoft.win32?view=net-9.0)

[__about_Registry_Provider__](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_registry_provider?view=powershell-7.5)

[__`Get-PSTreeRegistry` Source__](../../src/PSTree/Commands/GetPSTreeRegistryCommand.cs)

[__`Get-PSTreeRegistry` Tests__](../../tests/GetPSTreeRegistryCommand.tests.ps1)
