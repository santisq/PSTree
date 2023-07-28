$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([System.IO.Path]::Combine($PSScriptRoot, 'shared.psm1'))

Describe 'Formatting internals' {
    It 'Converts Length to their friendly representation' {
        [PSTree.Internal._Format]::GetFormattedLength(1mb) |
            Should -BeExactly '1.00 MB'
    }

    It 'Can get the source of the generated trees' {
        $testPath | Get-PSTree | ForEach-Object {
            [PSTree.Internal._Format]::GetSource($_) | Should -BeExactly $testPath
        }
    }
}
