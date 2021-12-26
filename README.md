# PSTree

### DESCRIPTION
Cmdlet designed to emulate the [`tree`](https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/tree) command and also calculate the __folder's size__.

---
### CHANGELOG

- __12/25/2021__

    - `-Files` switch has been added to the Module, now you can display files in the hierarchy tree if desired.
    - `Type` property has been added to the output object and is now part of the _Default MemberSet_.

```
PS /etc> gpstree . -Deep -Files -EA 0 | Select-Object -First 20

Type   Hierarchy                                Size
----   ---------                                ----
Folder etc                                      349.83 KB
Folder ├── acpi                                 1.83 KB
File   │   ├── asus-keyboard-backlight.sh       391 B
File   │   ├── asus-wireless.sh                 180 B
File   │   ├── ibm-wireless.sh                  608 B
File   │   ├── tosh-wireless.sh                 455 B
File   │   ├── undock.sh                        238 B
Folder │   └── events                           1.44 KB
File   │       ├── asus-keyboard-backlight-down 271 B
File   │       ├── asus-keyboard-backlight-up   265 B
File   │       ├── asus-wireless-off            73 B
File   │       ├── asus-wireless-on             72 B
File   │       ├── ibm-wireless                 223 B
File   │       ├── lenovo-undock                67 B
File   │       ├── thinkpad-cmos                277 B
File   │       └── tosh-wireless                222 B
Folder ├── alternatives                         5.08 KB
File   │   ├── animate                          24 B
File   │   ├── animate-im6                      24 B
File   │   ├── animate-im6.1.gz                 40 B
```
---


### PARAMETER

| Parameter Name | Description
| --- | --- |
| `-Path <string>` | Absolute or relative folder path. Alias: `FullName`, `PSPath` |
| `[-Depth <int>]` | Specifies the maximum level of recursion |
| `[-Deep <switch>]` | Recursion until maximum level is reached |
| `[-Force <switch>]` | Display hidden and system files and folders |
| `[-Files <switch>]` | Files will be displayed in the Hierarchy tree |
| `[<CommonParameters>]` | See [`about_CommonParameters`](https://go.microsoft.com/fwlink/?LinkID=113216) |

### OUTPUTS `Object[]`

```
Name           TypeNameOfValue
----           ---------------
Type           System.String
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

### How to install?

[`install.ps1`](https://github.com/santysq/PSTree/blob/main/install.ps1) can be used to download and install the Module, alternatively, you can `git clone` or download the `.zip` and extract the `PSTree` folder to your [`$env:PSModulePath`](https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_psmodulepath?view=powershell-7.2).

### How to use?

- __`Get-PSTree .`__ Gets the hierarchy and folder size of the current directory using __default Depth (3)__.
- __`Get-PSTree C:\users\user -Depth 10 -Force`__ Gets the hierarchy and folder size, including hidden ones, of the `user` directory  with a maximum of __10__ levels of recursion.
- __`Get-PSTree /home/user -Deep`:__ Gets the hierarchy and folder size of the `user` directory and all folders below.

---

### Sample

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
