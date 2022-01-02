function Get-FolderRecursive {
[cmdletbinding()]
param(
    [object]$Path,
    [int]$Nesting = 0,
    [int]$Depth,
    [switch]$Deep,
    [switch]$Force,
    [switch]$Files
)

    if(-not $Nesting)
    {
        $Path = [PSTreeParent]::new($Path)
        $file = $Path.GetFiles($Force.IsPresent)        
        $Path
        if($Files.IsPresent)
        {
            $file
        }
    }

    $Nesting++

    foreach($folder in $Path.GetFolders($Force.IsPresent))
    {
        $file = $Folder.GetFiles($Force.IsPresent)
        $folder
        if($Files.IsPresent)
        {
            $file
        }
        
        $PSBoundParameters.Path = $folder
        $PSBoundParameters.Nesting = $Nesting

        if($PSBoundParameters.ContainsKey('Depth'))
        {
            if($Nesting -lt $Depth)
            {
                Get-FolderRecursive @PSBoundParameters
            }
        }
        
        if($Deep.IsPresent)
        {
            Get-FolderRecursive @PSBoundParameters
        }
    }
}
