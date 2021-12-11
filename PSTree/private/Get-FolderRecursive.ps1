function Get-FolderRecursive {
[cmdletbinding()]
param(
    [string]$Path,
    [int]$Nesting = 0,
    [int]$Depth,
    [switch]$Deep,
    [switch]$Force
)

    $outObject = {
        param($Nesting, $folder)
        
        $files = Get-ChildItem -LiteralPath $folder.FullName -File -Force
        [long]$size = ($files | Measure-Object Length -Sum).Sum
        
        [pscustomobject]@{
            Nesting        = $Nesting
            Hierarchy      = Indent -String $folder.Name -Indent $Nesting
            Size           = SizeConvert $size
            FullName       = $folder.FullName
            Parent         = $folder.Parent
            CreationTime   = $folder.CreationTime
            LastAccessTime = $folder.LastAccessTime
            LastWriteTime  = $folder.LastWriteTime
        }
    }

    if(-not $Nesting)
    {
        $parent = Get-Item -LiteralPath $Path
        & $outObject -Nesting $Nesting -Folder $parent
    }

    $Nesting++

    $folders = if($Force.IsPresent)
    {
        Get-ChildItem -LiteralPath $Path -Directory -Force
    }
    else
    {
        Get-ChildItem -LiteralPath $Path -Directory
    }

    foreach($folder in $folders)
    {
        & $outObject -Nesting $Nesting -Folder $folder
        
        $PSBoundParameters.Path = $folder.FullName
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
