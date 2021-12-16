# PSTree

### DESCRIPTION
Cmdlet designed to emulate the [`tree`](https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/tree) command and also calculate the `Size` of each folder based of the files size of that folder (non-recursive).


### PARAMETER

| Parameter Name | Description
| --- | --- |
| `-Path <string>` | Absolute or relative folder path. Alias: `FullName`, `PSPath` |
| `[-Depth <int>]` | Specifies the maximum level of recursion |
| `[-Deep <switch>]` | Recursion until maximum level is reached |
| `[-Force <switch>]` | Display hidden and system folders |
| `[<CommonParameters>]` | See [`about_CommonParameters`](https://go.microsoft.com/fwlink/?LinkID=113216) |

### OUTPUTS `Object[]`

```
Name           TypeNameOfValue
----           ---------------
Nesting        System.Int32
Hierarchy      System.String
Size           System.String
RawSize        System.Double
FullName       System.String
Parent         System.IO.DirectoryInfo
CreationTime   System.DateTime
LastAccessTime System.DateTime
LastWriteTime  System.DateTime
```

### COMPATIBILITY
- Tested and compatible with __PowerShell v5.1__ and __PowerShell Core__.


### EXAMPLES

- __`Get-PSTree .`__ Gets the hierarchy and folder size of the current directory using __default Depth (3)__.
- __`Get-PSTree C:\users\user -Depth 10 -Force`__ Gets the hierarchy and folder size, including hidden ones, of the `user` directory  with a maximum of __10__ levels of recursion.
- __`Get-PSTree /home/user -Deep`:__ Gets the hierarchy and folder size of the `user` directory and all folders below.

---

```
PS /etc> $hierarchy = gpstree . -ErrorAction SilentlyContinue -Depth 5
PS /etc> $hierarchy | Select -First 20                                

Hierarchy                              Size
---------                              ----
etc                                    349.83 KB
├── acpi                               1.83 KB
│   └── events                         1.44 KB
├── alternatives                       5.08 KB
├── apache2                            0 B
│   ├── conf-available                 127 B
│   └── mods-available                 156 B
├── apm                                0 B
│   ├── event.d                        2.95 KB
│   ├── resume.d                       17 B
│   ├── scripts.d                      228 B
│   └── suspend.d                      17 B
├── apparmor                           3.36 KB
│   └── init                           0 B
│       └── network-interface-security 33 B
├── apparmor.d                         25.92 KB
│   ├── abstractions                   91.12 KB
│   │   ├── apparmor_api               2.41 KB
│   │   └── ubuntu-browsers.d          10.42 KB
│   ├── cache                          1.08 MB

PS /etc> $hierarchy[0] | Get-Member -MemberType NoteProperty, MemberSet

   TypeName: System.Management.Automation.PSCustomObject

Name              MemberType   Definition
----              ----------   ----------
PSStandardMembers MemberSet    PSStandardMembers {DefaultDisplayPropertySet}
CreationTime      NoteProperty datetime CreationTime=12/14/2021 6:16:05 PM
FullName          NoteProperty string FullName=/etc
Hierarchy         NoteProperty string Hierarchy=etc
LastAccessTime    NoteProperty datetime LastAccessTime=12/16/2021 12:40:41 AM
LastWriteTime     NoteProperty datetime LastWriteTime=12/14/2021 6:16:05 PM
Nesting           NoteProperty int Nesting=0
Parent            NoteProperty DirectoryInfo Parent=/
RawSize           NoteProperty double RawSize=358229
Size              NoteProperty string Size=349.83 KB
```
