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

$downloadDestination = Join-Path $PWD.Path -ChildPath tempPSTree
$null = New-Item $downloadDestination -ItemType Directory -Force
$downloadZip = Join-Path $downloadDestination -ChildPath "PSTree-main.zip"
Write-Verbose "Downloading PSTree Module..."
Invoke-WebRequest 'https://github.com/santysq/PSTree/archive/refs/heads/main-2.0.0.zip' -OutFile $downloadZip 4>$null
Write-Verbose "Download successful... Preparing to install..."
Write-Verbose "Extracting..."
$expanded = Expand-Archive $downloadZip -DestinationPath $downloadDestination -Force -Verbose:$false -PassThru
$expanded = $expanded.Where({$_.Name -eq 'PSTree.psm1'}).Directory
Write-Verbose "Installing...`n"
Copy-Item $expanded -Destination $installPath -Verbose:$false -Force -Recurse
Remove-Item $downloadDestination -Force -Recurse -Verbose:$false
Import-Module PSTree -Force | Out-Host
Start-Sleep -Seconds 1
Write-Verbose "Installation Completed. 'Get-PSTree' is now ready for use!"
''
Get-PSTree $installPath -Depth 2 | Format-Table -AutoSize | Out-Host

'Press any Key to continue...'
$null = [console]::ReadKey()
