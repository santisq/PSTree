$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([System.IO.Path]::Combine($PSScriptRoot, 'shared.psm1'))

if (!$isWin) {
    return
}

Describe 'Get-PSTreeRegistry.Windows' {
    Context 'Basic Functionality' {
        It 'Returns registry keys and registry values from a valid path' {
            Get-PSTreeRegistry HKCU:\ |
                ForEach-Object GetType |
                Should -BeIn ([PSTree.TreeRegistryKey], [PSTree.TreeRegistryValue])
        }

        It 'Returns a single Key when Depth is 0' {
            Get-PSTreeRegistry -Path HKCU:\ -Depth 0 |
                Should -HaveCount 1
        }

        It 'Limits recursion with Depth parameter' {
            $shallow = Get-PSTreeRegistry -Path HKCU:\ -Depth 1
            $deep = Get-PSTreeRegistry -Path HKCU:\ -Depth 2
            $deep.Count | Should -BeGreaterThan $shallow.Count
            $maxDepth = ($deep | Measure-Object -Property Depth -Maximum).Maximum
            $maxDepth | Should -BeExactly 2
        }
    }

    Context 'Parameter Validation' {
        It 'Throws on invalid registry path' {
            { Get-PSTreeRegistry -Path HKCU:\DoesNotExist } |
                Should -Throw -ExceptionType ([System.Management.Automation.ItemNotFoundException])

            { Get-PSTreeRegistry -LiteralPath HKCU:\DoesNotExist } |
                Should -Throw -ExceptionType ([System.Management.Automation.ItemNotFoundException])
        }

        It 'Accepts pipeline input' {
            'HKCU:\*' | Get-PSTreeRegistry | Should -Not -BeNullOrEmpty
            Get-Item 'HKCU:\' | Get-PSTreeRegistry | Should -Not -BeNullOrEmpty
        }

        It 'Handles multiple paths' {
            $paths = Get-Item 'HKCU:\Software\*' | Select-Object -ExpandProperty PSPath -First 2
            $result = Get-PSTreeRegistry -Path $paths
            $result.PSPath | Should -Contain $paths[0]
            $result.PSPath | Should -Contain $paths[1]

            $result = Get-PSTreeRegistry -LiteralPath $paths
            $result.PSPath | Should -Contain $paths[0]
            $result.PSPath | Should -Contain $paths[1]
        }
    }

    Context 'Output Properties' {
        It 'PSTreeRegistryKey has expected properties' {
            $key = Get-PSTreeRegistry -Path 'HKLM:\Software' -EA 0 |
                Where-Object { $_ -is [PSTree.TreeRegistryKey] } |
                Select-Object -First 1

            $key.Kind | Should -BeExactly RegistryKey
            $key.SubKeyCount | Should -Not -BeNullOrEmpty
            $key.ValueCount | Should -Not -BeNullOrEmpty
            $key.View | Should -BeOfType ([Microsoft.Win32.RegistryView])
            $key.Path | Should -Not -BeNullOrEmpty
            $key.PSPath | Should -Not -BeNullOrEmpty
            $key.PSParentPath | Should -BeOfType ([string])
            $key.Hierarchy | Should -Not -BeNullOrEmpty
            $key.Depth | Should -BeGreaterOrEqual 0
        }

        It 'PSTreeRegistryValue has expected properties' {
            $value = Get-PSTreeRegistry -Path 'HKLM:\Software' -EA 0 |
                Where-Object { $_ -is [PSTree.TreeRegistryValue] } |
                Select-Object -First 1

            $value.Kind | Should -BeOfType ([Microsoft.Win32.RegistryValueKind])
            $value.Name | Should -Not -BeNullOrEmpty
            $value.Path | Should -BeNullOrEmpty
            $value.PSPath | Should -BeNullOrEmpty
            $value.PSParentPath | Should -Not -BeNullOrEmpty
            $value.Hierarchy | Should -Not -BeNullOrEmpty
            $value.Depth | Should -BeGreaterOrEqual 0
        }
    }
}
