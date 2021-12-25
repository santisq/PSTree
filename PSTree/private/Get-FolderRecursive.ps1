function Get-FolderRecursive {
[cmdletbinding()]
param(
    [string]$Path,
    [int]$Nesting = 0,
    [int]$Depth,
    [switch]$Deep,
    [switch]$Force,
    [switch]$Files
)

    $outObject = {
        param($Nesting, $InputObject, $Size)
        
        [pscustomobject]@{
            Type           = ('File', 'Folder')[$InputObject.PSIsContainer]
            Nesting        = $Nesting
            Hierarchy      = Indent -String $InputObject.Name -Indent $Nesting
            Size           = SizeConvert $size
            RawSize        = $Size
            FullName       = $InputObject.FullName
            Parent         = $InputObject.Parent
            CreationTime   = $InputObject.CreationTime
            LastAccessTime = $InputObject.LastAccessTime
            LastWriteTime  = $InputObject.LastWriteTime
        }
    }

    if(-not $Nesting)
    {
        $parent = Get-Item -LiteralPath $Path
        $file = Get-ChildItem -LiteralPath $parent.FullName -File -Force
        $size = ($file | Measure-Object Length -Sum).Sum

        & $outObject -Nesting $Nesting -InputObject $parent -Size $size
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
        $file = Get-ChildItem -LiteralPath $folder.FullName -File -Force
        $size = ($file | Measure-Object Length -Sum).Sum

        & $outObject -Nesting $Nesting -InputObject $folder -Size $size
        
        if($Files.IsPresent)
        {
            foreach($f in $file)
            {
                & $outObject -Nesting ($Nesting+1) -InputObject $f -Size $f.Length
            }
        }
        
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
