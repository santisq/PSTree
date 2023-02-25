using namespace System.IO
using namespace System.Collections.Generic

<#
    .SYNOPSIS
    tree like function for PowerShell

    .DESCRIPTION
    PowerShell function that intends to emulate the tree command with added functionality to calculate the folders size as well as recursive folders size.

    .PARAMETER LiteralPath
    Absolute or relative folder path.

    .PARAMETER Depth
    Determines the number of subdirectory levels that are included in the recursion.

    .PARAMETER Recurse
    Gets the items in the specified locations and in all child items of the locations.

    .PARAMETER Force
    Gets items that otherwise can't be accessed by the user, such as hidden or system files.

    .PARAMETER Directory
    Displays Directories only.

    .PARAMETER RecursiveSize
    Displays the recursive Size of Folders.

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
    [OutputType('PSTreeDirectory', 'PSTreeFile')]
    param(
        [Parameter(Position = 0, ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [Alias('PSPath')]
        [string] $LiteralPath = $PWD.Path,

        [Parameter(ParameterSetName = 'Depth', Position = 1)]
        [int] $Depth = 3,

        [Parameter(ParameterSetName = 'Recurse', Position = 1)]
        [switch] $Recurse,

        [Parameter()]
        [switch] $Force,

        [Parameter()]
        [switch] $Directory,

        [Parameter()]
        [switch] $RecursiveSize
    )

    begin {
        $isRecursive = $RecursiveSize.IsPresent -or $Recurse.IsPresent
    }

    process {
        try {
            $absolutePath = $PSCmdlet.GetUnresolvedProviderPathFromPSPath($LiteralPath)
            if([File]::GetAttributes($absolutePath).HasFlag([FileAttributes]::Archive)) {
                return [PSTreeFile]::new($absolutePath, 0)
            }

            if($absolutePath -ne [Path]::GetPathRoot($absolutePath)) {
                $absolutePath = $absolutePath.TrimEnd([Path]::DirectorySeparatorChar)
            }

            foreach($item in $PSCmdlet.InvokeProvider.Item.Get($absolutePath, $Force.IsPresent, $true)) {
                $indexer = @{}
                $stack   = [Stack[PSTreeDirectory]]::new()
                $stack.Push([PSTreeDirectory]::new($item, 0))

                $output = while($stack.Count) {
                    $next  = $stack.Pop()
                    $level = $next.Depth + 1
                    $size  = 0

                    try {
                        $enum = $next.EnumerateFileSystemInfos()
                    }
                    catch {
                        if($Recurse.IsPresent -or $next.Depth -le $Depth) {
                            $next
                        }

                        $PSCmdlet.WriteError($_)
                        continue
                    }

                    $keepProcessing = $isRecursive -or $level -le $Depth

                    $items = foreach($item in $enum) {
                        if(-not $Force.IsPresent -and $item.Attributes.HasFlag([FileAttributes]::Hidden)) {
                            continue
                        }

                        if($item -is [FileInfo]) {
                            $size += $item.Length

                            if($Directory.IsPresent) {
                                continue
                            }

                            [PSTreeFile]::new($item, $level)
                            continue
                        }

                        if($keepProcessing) {
                            $stack.Push([PSTreeDirectory]::new($item, $level))
                        }
                    }

                    $next.SetSize($size)
                    $indexer[$next.FullName] = $next

                    if($RecursiveSize.IsPresent) {
                        foreach($parent in $next.GetParents($indexer)) {
                            $indexer[$parent].AddSize($size)
                        }
                    }

                    if($Recurse.IsPresent -or $next.Depth -le $Depth) {
                        $next

                        if($items -and $Recurse.IsPresent -or $level -le $Depth) {
                            $items
                        }
                    }
                }

                [PSTreeStatic]::DrawTree($output, 'Hierarchy', 'Depth')
            }
        }
        catch {
            $PSCmdlet.WriteError($_)
        }
    }
}