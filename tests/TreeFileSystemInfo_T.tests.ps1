$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([System.IO.Path]::Combine($PSScriptRoot, 'shared.psm1'))

Describe 'TreeFileSystemInfo<T>' {
    BeforeAll {
        $standardProperties = @(
            'Name'
            'Mode'
            'FullName'
            'Extension'
            'Attributes'
            'Length'
            'CreationTime'
            'CreationTimeUtc'
            'LastWriteTime'
            'LastWriteTimeUtc'
        )

        $standardProperties | Out-Null
    }

    It 'Can determine if an instance is a Directory or File with its .HasFlag() method' {
        $testPath | Get-PSTree | ForEach-Object {
            if ($_.HasFlag([System.IO.FileAttributes]::Directory)) {
                $_ | Should -BeOfType ([PSTree.TreeDirectory])
                return
            }

            $_ | Should -BeOfType ([PSTree.TreeFile])
        }
    }

    It 'Shares same properties as FileSystemInfo' {
        $instance = $testPath | Get-PSTree |
            Where-Object { $_ -is [PSTree.TreeFile] } |
            Select-Object -First 1

        $instance | Should -HaveCount 1

        $item = $instance | Get-Item

        foreach ($property in $standardProperties) {
            $instance.$property | Should -BeExactly ($item.$property)
        }
    }

    It 'Can refresh the state of the object' {
        { $testPath | Get-PSTree | ForEach-Object Refresh } |
            Should -Not -Throw
    }

    It 'ToString() should resolve to the instance FullName property' {
        $testPath | Get-PSTree | ForEach-Object {
            $_.ToString() | Should -BeExactly $_.FullName
        }
    }

    It 'AccessTime properties should be DateTime' {
        $instance = Get-PSTree -Depth 0
        $instance.LastAccessTime | Should -BeOfType ([datetime])
        $instance.LastAccessTimeUtc | Should -BeOfType ([datetime])
        $instance.LastWriteTime | Should -BeOfType ([datetime])
        $instance.LastWriteTimeUtc | Should -BeOfType ([datetime])
    }

    It 'GetFormattedLength() outputs friendly length' {
        $pattern = '(?:{0}$)' -f ('B', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB' -join '|')
        (Get-PSTree $testPath).GetFormattedLength() | Should -Match $pattern
    }

    It 'GetUnderlyingObject() Method' {
        Get-PSTree $testPath | ForEach-Object GetUnderlyingObject |
            Should -BeOfType ([System.IO.FileSystemInfo])
    }
}
