$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', '..', 'module', '*.psd1'))).BaseName
$manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', '..', 'output', $moduleName)
Import-Module $manifestPath

Describe 'TreeStyle' {
    BeforeAll {
        $style = [PSTree.Style.TreeStyle]::Instance
        $style, $escape | Out-Null

        $path = Join-Path $pwd 'teststyle'
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
        Get-PSTree $TestDrive -Recurse | ForEach-Object Hierarchy |
            Should -Match '^\x1B\[(?:[0-9]+;?){1,}m'

        $style.OutputRendering = [PSTree.Style.OutputRendering]::PlainText

        Get-PSTree $TestDrive -Recurse | ForEach-Object Hierarchy |
            Should -Not -Match '^\x1B\[(?:[0-9]+;?){1,}m'
    }
}
