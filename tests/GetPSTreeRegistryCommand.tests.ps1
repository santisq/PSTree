$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath

Describe 'Get-PSTreeRegistry' {
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

    # Context 'Output Properties' {
    #     It 'PSTreeRegistryKey has expected properties' {
    #         $key = Get-PSTreeRegistry -Path 'HKLM:\Software' |
    #             Where-Object { $_ -is [PSTree.PSTreeRegistryKey] } | Select-Object -First 1
    #         $key.PSObject.Properties.Name | Should -Contain 'Name'
    #         $key.PSObject.Properties.Name | Should -Contain 'Path'
    #         $key.PSObject.Properties.Name | Should -Contain 'SubKeyCount'
    #         $key.PSObject.Properties.Name | Should -Contain 'ValueCount'
    #         $key.PSObject.Properties.Name | Should -Contain 'Depth'
    #         $key.PSObject.Properties.Name | Should -Contain 'LastWriteTime'
    #     }

    #     It 'PSTreeRegistryValue has expected properties' {
    #         $value = Get-PSTreeRegistry -Path 'HKLM:\Software' -Depth 0 |
    #             Where-Object { $_ -is [PSTree.PSTreeRegistryValue] } | Select-Object -First 1
    #         $value.PSObject.Properties.Name | Should -Contain 'Name'
    #         $value.PSObject.Properties.Name | Should -Contain 'Path'
    #         $value.PSObject.Properties.Name | Should -Contain 'Value'
    #         $value.PSObject.Properties.Name | Should -Contain 'Kind'
    #         $value.PSObject.Properties.Name | Should -Contain 'Depth'
    #     }
    # }

    # Context 'Force Parameter' {
    #     It 'Runs without error when Force is specified' {
    #         $result = Get-PSTreeRegistry -Path 'HKLM:\Software' -Force
    #         $result | Should -Not -BeNullOrEmpty
    #         # Add more if Force has a specific effect later
    #     }
    # }

    # Context 'Error Handling' {
    #     It 'Silently skips inaccessible keys' {
    #         # HKLM:\SECURITY often requires elevated perms
    #         $result = Get-PSTreeRegistry -Path 'HKLM:\SECURITY' -ErrorAction SilentlyContinue
    #         $result | Should -BeNullOrEmpty # Adjust if it returns partial results
    #     }
    # }
}
