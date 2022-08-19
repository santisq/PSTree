using namespace System.IO
using namespace System.Collections.Generic

<#
.DESCRIPTION
Cmdlet that intends to emulate the tree command and also calculate the folder's total size.

.PARAMETER Path
Absolute or relative folder path. Alias: FullName, PSPath

.PARAMETER Depth
Specifies the maximum level of recursion

.PARAMETER Deep
Recursion until maximum level is reached

.PARAMETER Force
Display hidden and system files and folders

.PARAMETER Files
Files will be displayed in the Hierarchy tree

.INPUTS
String

You can pipe a string that contains a path to Get-PSTree.
Can take pipeline input from cmdlets that outputs System.IO.DirectoryInfo.

.OUTPUTS
Object[], PSTreeDirectory, PSTreeFile

.EXAMPLE
PS /> Get-PSTree .
Gets the hierarchy and folder size of the current directory using default Depth (3).

.EXAMPLE
PS /> Get-PSTree C:\users\user -Depth 10 -Force
Gets the hierarchy and folder size, including hidden ones, of the user directory with a maximum of 10 levels of recursion.

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
    [alias('gpstree')]
    param(
        [parameter(Mandatory, Position = 0, ValueFromPipeline)]
        [alias('FullName')]
        [ValidateScript({
            if(Test-Path $_ -PathType Container) {
                return $true
            }
            throw 'Invalid Folder Path'
        })]
        [string] $Path,

        [ValidateRange(1, [int]::MaxValue)]
        [parameter(ParameterSetName = 'Depth', Position = 1)]
        [int] $Depth = 3,

        [parameter(ParameterSetName = 'Max', Position = 1)]
        [switch] $Recurse,

        [parameter()]
        [switch] $Force,

        [parameter()]
        [switch] $Directory
    )

    begin {
        $Path = $PSCmdlet.GetUnresolvedProviderPathFromPSPath($Path)
    }

    process {
        $stack = [Stack[PSTreeDirectory]]::new()
        $stack.Push([PSTreeDirectory]::new($Path, 0))

        $output = while($stack.Count) {
            $next  = $stack.Pop()
            $level = $next.Depth + 1
            $size  = 0

            if($PSBoundParameters.ContainsKey('Depth') -and $next.Depth -gt $Depth) {
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
                if($item.Attributes -band [FileAttributes]'Hidden, System' -and -not $Force.IsPresent) {
                    if($item -is [FileInfo]) {
                        $size += $item.Length
                    }
                    continue
                }

                if($item -is [FileInfo]) {
                    $size += $item.Length
                    [PSTreeFile]::new($item, $level)
                    continue
                }

                $stack.Push([PSTreeDirectory]::new($item, $level))
            }

            $next.Length = $size
            $next

            if(-not $Directory.IsPresent) {
                $files
            }
        }
        [PSTreeStatic]::DrawHierarchy($output, 'Hierarchy', 'Depth')
        $output
    }
}