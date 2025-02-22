$testPath = Split-Path $PSScriptRoot
$isWin = [System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform(
    [System.Runtime.InteropServices.OSPlatform]::Windows)

$testPath, $isWin | Out-Null
Export-ModuleMember -Variable testPath, isWin
