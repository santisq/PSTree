function Test-NormalizePath {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory, ValueFromPipeline)]
        [System.Reflection.MethodInfo] $Method,

        [Parameter(Mandatory, Position = 0)]
        [string] $Path,

        [Parameter()]
        [switch] $IsLiteral,

        [Parameter()]
        [switch] $ThrowOnInvalidPath,

        [Parameter()]
        [switch] $ThrowOnInvalidProvider
    )

    $arguments = @(
        $Path
        $IsLiteral.IsPresent
        $PSCmdlet
        $ThrowOnInvalidPath.IsPresent
        $ThrowOnInvalidProvider.IsPresent
    )

    $Method.Invoke($null, $Arguments)
}

$testPath = Split-Path $PSScriptRoot
$isWin = [System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform(
    [System.Runtime.InteropServices.OSPlatform]::Windows)

$testPath, $isWin | Out-Null
Export-ModuleMember -Function Test-NormalizePath -Variable testPath, isWin
