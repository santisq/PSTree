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

    begin
    {
        [Environment]::CurrentDirectory = $pwd.Path
        $PSBoundParameters.Path = ([System.IO.FileInfo]$Path).FullName
    }

    process
    {        
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
