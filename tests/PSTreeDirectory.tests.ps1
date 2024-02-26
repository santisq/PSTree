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

    It 'Can enumerate its parent directories with .GetParents()' {
        $treedir = $testPath | Get-PSTree -Depth 0
        $parent = $treedir.Parent
        $parent | Should -BeOfType ([System.IO.DirectoryInfo])
        $paths = $parent.FullName.Split([System.IO.Path]::DirectorySeparatorChar)
        $treedir.GetParents() | Should -HaveCount $paths.Count
    }
}
