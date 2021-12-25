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

    process
    {
        [Environment]::CurrentDirectory = $pwd.Path
        $PSBoundParameters.Path = ([System.IO.FileInfo]$Path).FullName
        
        $DefaultProps = @(
            'Type'
            'Hierarchy'
            'Size'
        )
        [Management.Automation.PSMemberInfo[]]$standardMembers =
            [System.Management.Automation.PSPropertySet]::new(
                'DefaultDisplayPropertySet',
                [string[]]$DefaultProps
            )
        
        $hash = @{
            Name = 'PSStandardMembers'
            MemberType = 'MemberSet'
            Value = $standardMembers
        }
        
        $isDepthParam = $PSCmdlet.ParameterSetName -eq 'Depth'
        $containsDepth = $PSBoundParameters.ContainsKey('Depth')
        
        if($isDepthParam -and -not $containsDepth)
        {
            $PSBoundParameters.Add('Depth', $Depth)
        }

        $result = @(Get-FolderRecursive @PSBoundParameters)
        
        $drawProps = @{
            Array = $result
            PropertyName = 'Hierarchy'
            RecursionProperty = 'Nesting'
        }

        DrawHierarchy @drawProps | ForEach-Object {
            $hash.InputObject = $_
            Add-Member @hash
        }

        $result
    }
}
