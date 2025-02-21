$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([System.IO.Path]::Combine($PSScriptRoot, 'shared.psm1'))

if ($isWin) {
    return
}

Describe 'Get-PSTreeRegistry.NonWindows' {
    It 'Should throw a PlatformNotSupportedException on Non-Windows Platforms' {
        { Get-PSTreeRegistry HKCU:\ } | Should -Throw -ExceptionType ([System.PlatformNotSupportedException])
    }
}
