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
etc                                    349.3 Kb
├── acpi                               1.8 Kb
│   └── events                         1.4 Kb
├── alternatives                       5.1 Kb
├── apache2                            0 Bytes
│   ├── conf-available                 127 Bytes
│   └── mods-available                 156 Bytes
├── apm                                0 Bytes
│   ├── event.d                        2.9 Kb
│   ├── resume.d                       17 Bytes
│   ├── scripts.d                      228 Bytes
│   └── suspend.d                      17 Bytes
├── apparmor                           3.4 Kb
│   └── init                           0 Bytes
│       └── network-interface-security 33 Bytes
├── apparmor.d                         25.9 Kb
│   ├── abstractions                   91.1 Kb
│   │   ├── apparmor_api               2.4 Kb
│   │   └── ubuntu-browsers.d          10.4 Kb
│   ├── cache                          1.1 Mb

PS /etc> $hierarchy[0] | Get-Member

   TypeName: Selected.System.Management.Automation.PSCustomObject

Name              MemberType   Definition
----              ----------   ----------
PSStandardMembers MemberSet    PSStandardMembers {DefaultDisplayPropertySet}
Equals            Method       bool Equals(System.Object obj)
GetHashCode       Method       int GetHashCode()
GetType           Method       type GetType()
ToString          Method       string ToString()
CreationTime      NoteProperty datetime CreationTime=12/9/2021 11:15:53 AM
FullName          NoteProperty string FullName=/etc
Hierarchy         NoteProperty string Hierarchy=etc
LastAccessTime    NoteProperty datetime LastAccessTime=12/10/2021 11:18:27 PM
LastWriteTime     NoteProperty datetime LastWriteTime=12/9/2021 11:15:53 AM
Nesting           NoteProperty int Nesting=0
Parent            NoteProperty DirectoryInfo Parent=/
Size              NoteProperty string Size=349.3 Kb
```
