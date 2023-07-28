$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([System.IO.Path]::Combine($PSScriptRoot, 'shared.psm1'))

Describe 'PSTreeDirectory' {
    BeforeAll {
        $parent = $testPath | Get-PSTree -Depth 0
        $parent | Out-Null
    }

    It 'Can enumerate Files with .EnumerateFiles()' {
        $parent.EnumerateFiles() | Should -BeOfType ([System.IO.FileInfo])
    }

    It 'Can enumerate Directories with .EnumerateDirectories()' {
        $parent.EnumerateDirectories() | Should -BeOfType ([System.IO.DirectoryInfo])
    }

    It 'Can enumerate File System Infos with .EnumerateFileSystemInfos()' {
        $parent.EnumerateFileSystemInfos() | ForEach-Object GetType |
            Should -BeIn ([System.IO.FileInfo], [System.IO.DirectoryInfo])
    }

    It 'Can enumerate its parent directories with .GetParents()' {
        $paths = $parent.Parent.FullName.Split([System.IO.Path]::DirectorySeparatorChar)
        $parent.GetParents() | Should -HaveCount $paths.Count
    }
}
