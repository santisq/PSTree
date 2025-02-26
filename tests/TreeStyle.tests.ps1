$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)
Import-Module $manifestPath

Describe 'GetPSTreeStyle Command' {
    It 'Outputs a session TreeStyle instance' {
        Get-PSTreeStyle | Should -BeOfType ([PSTree.Style.TreeStyle])
    }
}

Describe 'TreeStyle Type' {
    BeforeAll {
        $style = [PSTree.Style.TreeStyle]::Instance
        $style | Out-Null
    }

    It 'CombineSequence() can combine 2 VT Sequences' {
        $vt = $style.CombineSequence(
            $style.Palette.Background.BrightRed,
            $style.Palette.Foreground.White)
        $vt | Should -Match '^\x1B\[101;37m$'
    }

    It 'ResetSettings() resets TreeStyle to its initial state' {
        { $style.ResetSettings() } | Should -Not -Throw
    }

    It 'ToBold() adds bold accent' {
        $style.ToBold($style.Palette.Background.White) | Should -Match ';1m$'
    }

    It 'ToItalic() adds italic accent' {
        $style.ToItalic($style.Palette.Background.White) | Should -Match ';3m$'
    }
}

Describe 'FileSystemStyle Type' {
    BeforeAll {
        $style = [PSTree.Style.TreeStyle]::Instance
        $style | Out-Null

        $path = Join-Path $TestDrive 'teststyle'
        '.exe', '.ps1' | New-Item -Path { Join-Path $path "file$_" } -Force | Out-Null
    }

    It 'Directory property can be set' {
        $style.FileSystem.Directory | Should -Not -BeNullOrEmpty
        { $style.FileSystem.Directory = 'Invalid' } | Should -Throw
        { $style.FileSystem.Directory = $style.Palette.Background.BrightGreen } | Should -Not -Throw
    }

    It 'Executable property can be set' {
        $style.FileSystem.Executable | Should -Not -BeNullOrEmpty
        { $style.FileSystem.Executable = 'Invalid' } | Should -Throw
        { $style.FileSystem.Executable = $style.Palette.Background.BrightGreen } | Should -Not -Throw
    }

    It 'OutputRendering defines if output is colored' {
        $style.OutputRendering = [PSTree.Style.OutputRendering]::Host

        Get-PSTree $TestDrive -Recurse -Include *.ps1, *.exe | ForEach-Object {
            if ($_.Extension -eq '.exe' -and $IsLinux) {
                return
            }

            $_.Hierarchy | Should -Match '\b\x1B\[(?:[0-9]+;?){1,}m'
        }

        $style.OutputRendering = [PSTree.Style.OutputRendering]::PlainText

        Get-PSTree $TestDrive -Recurse -Include *.ps1, *.exe | ForEach-Object Hierarchy |
            Should -Not -Match '\b\x1B\[(?:[0-9]+;?){1,}m'
    }
}

if ($isWin) {
    Describe 'RegistryStyle Type' {
        BeforeAll {
            $style = [PSTree.Style.TreeStyle]::Instance
            $style | Out-Null
        }

        It 'RegistryKey property can be set' {
            $style.Registry.RegistryKey | Should -Not -BeNullOrEmpty
            { $style.Registry.RegistryKey = 'Invalid' } | Should -Throw
            { $style.Registry.RegistryKey = $style.Palette.Background.BrightGreen } | Should -Not -Throw
        }

        It 'OutputRendering defines if output is colored' {
            $style.OutputRendering = [PSTree.Style.OutputRendering]::Host
            Get-PSTreeRegistry HKCU:\ -KeysOnly | ForEach-Object Hierarchy |
                Should -Match '\s?\x1B\[(?:[0-9]+;?){1,}m'

            $style.OutputRendering = [PSTree.Style.OutputRendering]::PlainText
            Get-PSTreeRegistry HKCU:\ -KeysOnly | ForEach-Object Hierarchy |
                Should -Not -Match '\s?\x1B\[(?:[0-9]+;?){1,}m'

            $style.OutputRendering = [PSTree.Style.OutputRendering]::Host
        }
    }

    Describe 'ValueKind Type' {
        BeforeAll {
            $style = [PSTree.Style.TreeStyle]::Instance
            $kind = $style.Registry.RegistryValueKind
            $escape = "$([char] 27)"
            $kind, $escape | Out-Null
        }

        It 'Keys should be empty' {
            $kind.Keys | Should -BeNullOrEmpty
        }

        It 'Values should be empty' {
            $kind.Values | Should -BeNullOrEmpty
        }

        It 'Supports setting RegistryValueKind sequence by index' {
            { $kind['String'] = "$escape[33;1m" } | Should -Not -Throw
            { $kind['String'] = 'Invalid' } | Should -Throw
            { $kind['Invalid'] = "$escape[33;1m" } | Should -Throw
        }

        It 'Supports getting extension sequence by index' {
            $kind['String'] | Should -BeExactly "$escape[33;1m"
        }


        It 'ToString() should not be empty' {
            $kind.ToString() | Should -Not -BeNullOrEmpty
        }

        It 'GetEscapedValues() should not be empty' {
            $kind.GetEscapedValues() | Should -Not -BeNullOrEmpty
        }

        It 'ContainsKey() checks if a extension exists' {
            $kind.ContainsKey('String') | Should -BeTrue
            $kind.ContainsKey('ExpandString') | Should -BeFalse
            { $kind.ContainsKey('NotExist') } | Should -Throw
        }

        It 'Add() adds a new extension' {
            { $kind.Add('ExpandString', "$escape[33;1m") } | Should -Not -Throw
            { $kind.Add('Invalid', "$escape[33;1m") } | Should -Throw
            { $kind.Add('DWord', 'Invalid') } | Should -Throw
            { $kind.Add('DWord', "$escape[33;1m") } | Should -Not -Throw
        }

        It 'Adds coloring to TreeRegistryValue instances' {
            $style.OutputRendering = [PSTree.Style.OutputRendering]::Host

            Get-PSTreeRegistry HKCU:\ -EA 0 |
                Where-Object Kind -In DWord, ExpandString, String |
                Select-Object -ExpandProperty Hierarchy -First 10 |
                Should -Match '\s?\x1B\[(?:[0-9]+;?){1,}m'
        }

        It 'Remove() removes an existing extension' {
            $kind.Remove('DWord') | Should -BeTrue
            $kind.Remove('DWord') | Should -BeFalse
        }

        It 'Clear() clears the internal dictionary' {
            $kind.Clear()
            $kind.Count | Should -BeExactly 0
        }
    }
}

Describe 'Palette Type' {
    BeforeAll {
        $palette = [PSTree.Style.TreeStyle]::Instance.Palette
        $palette | Out-Null
    }

    It 'Foreground Type' {
        $palette.Foreground.ToString() | Should -Not -BeNullOrEmpty
        $palette.Foreground.PSObject.Properties.Value |
            Should -Match '^\x1B\[(?:[0-9]+;?){1,}m$'
    }

    It 'Background Type' {
        $palette.Background.ToString() | Should -Not -BeNullOrEmpty
        $palette.Background.PSObject.Properties.Value |
            Should -Match '^\x1B\[(?:[0-9]+;?){1,}m$'
    }

    It 'Palette Type' {
        $palette.ToString() | Should -Not -BeNullOrEmpty
    }
}

Describe 'Extension Type' {
    BeforeAll {
        $extension = [PSTree.Style.TreeStyle]::Instance.FileSystem.Extension
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
        { $extension['.ps1'] = 'Invalid' } | Should -Throw
        { $extension['Invalid'] = "$escape[33;1m" } | Should -Throw
    }

    It 'ToString() should not be empty' {
        $extension.ToString() | Should -Not -BeNullOrEmpty
    }

    It 'GetEscapedValues() should not be empty' {
        $extension.GetEscapedValues() | Should -Not -BeNullOrEmpty
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
