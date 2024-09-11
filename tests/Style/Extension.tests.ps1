$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)
Import-Module $manifestPath

Describe 'Extension' {
    BeforeAll {
        $extension = [PSTree.Style.TreeStyle]::Instance.Extension
        $escape = "$([char] 27)"
        $extension, $escape | Out-Null
    }

    It 'Keys should not be empty' {
        $extension.Keys | Should -Not -BeNullOrEmpty
    }

    It 'Values should not be empty' {
        $extension.Values | Should -Not -BeNullOrEmpty
    }

    It 'Supports getting extension sequence by index' {
        $extension['.ps1'] | Should -BeExactly "$escape[33;1m"
    }

    It 'Supports setting extension sequence by index' {
        { $extension['.ps1'] = "$escape[33;1m" } | Should -Not -Throw
        { $extension['.ps1'] = "Invalid" } | Should -Throw
        { $extension['Invalid'] = "$escape[33;1m" } | Should -Throw
    }

    It 'ToString() should not be empty' {
        $extension.ToString() | Should -Not -BeNullOrEmpty
    }

    It 'GetEscapedValues() should not be empty' {
        $extension::GetEscapedValues($extension) | Should -Not -BeNullOrEmpty
    }

    It 'ContainsKey() checks if a extension exists' {
        $extension.ContainsKey('.ps1') | Should -BeTrue
        $extension.ContainsKey('NotExist') | Should -BeFalse
    }

    It 'Add() adds a new extension' {
        { $extension.Add('.ps1', "$escape[33;1m") } | Should -Throw
        { $extension.Add('Invalid', "$escape[33;1m") } | Should -Throw
        { $extension.Add('.ps2', 'Invalid') } | Should -Throw
        { $extension.Add('.ps2', "$escape[33;1m") } | Should -Not -Throw
    }

    It 'Remove() removes an existing extension' {
        $extension.Remove('.ps2') | Should -BeTrue
        $extension.Remove('.ps2') | Should -BeFalse
    }

    It 'Clear() clears the internal dictionary' {
        $extension.Clear()
        $extension.Count | Should -BeExactly 0
    }
}

