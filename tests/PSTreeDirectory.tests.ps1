$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([System.IO.Path]::Combine($PSScriptRoot, 'shared.psm1'))

Describe 'PSTreeDirectory' {
    It 'Can enumerate Files with .EnumerateFiles()' {
        ($testPath | Get-PSTree -Depth 0).EnumerateFiles() |
            Should -BeOfType ([System.IO.FileInfo])
    }

    It 'Can enumerate Directories with .EnumerateDirectories()' {
        ($testPath | Get-PSTree -Depth 0).EnumerateDirectories() |
            Should -BeOfType ([System.IO.DirectoryInfo])
    }

    It 'Can enumerate File System Infos with .EnumerateFileSystemInfos()' {
        ($testPath | Get-PSTree -Depth 0).EnumerateFileSystemInfos() |
            ForEach-Object GetType |
            Should -BeIn ([System.IO.FileInfo], [System.IO.DirectoryInfo])
    }

    It '.Parent property gets the PSTreeDirectory Parent DirectoryInfo instance' {
        (Get-PSTree $testPath -Depth 0).Parent | Should -BeOfType ([System.IO.DirectoryInfo])
    }

    It 'ItemCount gets the count of direct child items' {
        $childCount = @(Get-ChildItem -Force $testPath).Count
        (Get-PSTree $testPath -Depth 1 -Force)[0].ItemCount | Should -BeExactly $childCount
    }

    It 'TotalItemCount gets the recursive count of child items' {
        $childCount = @(Get-ChildItem -Force $testPath -Recurse).Count
        (Get-PSTree $testPath -Recurse -Force)[0].TotalItemCount | Should -BeExactly $childCount

        $childCount = @(Get-ChildItem $testPath -Depth 2).Count
        (Get-PSTree $testPath)[0].TotalItemCount | Should -BeExactly $childCount
    }
}
