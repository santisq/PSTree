<h1 align="center">PSTree</h1>

PowerShell function that intends to emulate the [`tree` command](https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/tree) with added functionality to calculate the __folders size__ as well as __recursive folders size__.

---

## INSTALLATION

```powershell
Install-Module PSTree -Scope CurrentUser
```

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

## REQUIREMENTS

__PowerShell v5.1__ or __PowerShell Core 7+__.

## EXAMPLES

### Get hierarchy of the current Directory with default parameters (`-Depth 3`)

```
PS ..\PSTree> Get-PSTree

Mode  Hierarchy                             Size
----  ---------                             ----
d---- PSTree                             9.52 Kb
-a--- ├── LICENSE                        1.07 Kb
-a--- ├── README.md                      8.45 Kb
d---- └── PSTree                         4.83 Kb
-a---     ├── PSTree.psd1                4.57 Kb
-a---     ├── PSTree.psm1              270 Bytes
d----     ├── public                     5.96 Kb
-a---     │   └── Get-PSTree.ps1         5.96 Kb
d----     ├── private                    0 Bytes
d----     │   └── classes                3.29 Kb
-a---     │       └── classes.ps1        3.29 Kb
d----     └── Format                     1.83 Kb
-a---         └── PSTree.Format.ps1xml   1.83 Kb
```

### Get hierarchy of the current Directory recursively displaying only Folders

```
PS ..\PSTree> Get-PSTree -Directory -Recurse

Mode  Hierarchy              Size
----  ---------              ----
d---- PSTree              9.52 Kb
d---- └── PSTree          4.83 Kb
d----     ├── public      5.72 Kb
d----     ├── private     0 Bytes
d----     │   └── classes 3.29 Kb
d----     └── Format      1.83 Kb
```

### Get hierarchy of the current Directory 2 levels deep and displaying hidden Folders

```
PS ..\PSTree> Get-PSTree -Depth 2 -Force

Mode  Hierarchy                   Size
----  ---------                   ----
d---- PSTree                   8.46 Kb
-a--- ├── LICENSE              1.07 Kb
-a--- ├── README.md             7.4 Kb
d---- ├── PSTree               4.83 Kb
-a--- │   ├── PSTree.psd1      4.57 Kb
-a--- │   ├── PSTree.psm1    270 Bytes
d---- │   ├── public           5.96 Kb
d---- │   ├── private          0 Bytes
d---- │   └── Format           1.83 Kb
d--h- └── .git                 1.69 Kb
-a---     ├── COMMIT_EDITMSG   2 Bytes
-a---     ├── config         296 Bytes
-a---     ├── description     73 Bytes
-a---     ├── FETCH_HEAD     198 Bytes
-a---     ├── HEAD            21 Bytes
-a---     ├── index          926 Bytes
-a---     ├── ORIG_HEAD       41 Bytes
-a---     ├── packed-refs    177 Bytes
d----     ├── refs             0 Bytes
d----     ├── objects          0 Bytes
d----     ├── logs              2.2 Kb
d----     ├── info           240 Bytes
d----     └── hooks           22.89 Kb
```

### Get hierarchy 2 levels deep displaying only Folders with their recursive size

```
PS ..\PSTree> Get-PSTree -Depth 2 -RecursiveSize -Directory

Mode  Hierarchy            Size
----  ---------            ----
d---- PSTree          181.76 Kb
d---- └── PSTree       15.91 Kb
d----     ├── public    5.96 Kb
d----     ├── private   3.29 Kb
d----     └── Format    1.83 Kb
```
