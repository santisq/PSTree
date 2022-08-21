$VerbosePreference = 'Continue'
$ProgressPreference = 'SilentlyContinue'
$ErrorActionPreference = 'Stop'

Write-Verbose "Installing PSTree Module... Please wait a moment."
Write-Verbose "PSTree default Scope is 'CurrentUser'.`n"

$installPath = $env:PSModulePath -split [System.IO.Path]::PathSeparator |
    Where-Object { $_.StartsWith([Environment]::GetFolderPath('MyDocuments')) } |
        Select-Object -First 1

Write-Verbose "Module will be installed on:"
Write-Verbose "$installPath`n"

if(-not(Test-Path $installPath)) {
    Write-Verbose "Module folder could not be found. Creating it..."
    $path = New-Item $installPath -ItemType Directory -Force
    Write-Verbose "Module folder has been created:"
    Write-Verbose "$($path.FullName)`n"
}

$downloadDestination = Join-Path $PWD.Path -ChildPath tempPSTree
$null = New-Item $downloadDestination -ItemType Directory -Force
$downloadZip = Join-Path $downloadDestination -ChildPath "PSTree-main.zip"
Write-Verbose "Downloading PSTree Module..."
Invoke-WebRequest 'https://github.com/santysq/PSTree/archive/refs/heads/main.zip' -OutFile $downloadZip 4>$null
Write-Verbose "Download successful... Preparing to install..."
Write-Verbose "Extracting..."
Expand-Archive $downloadZip -DestinationPath $downloadDestination -Force -Verbose:$false
Write-Verbose "Installing...`n"
$modulePath = Get-ChildItem $downloadDestination -Filter PSTree -Directory -Recurse
Copy-Item $modulePath.FullName -Destination $installPath -Verbose:$false -Force -Recurse
Remove-Item $downloadDestination -Force -Recurse 4>$null
Import-Module PSTree -Force | Out-Host
Start-Sleep -Seconds 1
Write-Verbose "Installation Completed. 'Get-PSTree' is now ready for use!"
''
Get-PSTree $installPath -Depth 2 | Format-Table -AutoSize | Out-Host

'Press any Key to continue...'
$Host.UI.ReadLine()