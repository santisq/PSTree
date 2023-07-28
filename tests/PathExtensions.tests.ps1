$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([System.IO.Path]::Combine($PSScriptRoot, 'shared.psm1'))


Describe 'PathExtensions' {
    BeforeAll {
        $normalizePath = (Get-PSTree -Depth 0).GetType().
            Assembly.
            GetType('PSTree.PathExtensions').
            GetMethod(
                'NormalizePath',
                [System.Reflection.BindingFlags] 'NonPublic, Static',
                $null,
                [type[]] ([string], [bool], [System.Management.Automation.PSCmdlet], [bool], [bool]),
                $null)

        $normalizePath | Out-Null
    }

    It 'Should throw on Provider Paths' {
        { $normalizePath | Test-NormalizePath function: -IsLiteral } |
            Should -Throw

        { $normalizePath | Test-NormalizePath function: } |
            Should -Throw

        { $normalizePath | Test-NormalizePath function: -ThrowOnInvalidProvider } |
            Should -Throw
    }

    It 'Should throw on invalid Paths' {
        { $normalizePath | Test-NormalizePath \does\not\exist -IsLiteral } |
            Should -Throw

        { $normalizePath | Test-NormalizePath \does\not\exist } |
            Should -Throw

        { $normalizePath | Test-NormalizePath \does\not\exist -ThrowOnInvalidPath } |
            Should -Throw
    }
}
