using namespace System.IO
using namespace System.Collections.Generic

<#
.SYNOPSIS
tree like function for PowerShell

.DESCRIPTION
PowerShell function that intends to emulate the tree command with added functionality to calculate the folders size as well as recursive folders size.

.PARAMETER Path
Absolute or relative folder path.

.PARAMETER Depth
Controls the recursion limit

.PARAMETER Recurse
Traverse all Directory hierarchy

.PARAMETER Force
Displays hidden Files and Folders

.PARAMETER Directory
Displays Folders only

.PARAMETER RecursiveSize
Displays the recursive Folders Size

.INPUTS
System.String

You can pipe a string that contains a path to Get-PSTree.
Can take pipeline input from cmdlets that outputs System.IO.DirectoryInfo.

.OUTPUTS
Object[], PSTreeDirectory, PSTreeFile

.EXAMPLE

Get the hierarchy of the current Directory using default parameters (-Depth 3).
Get-PSTree

Mode  Hierarchy                             Size
----  ---------                             ----
d---- PSTree                             9.52 Kb
-a--- ├── LICENSE                        1.07 Kb
-a--- ├── README.md                      8.45 Kb
d---- └── PSTree                         4.83 Kb
-a---     ├── PSTree.psd1                4.57 Kb
-a---     ├── PSTree.psm1              270 Bytes
d----     ├── public                     4.91 Kb
-a---     │   └── Get-PSTree.ps1         4.91 Kb
d----     ├── private                    0 Bytes
d----     │   └── classes                3.29 Kb
-a---     │       └── classes.ps1        3.29 Kb
d----     └── Format                     1.83 Kb
-a---         └── PSTree.Format.ps1xml   1.83 Kb


.EXAMPLE
Get the hierarchy of the current Directory displaying only Folders

PS ..\PSTree> Get-PSTree -Directory

Mode  Hierarchy              Size
----  ---------              ----
d---- PSTree              9.52 Kb
d---- └── PSTree          4.83 Kb
d----     ├── public      5.72 Kb
d----     ├── private     0 Bytes
d----     │   └── classes 3.29 Kb
d----     └── Format      1.83 Kb


.EXAMPLE
PS /> Get-PSTree /home/user -Recurse
Gets the hierarchy and folder size of the user directory and all folders below.

.EXAMPLE
PS /> Get-PSTree /home/user -Recurse -Directory
Gets the hierarchy (excluding files) and folder size of the user directory and all folders below.

.LINK
https://github.com/santysq/PSTree
#>

function Get-PSTree {
    [cmdletbinding(DefaultParameterSetName = 'Depth')]
    param(
        [parameter(Position = 0, ValueFromPipeline)]
        [alias('FullName')]
        [ValidateScript({
            if(Test-Path $_ -PathType Container) {
                return $true
            }
            throw 'Invalid Folder Path.'
        })]
        [string] $Path = $PWD,

        [ValidateRange(1, [int]::MaxValue)]
        [parameter(ParameterSetName = 'Depth', Position = 1)]
        [int] $Depth = 3,

        [parameter(ParameterSetName = 'Max', Position = 1)]
        [switch] $Recurse,

        [parameter()]
        [switch] $Force,

        [parameter()]
        [switch] $Directory,

        [parameter()]
        [switch] $RecursiveSize
    )

    begin {
        $withDepth = $PSBoundParameters.ContainsKey('Depth')
        $hidden    = [FileAttributes] 'Hidden'
        $ignore    = [HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
        $indexer   = @{}
    }

    process {
        $Path = $PSCmdlet.GetUnresolvedProviderPathFromPSPath($Path).TrimEnd('\/')
        if($Path.EndsWith(':')) {
            $Path = $ExecutionContext.SessionState.Path.Combine($Path, '')
        }

        $stack = [Stack[PSTreeDirectory]]::new()
        $stack.Push([PSTreeDirectory]::new($Path, 0))

        $output = :outer while($stack.Count) {
            $next  = $stack.Pop()
            $level = $next.Depth + 1
            $size  = 0

            if(-not $RecursiveSize.IsPresent -and $withDepth -and $next.Depth -gt $Depth) {
                continue
            }

            try {
                $enum = $next.EnumerateFileSystemInfos()
            }
            catch {
                $PSCmdlet.WriteError($_)
                continue
            }

            $files = foreach($item in $enum) {
                if($item -is [FileInfo]) {
                    $size += $item.Length

                    if(-not $Force.IsPresent -and $item.Attributes.HasFlag($hidden)) {
                        continue
                    }

                    if(-not $RecursiveSize.IsPresent -and $withDepth -and $level -gt $Depth) {
                        continue
                    }

                    [PSTreeFile]::new($item, $level)
                    continue
                }

                $stack.Push([PSTreeDirectory]::new($item, $level))
            }

            $next.SetSize($size)
            $absolutePath = $next.GetAbsolutePath()
            $indexer[$absolutePath] = $next
            $parents = $next.GetParents($indexer)

            if($RecursiveSize.IsPresent) {
                foreach($parent in $parents) {
                    $indexer[$parent].AddSize($size)
                }
            }

            if($absolutePath -ne $Path) {
                if(-not $Force.IsPresent -and $next.GetAttributes().HasFlag($hidden)) {
                    $null = $ignore.Add($absolutePath)
                    continue
                }
            }

            foreach($parent in $parents) {
                if($ignore.Contains($parent)) {
                    continue outer
                }
            }

            if($withDepth -and $next.Depth -gt $Depth) {
                continue
            }

            $next

            if(-not $Directory.IsPresent) {
                $files
            }
        }

        [PSTreeStatic]::DrawHierarchy($output, 'Hierarchy', 'Depth')
    }
}