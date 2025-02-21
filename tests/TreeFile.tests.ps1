$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([System.IO.Path]::Combine($PSScriptRoot, 'shared.psm1'))

Describe 'TreeFile' {
    It 'Has a reference of the parent DirectoryInfo instance' {
        $instance = $testPath | Get-PSTree | Where-Object { $_ -is [PSTree.TreeFile] } |
            Select-Object -First 1

        $instance.Directory | Should -BeOfType ([System.IO.DirectoryInfo])
    }

    It 'Has a reference of the parent directory path' {
        $instance = $testPath | Get-PSTree | Where-Object { $_ -is [PSTree.TreeFile] } |
            Select-Object -First 1

        $instance.DirectoryName | Should -BeExactly (Get-Item $instance.FullName).DirectoryName
    }
}
