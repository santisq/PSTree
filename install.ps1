$ErrorActionPreference = 'Stop'
$VerbosePreference = 'Continue'
$ProgressPreference = 'SilentlyContinue'

Write-Verbose "Installing PSTree Module... Please wait a moment."
Write-Verbose "PSTree default Scope is 'CurrentUser'.`n" 

$installPath = (
    "$HOME/.local/share/powershell/Modules",
    "$HOME\Documents\PowerShell\Modules"
)[$IsWindows]


Write-Verbose "Module will be installed on:"
Write-Verbose "$installPath`n"

if(-not(Test-Path $installPath))
{
    Write-Verbose "Module folder could not be found. Creating it..."

    $path = New-Item $installPath -ItemType Directory -Force
    Write-Verbose "Module folder has been created:"
    Write-Verbose "$($path.FullName)`n"
}

Write-Verbose "Checking dependencies..."

$structure = @(
    'DrawHierarchy.ps1',
    'Get-FolderRecursive.ps1',
    'Indent.ps1',
    'SizeConvert.ps1' | ForEach-Object {
        [System.IO.Path]::Combine('PSTree', 'private', $_)
    }
    
    [System.IO.Path]::Combine(
        'PSTree', 'public' , 'Get-PSTree.ps1'
    )
    [System.IO.Path]::Combine(
        'PSTree', 'PSTree.psm1'
    )
    [System.IO.Path]::Combine(
        'PSTree', 'PSTree.psd1'
    )

).ForEach({ Join-Path $PSScriptRoot -ChildPath $_ })

$shouldDownload = foreach($file in $structure)
{
    if(-not (Test-Path $file))
    {
        $true
        break
    }
}

$sourcePath = Join-Path $PSScriptRoot -ChildPath PSTree

if($shouldDownload)
{
    $download = Join-Path $PSScriptRoot -ChildPath "main.zip"
    Write-Verbose "Missing dependencies, attempting to download the module..."
    Invoke-WebRequest 'https://github.com/santysq/PSTree/archive/refs/heads/main.zip' -OutFile $download 4>$null
    Write-Verbose "Download successful... Preparing to install..."
    Write-Verbose "Extracting..."
    $expanded = Expand-Archive $download -DestinationPath $PSScriptRoot -Force -Verbose:$false -PassThru
    $expanded = $expanded.Where({$_.Name -eq 'PSTree.psm1'}).Directory
    Move-Item $expanded -Destination $PSScriptRoot -Force -Verbose:$false
    Remove-Item $expanded.Parent -Force -Recurse -Confirm:$false -Verbose:$false
}

Write-Verbose "Installing...`n"
Copy-Item $sourcePath -Destination $installPath -Verbose:$false -Force -Recurse

Import-Module PSTree -Force | Out-Host
Start-Sleep -Seconds 1
Write-Verbose "Installation Completed. 'Get-PSTree' is now ready for use!"
''
Get-PSTree $installPath -Depth 2 | Format-Table -AutoSize | Out-Host

'Press any Key to continue...'
$null = [console]::ReadKey()
