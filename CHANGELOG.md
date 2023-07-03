# CHANGELOG

- __07/03/2023__
  - Added `-Path` parameter, now both `-Path` and `-LiteralPath` parameters take `string[]` as input and support pipeline input.
  - Added Pester tests, Code Coverage and coverage upload to [codecov.io](https://app.codecov.io/gh/santisq/PSTree).
  - Removed `.Size` Property from `PSTreeFile` and `PSTreeDirectory` instances. The `Size` column has been renamed to `Length` and the default display for this column is available through `[PSTree.Internal._Format]::GetFormattedLength(...)`, for example:

    ```powershell
    Get-PSTree | Select-Object Hierarchy, @{ N='Size'; E={ [PSTree.Internal._Format]::GetFormattedLength($_.Length) }}
    ```

  - Added `GroupBy` tag to the default view, now trees are grouped by the source Path omg! Little example:

    ```powershell
    PS ..\PSTree> Get-PSTree .\src\, .\module\ -Exclude *\obj,*\bin

      Source: C:\path\to\PSTree\src

    Mode            Length Hierarchy
    ----            ------ ---------
    d----          0.00  B src
    d----         10.30 KB └── PSTree
    -a---        931.00  B     ├── ExceptionHelpers.cs
    -a---        439.00  B     ├── PSTree.csproj
    -a---          1.06 KB     ├── PSTreeDirectory.cs
    -a---          4.01 KB     ├── PSTreeExtensions.cs
    -a---        517.00  B     ├── PSTreeFile.cs
    -a---        399.00  B     ├── PSTreeFileSystemInfo.cs
    -a---          1.51 KB     ├── PSTreeFileSystemInfo_T.cs
    -a---        897.00  B     ├── PSTreeHelper.cs
    -a---        619.00  B     ├── PSTreeIndexer.cs
    d----          1.13 KB     ├── Internal
    -a---          1.13 KB     │   └── _Format.cs
    d----          5.68 KB     └── Commands
    -a---          5.68 KB         └── GetPSTreeCommand.cs

      Source: C:\path\to\PSTree

    Mode            Length Hierarchy
    ----            ------ ---------
    d----          6.22 KB module
    -a---          1.54 KB ├── PSTree.Format.ps1xml
    -a---          4.67 KB └── PSTree.psd1
    ```

- __03/22/2023__
  - `Get-PSTree` is now a binary cmdlet. Functionality remains the same. Big thanks to [SeeminglyScience](https://github.com/SeeminglyScience/) and [jborean93](https://github.com/jborean93/) for all their help!
  - Added `-Exclude` parameter to the cmdlet. The parameter accepts wildcards and patterns are matched with the object's `.FullName` property. For more details checkout [cmdlet docs](/docs/en-US/Get-PSTree.md).

- __02/25/2023__
  - Fixed a bug that made `Get-PSTree` use `-Recurse` by default.
  - Added [ETS properties](./PSTree/PSTree.Types.ps1xml) to `PSTreeDirectory` and `PSTreeFile` instances that would make exporting the output easier.

- __10/23/2022__
  - __PSTree Module__ is now published to the [PowerShell Gallery](https://www.powershellgallery.com/)!
  - Introducing `-RecursiveSize` switch parameter to `Get-PSTree`. By default, `Get-PSTree` only displays the size of folders __based on the sum of the files length in each Directory__.
  This parameter allows to calculate the recursive size of folders in the hierarchy, similar to how explorer does it. __It's important to note that this is a more expensive operation__, in order to calculate the recursive size, all folders in the hierarchy need to be traversed.

```powershell
PS ..\PSTree> pstree -Directory -Depth 2

Mode  Hierarchy          Size
----  ---------          ----
d---- PSTree          9.51 Kb
d---- └── PSTree      4.83 Kb
d----     ├── public   4.8 Kb
d----     ├── private 0 Bytes
d----     └── Format  1.83 Kb

PS ..\PSTree> pstree -Directory -Depth 2 -RecursiveSize

Mode  Hierarchy            Size
----  ---------            ----
d---- PSTree          180.38 Kb
d---- └── PSTree       14.75 Kb
d----     ├── public     4.8 Kb
d----     ├── private   3.29 Kb
d----     └── Format    1.83 Kb
```

- __06/19/2022__
  - Added [format view](https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_format.ps1xml?view=powershell-7.2&viewFallbackFrom=powershell-6) for the Module - [`PSTree.Format.ps1xml`](PSTree/PSTree.Format.ps1xml).
  - The module now uses [`EnumerateFileSystemInfos()`](https://docs.microsoft.com/en-us/dotnet/api/system.io.directoryinfo.enumeratefilesysteminfos?view=net-6.0#system-io-directoryinfo-enumeratefilesysteminfos) instance method.
  - Improved error handling (a lot).
  - `-Files` parameter has been replaced with `-Directory` parameter, now the module displays files by default.
  - `-Deep` parameter has been replaced with `-Recurse` parameter, same functionality.
  - `PSTreeDirectory` and `PSTreeFile` instances now only include 2 visible properties, `Hierarchy` and `Length`, the rest is done with format view.

```powershell
PS ..\PSTree> pstree -Recurse

Mode  Hierarchy                             Size
----  ---------                             ----
d---- PSTree                            10.21 Kb
-a--- ├── LICENSE                        1.07 Kb
-a--- ├── README.md                      9.15 Kb
d---- └── PSTree                         4.83 Kb
-a---     ├── PSTree.psd1                4.57 Kb
-a---     ├── PSTree.psm1              270 Bytes
d----     ├── public                      4.8 Kb
-a---     │   └── Get-PSTree.ps1          4.8 Kb
d----     ├── private                    0 Bytes
d----     │   └── classes                3.29 Kb
-a---     │       └── classes.ps1        3.29 Kb
d----     └── Format                     1.83 Kb
-a---         └── PSTree.Format.ps1xml   1.83 Kb
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
