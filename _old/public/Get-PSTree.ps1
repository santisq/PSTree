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
Get-PSTree

Get hierarchy of the current Directory with default parameters (`-Depth 3`)

.EXAMPLE
Get-PSTree -Directory -Recurse

Get hierarchy of the current Directory recursively displaying only Folders

.EXAMPLE
Get-PSTree -Depth 2 -Force

Get hierarchy of the current Directory 2 levels deep and displaying hidden Folders

.EXAMPLE
Get-PSTree -Depth 2 -RecursiveSize -Directory

Get hierarchy 2 levels deep displaying only Folders with their recursive size

.LINK
https://github.com/santisq/PSTree
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

        [parameter(ParameterSetName = 'Recurse', Position = 1)]
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