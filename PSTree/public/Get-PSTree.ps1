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
String.

You can pipe a string that contains a path to Get-PSTree.
Can take pipeline input from cmdlets that outputs System.IO.DirectoryInfo.

.OUTPUTS
Object.

Name           TypeNameOfValue
----           ---------------
Attributes     System.IO.FileAttributes
Hierarchy      System.String
Size           System.String
RawSize        System.Int64
Name           System.String
FullName       System.String
Parent         System.IO.DirectoryInfo
CreationTime   System.DateTime
LastAccessTime System.DateTime
LastWriteTime  System.DateTime

.EXAMPLE
PS /> Get-PSTree .
Gets the hierarchy and folder size of the current directory using default Depth (3).

.EXAMPLE
PS /> Get-PSTree C:\users\user -Depth 10 -Force
Gets the hierarchy and folder size, including hidden ones, of the user directory with a maximum of 10 levels of recursion.

.EXAMPLE
PS /> Get-PSTree /home/user -Deep
Gets the hierarchy and folder size of the user directory and all folders below.

.LINK
https://github.com/santysq/PSTree
#>

function Get-PSTree {
[cmdletbinding(DefaultParameterSetName = 'Depth')]
[alias('gpstree')]
param(
    [parameter(
        Mandatory,
        ParameterSetName = 'Path',
        Position = 0
    )]
    [parameter(
        Mandatory,
        ParameterSetName = 'Depth',
        Position = 0
    )]
    [parameter(
        Mandatory,
        ParameterSetName = 'Max',
        Position = 0
    )]
    [alias('FullName', 'PSPath')]
    [ValidateScript({ 
        if(Test-Path $_ -PathType Container)
        {
            return $true
        }
        throw 'Invalid Folder Path'
    })]
    [string]$Path,
    [ValidateRange(1, [int]::MaxValue)]
    [parameter(
        ParameterSetName = 'Depth',
        Position = 1
    )]
    [int]$Depth = 3,
    [parameter(
        ParameterSetName = 'Max',
        Position = 1
    )]
    [switch]$Deep,
    [switch]$Force,
    [switch]$Files
)

    begin {
        $PSBoundParameters.Path = $PSCmdlet.GetUnresolvedProviderPathFromPSPath($Path)
    }

    process {
        $isDepthParam = $PSCmdlet.ParameterSetName -eq 'Depth'
        $containsDepth = $PSBoundParameters.ContainsKey('Depth')
        
        if($isDepthParam -and -not $containsDepth)
        {
            $PSBoundParameters.Add('Depth', $Depth)
        }

        [PSTreeStatic]::DrawHierarchy(
            (Get-FolderRecursive @PSBoundParameters),
            'Hierarchy',
            'Nesting'
        )
    }
}
