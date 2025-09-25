using namespace System.IO
using namespace System.Management.Automation
using namespace System.Security
using namespace Microsoft.Win32

$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([Path]::Combine($PSScriptRoot, 'shared.psm1'))

if (!$isWin) {
    Describe 'Get-PSTreeRegistry.NonWindows' {
        It 'Should throw a PlatformNotSupportedException on Non-Windows Platforms' {
            { Get-PSTreeRegistry HKCU:\ } | Should -Throw -ExceptionType ([PlatformNotSupportedException])
        }
    }

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

        It 'Ignores -Recurse if -Depth is used' {
            Get-PSTreeRegistry -Path HKCU:\ -Depth 0 -Recurse |
                Should -HaveCount 1
        }

        It 'Limits recursion with Depth parameter' {
            $withDepth = Get-PSTreeRegistry -Path HKCU:\ -Depth 2 -ErrorAction SilentlyContinue
            $deep = Get-PSTreeRegistry -Path HKCU:\ -Recurse -ErrorAction SilentlyContinue
            $deep.Count | Should -BeGreaterThan $withDepth.Count
            $maxDepth = ($withDepth | Measure-Object -Property Depth -Maximum).Maximum
            $maxDepth | Should -BeExactly 2
        }

        It 'Displays only TreeRegistryKey with -KeysOnly' {
            Get-PSTreeRegistry -Path HKCU:\ -KeysOnly |
                Should -BeOfType ([PSTree.TreeRegistryKey])
        }

        It 'Can throw if non-elevated' {
            { Get-PSTreeRegistry -Path HKLM:\ } |
                Should -Throw -ExceptionType ([SecurityException])

            { Get-PSTreeRegistry HKLM:\SECURITY } |
                Should -Throw -ExceptionType ([SecurityException])
        }

        It 'Excludes child items with -Exclude parameter' {
            $exclude = '*a*', '*net*', '*sys*'
            Get-PSTreeRegistry HKCU:\ -Exclude * | Should -HaveCount 1
            Get-PSTreeRegistry HKCU:\ -Exclude $exclude -Recurse | ForEach-Object {
                [System.Linq.Enumerable]::Any(
                    [string[]] $exclude,
                    [System.Func[string, bool]] { $_.Name -like $args[0] })
            } | Should -Not -BeTrue
        }

        It 'Includes child items with -Include parameter' {
            $include = '*sys*', '*user*'
            Get-PSTreeRegistry HKCU:\ -Include $include | ForEach-Object {
                [System.Linq.Enumerable]::Any(
                    [string[]] $include,
                    [System.Func[string, bool]] {
                        $_.Name -like $args[0] -or $_ -is [PSTree.TreeRegistryKey]
                    }
                )
            } | Should -BeTrue
        }
    }

    Context 'Parameter Validation' {
        It 'Throws on invalid registry path' {
            { Get-PSTreeRegistry -Path HKCU:\DoesNotExist } |
                Should -Throw -ExceptionType ([ItemNotFoundException])

            { Get-PSTreeRegistry -LiteralPath HKCU:\DoesNotExist } |
                Should -Throw -ExceptionType ([ItemNotFoundException])
        }

        It 'Throws on invalid provider path' {
            { Get-PSTreeRegistry -Path Function:\* } |
                Should -Throw -ExceptionType ([ArgumentException])

            { Get-PSTreeRegistry -LiteralPath Function:\Clear-Host } |
                Should -Throw -ExceptionType ([ArgumentException])

            Get-PSTreeRegistry -Path Function:\* -EA 0 | Should -BeNullOrEmpty
        }

        It 'Accepts pipeline input' {
            'HKCU:\*' | Get-PSTreeRegistry | Should -Not -BeNullOrEmpty
            Get-Item 'HKCU:\' | Get-PSTreeRegistry | Should -Not -BeNullOrEmpty
            Get-PSTreeRegistry HKCU: -Depth 0 | Get-PSTreeRegistry | Should -Not -BeNullOrEmpty
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

    Context 'Output Types' {
        It 'PSTreeRegistryKey has expected properties' {
            $key = Get-PSTreeRegistry -Path 'HKLM:\Software' -KeysOnly -EA 0 |
                Select-Object -First 1

            $key.Kind | Should -BeExactly RegistryKey
            $key.SubKeyCount | Should -Not -BeNullOrEmpty
            $key.ValueCount | Should -Not -BeNullOrEmpty
            $key.View | Should -BeOfType ([RegistryView])
            $key.Path | Should -Not -BeNullOrEmpty
            $key.PSPath | Should -Not -BeNullOrEmpty
            $key.PSParentPath | Should -BeOfType ([string])
            $key.Hierarchy | Should -Not -BeNullOrEmpty
            $key.Depth | Should -BeGreaterOrEqual 0
            $key.LastWriteTime | Should -BeOfType ([datetime])
        }

        It 'PSTreeRegistryValue has expected properties' {
            $value = Get-PSTreeRegistry -Path 'HKLM:\Software' -EA 0 |
                Where-Object { $_ -is [PSTree.TreeRegistryValue] } |
                Select-Object -First 1

            $value.Kind | Should -BeOfType ([RegistryValueKind])
            $value.Name | Should -Not -BeNullOrEmpty
            $value.Path | Should -BeNullOrEmpty
            $value.PSPath | Should -BeNullOrEmpty
            $value.PSParentPath | Should -Not -BeNullOrEmpty
            $value.Hierarchy | Should -Not -BeNullOrEmpty
            $value.Depth | Should -BeGreaterOrEqual 0
        }

        It 'PSTreeRegistryValue has GetValue()' {
            Get-PSTreeRegistry -Path 'HKLM:\Software' -EA 0 |
                Where-Object { $_ -is [PSTree.TreeRegistryValue] } |
                ForEach-Object GetValue | Should -BeOfType ([object])
        }

        It 'Should be able to Cancel the cmdlet' {
            Measure-Command {
                $ps = [powershell]::Create().AddScript({
                    Import-Module $args[0]

                    $roots = Get-PSDrive |
                        Where-Object { $_.Provider.Name -eq 'Registry' } |
                        ForEach-Object { 'Registry::' + $_.Root }

                    Get-PSTreeRegistry $roots -Recurse -ErrorAction SilentlyContinue
                }).AddArgument($manifestPath)

                $task = $ps.BeginInvoke()
                Start-Sleep 0.5
                $ps.Stop()
                try { $ps.EndInvoke($task) } catch { }
            } | Should -BeLessThan ([timespan] '00:00:02')
        }
    }
}
