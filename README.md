# PSTree

### DESCRIPTION
Cmdlet that intends to emulate the [`tree`](https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/tree) command and also calculate the __folder's total size__.

---
### CHANGELOG

- __05/24/2022__

    - Lots of code improvements have been done to the Module. Now uses [`System.IO.DirectoryInfo`](https://docs.microsoft.com/en-us/dotnet/api/system.io.directoryinfo?view=net-6.0) to get files and directories. Each `PSTreeDirectory` instance now holds an instance of `DirectoryInfo`. [`System.Collections.Stack`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.stack?view=net-6.0) has been changed for [`System.Collections.Generic.Stack<T>`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.stack-1?view=net-6.0).

- __04/21/2022__
    
    - __PSTree Module__ now uses [`System.Collections.Stack`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.stack?view=net-6.0) instead of recursion, perfomance should be much better now and functionality remains the same. Special thanks to [IISResetMe](https://github.com/IISResetMe).

- __01/02/2022__
    
    - __PSTree Module__ now has it's own classes, functionality remains the same however a lot has been improved.
    - Recursion is now done using the static methods [`[System.IO.Directory]::GetDirectories()`](https://docs.microsoft.com/en-us/dotnet/api/system.io.directory.getdirectories?view=net-6.0) and [`[System.IO.Directory]::GetFiles()`](https://docs.microsoft.com/en-us/dotnet/api/system.io.directory.getfiles?view=net-6.0) instead of [`Get-ChildItem`](https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.management/get-childitem).

```
PS /home/user/.local/share/powershell/Modules> gpstree . -Files -Deep 

Attributes Hierarchy                               Size
---------- ---------                               ----
 Directory Modules                                 0 B
 Directory └── PSTree                              4.34 KB
    Normal     ├── PSTree.psd1                     4 KB
    Normal     ├── PSTree.psm1                     352 B
 Directory     ├── public                          1.44 KB
    Normal     │   └── Get-PSTree.ps1              1.44 KB
 Directory     └── private                         0 B
 Directory         ├── classes                     6.91 KB
    Normal         │   └── PSTree Classes.ps1      6.91 KB
 Directory         └── functions                   1012 B
    Normal             └── Get-FolderRecursive.ps1 1012 B
```
- __12/25/2021__

    - `-Files` switch has been added to the Module, now you can display files in the hierarchy tree if desired.
    - `Type` property has been added to the output object and is now part of the _Default MemberSet_.
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

### `PSTreeDirectory` Class

```
   TypeName: PSTreeDirectory

Name              MemberType Definition
----              ---------- ----------
PSStandardMembers MemberSet  PSStandardMembers {DefaultDisplayPropertySet}
Equals            Method     bool Equals(System.Object obj)
GetFiles          Method     PSTreeFile[] GetFiles(bool Force), PSTreeFile[] GetFiles(string Path, long Nesting, bool Force)
GetFolders        Method     PSTreeDirectory[] GetFolders(bool Force), PSTreeDirectory[] GetFolders(string Path, long Nesting, bool Force)
GetHashCode       Method     int GetHashCode()
GetType           Method     type GetType()
SetHierarchy      Method     void SetHierarchy()
ToString          Method     string ToString()
Attributes        Property   System.IO.FileAttributes Attributes {get;set;}
CreationTime      Property   datetime CreationTime {get;set;}
FullName          Property   string FullName {get;set;}
Hierarchy         Property   string Hierarchy {get;set;}
IOInstance        Property   System.IO.DirectoryInfo IOInstance {get;set;}
LastAccessTime    Property   datetime LastAccessTime {get;set;}
LastWriteTime     Property   datetime LastWriteTime {get;set;}
Name              Property   string Name {get;set;}
Parent            Property   System.IO.DirectoryInfo Parent {get;set;}
RawSize           Property   long RawSize {get;set;}
Size              Property   string Size {get;set;}
```

### `PSTreeFile` Class

```
   TypeName: PSTreeFile

Name              MemberType Definition
----              ---------- ----------
PSStandardMembers MemberSet  PSStandardMembers {DefaultDisplayPropertySet}
Equals            Method     bool Equals(System.Object obj)
GetHashCode       Method     int GetHashCode()
GetType           Method     type GetType()
ToString          Method     string ToString()
Attributes        Property   System.IO.FileAttributes Attributes {get;set;}
CreationTime      Property   datetime CreationTime {get;set;}
FullName          Property   string FullName {get;set;}
Hierarchy         Property   string Hierarchy {get;set;}
LastAccessTime    Property   datetime LastAccessTime {get;set;}
LastWriteTime     Property   datetime LastWriteTime {get;set;}
Name              Property   string Name {get;set;}
Parent            Property   System.IO.DirectoryInfo Parent {get;set;}
RawSize           Property   long RawSize {get;set;}
Size              Property   string Size {get;set;}
```

### COMPATIBILITY
- Tested and compatible with __PowerShell v5.1__ and __PowerShell Core__.

### How to install?

- [`install.ps1`](https://github.com/santysq/PSTree/blob/main/install.ps1) can be used to download and install the Module automatically:

```
Invoke-RestMethod https://raw.githubusercontent.com/santysq/PSTree/main/install.ps1 | Invoke-Expression
```

- Alternatively, you can `git clone` or download the `.zip` and extract the `PSTree` folder to your [`$env:PSModulePath`](https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_psmodulepath?view=powershell-7.2).

### How to use?

- __`Get-PSTree .`__ Gets the hierarchy and folder size of the current directory using __default Depth (3)__.
- __`Get-PSTree C:\users\user -Depth 10 -Force`__ Gets the hierarchy and folder size, including hidden ones, of the `user` directory  with a maximum of __10__ levels of recursion.
- __`Get-PSTree /home/user -Deep`__ Gets the hierarchy and folder size of the `user` directory and all folders below.

---

### Sample

```
PS /etc> $hierarchy = gpstree . -Depth 5 -EA 0
PS /etc> $hierarchy | Select-Object -First 20

             Attributes Hierarchy                            Size
             ---------- ---------                            ----
    ReadOnly, Directory etc                                  294.45 KB
    ReadOnly, Directory ├── netplan                          104 B
    ReadOnly, Directory ├── libgda-5.0                       100 B
    ReadOnly, Directory ├── dconf                            0 B
    ReadOnly, Directory │   ├── profile                      28 B
    ReadOnly, Directory │   └── db                           2.85 KB
    ReadOnly, Directory │       └── ibus.d                   1.49 KB
    ReadOnly, Directory ├── logrotate.d                      2.94 KB
    ReadOnly, Directory ├── xdg                              832 B
    ReadOnly, Directory │   ├── autostart                    27.1 KB
    ReadOnly, Directory │   ├── tumbler                      2.22 KB
    ReadOnly, Directory │   ├── menus                        15.54 KB
    ReadOnly, Directory │   ├── systemd                      0 B
Directory, ReparsePoint │   │   └── user                     0 B
    ReadOnly, Directory │   │       ├── default.target.wants 40 B
    ReadOnly, Directory │   │       └── sockets.target.wants 291 B
    ReadOnly, Directory │   └── Xwayland-session.d           215 B
    ReadOnly, Directory ├── cron.weekly                      1.49 KB
    ReadOnly, Directory ├── cron.monthly                     313 B
    ReadOnly, Directory ├── pki                              0 B
    
PS /etc> $hierarchy[0] | Get-Member -MemberType Properties, MemberSet

   TypeName: PSTreeParent

Name              MemberType Definition
----              ---------- ----------
PSStandardMembers MemberSet  PSStandardMembers {DefaultDisplayPropertySet}
Attributes        Property   System.IO.FileAttributes Attributes {get;set;}
CreationTime      Property   datetime CreationTime {get;set;}
FullName          Property   string FullName {get;set;}
Hierarchy         Property   string Hierarchy {get;set;}
LastAccessTime    Property   datetime LastAccessTime {get;set;}
LastWriteTime     Property   datetime LastWriteTime {get;set;}
Name              Property   string Name {get;set;}
Parent            Property   System.IO.DirectoryInfo Parent {get;set;}
RawSize           Property   long RawSize {get;set;}
Size              Property   string Size {get;set;}
```
