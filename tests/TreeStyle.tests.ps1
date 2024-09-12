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
        $style, $escape | Out-Null

        $path = Join-Path $TestDrive 'teststyle'
        '.exe', '.ps1' | New-Item -Path { Join-Path $path "file$_" } -Force | Out-Null
    }

    It 'Directory property can be set' {
        $style.Directory | Should -Not -BeNullOrEmpty
        { $style.Directory = 'Invalid' } | Should -Throw
        { $style.Directory = $style.Palette.Background.BrightGreen } | Should -Not -Throw
    }

    It 'Executable property can be set' {
        $style.Executable | Should -Not -BeNullOrEmpty
        { $style.Executable = 'Invalid' } | Should -Throw
        { $style.Executable = $style.Palette.Background.BrightGreen } | Should -Not -Throw
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
        { $extension['.ps1'] = 'Invalid' } | Should -Throw
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

