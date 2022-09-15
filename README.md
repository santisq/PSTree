# PSTree

### DESCRIPTION
Cmdlet that intends to emulate the [`tree`](https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/tree) command and also calculate the __folder's total size__.

---
### CHANGELOG

- __06/19/2022__

    - Added [format view](https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_format.ps1xml?view=powershell-7.2&viewFallbackFrom=powershell-6) for the Module - [`PSTree.Format.ps1xml`](https://github.com/santysq/PSTree/blob/main/PSTree/Format/PSTree.Format.ps1xml).
    - The module now uses [`EnumerateFileSystemInfos()`](https://docs.microsoft.com/en-us/dotnet/api/system.io.directoryinfo.enumeratefilesysteminfos?view=net-6.0#system-io-directoryinfo-enumeratefilesysteminfos) instance method.
    - Improved error handling (a lot).
    - `-Files` parameter has been replaced with `-Directory` parameter, now the module displays files by default.
    - `-Deep` parameter has been replaced with `-Recurse` parameter, same functionality.
    - `PSTreeDirectory` and `PSTreeFile` instances now only include 2 visible properties, `Hierarchy` and `Length`, the rest is done with format view.

```
PS /home/user/.local/share/powershell/Modules> gpstree . -Recurse

Mode     Hierarchy                                                          Size
----     ---------                                                          ----
d----    PSTree                                                             11.5 KB
-a---    ├── install.ps1                                                    1.82 KB
-a---    ├── LICENSE                                                        1.07 KB
-a---    ├── README.md                                                      8.61 KB
d----    └── PSTree                                                         4.52 KB
-a---        ├── PSTree.psd1                                                4.16 KB
-a---        ├── PSTree.psm1                                                372 B
d----        ├── public                                                     3.74 KB
-a---        │   └── Get-PSTree.ps1                                         3.74 KB
d----        ├── private                                                    0 B
d----        │   └── classes                                                3.01 KB
-a---        │       └── PSTree Classes.ps1                                 3.01 KB
d----        └── Format                                                     1.81 KB
-a---            └── PSTree.Format.ps1xml                                   1.81 KB
```

- __05/24/2022__

    - Lots of code improvements have been done to the Module and improved error handling. Now uses the [`GetDirectories()`](https://docs.microsoft.com/en-us/dotnet/api/system.io.directoryinfo.getdirectories?view=net-6.0#system-io-directoryinfo-getdirectories) and [`GetFiles()`](https://docs.microsoft.com/en-us/dotnet/api/system.io.directoryinfo.getfiles?view=net-6.0#system-io-directoryinfo-getfiles) methods from [`System.IO.DirectoryInfo`](https://docs.microsoft.com/en-us/dotnet/api/system.io.directoryinfo?view=net-6.0). Each `PSTreeDirectory` instance now holds an instance of `DirectoryInfo`. [`System.Collections.Stack`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.stack?view=net-6.0) has been changed for [`System.Collections.Generic.Stack<T>`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.stack-1?view=net-6.0).

- __04/21/2022__

    - __PSTree Module__ now uses [`System.Collections.Stack`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.stack?view=net-6.0) instead of recursion, performance should be much better now and functionality remains the same. Special thanks to [IISResetMe](https://github.com/IISResetMe).

- __01/02/2022__

    - __PSTree Module__ now has it's own classes, functionality remains the same however a lot has been improved.
    - Recursion is now done using the static methods [`[System.IO.Directory]::GetDirectories()`](https://docs.microsoft.com/en-us/dotnet/api/system.io.directory.getdirectories?view=net-6.0) and [`[System.IO.Directory]::GetFiles()`](https://docs.microsoft.com/en-us/dotnet/api/system.io.directory.getfiles?view=net-6.0) instead of [`Get-ChildItem`](https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.management/get-childitem).

- __12/25/2021__

    - `-Files` switch has been added to the Module, now you can display files in the hierarchy tree if desired.
    - `Type` property has been added to the output object and is now part of the _Default MemberSet_.

---


### PARAMETER

| Parameter Name | Description
| --- | --- |
| `-Path <string>` | Absolute or relative folder path. Alias: `FullName` |
| `[-Depth <int>]` | Specifies the maximum level of recursion |
| `[-Recurse <switch>]` | Recursion until maximum level is reached |
| `[-Force <switch>]` | Display hidden and system files and folders |
| `[-Directory <switch>]` | Display only Directories in the Hierarchy tree |
| `[<CommonParameters>]` | See [`about_CommonParameters`](https://go.microsoft.com/fwlink/?LinkID=113216) |

### OUTPUTS `Object[]`

### `PSTreeDirectory` Class

```powershell
   TypeName: PSTreeDirectory

Name                     MemberType Definition
----                     ---------- ----------
EnumerateDirectories     Method     System.Collections.Generic.IEnumerable[System.IO.DirectoryInfo] EnumerateDirectories()
EnumerateFiles           Method     System.Collections.Generic.IEnumerable[System.IO.FileInfo] EnumerateFiles()
EnumerateFileSystemInfos Method     System.Collections.Generic.IEnumerable[System.IO.FileSystemInfo] EnumerateFileSystemInfos()
Equals                   Method     bool Equals(System.Object obj)
GetHashCode              Method     int GetHashCode()
GetType                  Method     type GetType()
ToString                 Method     string ToString()
Hierarchy                Property   string Hierarchy {get;set;}
Length                   Property   long Length {get;set;}
```
### `PSTreeFile` Class

#### Properties

```powershell
   TypeName: PSTreeFile

Name        MemberType Definition
----        ---------- ----------
Equals      Method     bool Equals(System.Object obj)
GetHashCode Method     int GetHashCode()
GetType     Method     type GetType()
ToString    Method     string ToString()
Hierarchy   Property   string Hierarchy {get;set;}
Length      Property   long Length {get;set;}
```

### COMPATIBILITY
- Tested and compatible with __PowerShell v5.1__ and __PowerShell Core__.

### How to install?

- [`install.ps1`](https://github.com/santysq/PSTree/blob/main/install.ps1) can be used to download and install the Module automatically:

```powershell
Invoke-RestMethod https://raw.githubusercontent.com/santysq/PSTree/main/install.ps1 | Invoke-Expression
```

- Alternatively, you can `git clone` or download the `.zip` and extract the `PSTree` folder to your [`$env:PSModulePath`](https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_psmodulepath?view=powershell-7.2).

### How to use?

- __`Get-PSTree .`__ Gets the hierarchy and folder size of the current directory using __default Depth (3)__.
- __`Get-PSTree C:\users\user -Depth 10 -Force`__ Gets the hierarchy and folder size, including hidden ones, of the `user` directory  with a maximum of __10__ levels of recursion.
- __`Get-PSTree /home/user -Recurse`__ Gets the hierarchy and folder size of the `user` directory and all folders below.

---

### Example

```
PS D:\> $tree = gpstree C:\Windows\System32\ -Directory -Depth 2 -EA 0
PS D:\> $tree | Select-Object -First 20

Mode     Hierarchy                                                          Size
----     ---------                                                          ----
d----    System32                                                           2.1 GB
d----    ├── zh-TW                                                          204.5 KB
d----    ├── zh-CN                                                          234.49 KB
d----    ├── winrm                                                          0 B
d----    │   └── 0409                                                       100.12 KB
d----    ├── WinMetadata                                                    6.13 MB
d----    ├── winevt                                                         0 B
d----    │   ├── TraceFormat                                                0 B
d----    │   └── Logs                                                       261.52 MB
d----    ├── WindowsPowerShell                                              0 B
d----    │   └── v1.0                                                       1.73 MB
d----    ├── WinBioPlugIns                                                  1.86 MB
d----    │   ├── FaceDriver                                                 680.34 KB
d----    │   └── en-US                                                      0 B
d----    ├── WinBioDatabase                                                 1.12 KB
d----    ├── WCN                                                            0 B
d----    │   └── en-US                                                      0 B
d----    ├── wbem                                                           66.35 MB
d----    │   ├── xml                                                        99.87 KB
d----    │   ├── tmf                                                        0 B
```