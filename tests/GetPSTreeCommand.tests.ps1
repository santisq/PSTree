$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([System.IO.Path]::Combine($PSScriptRoot, 'shared.psm1'))

Describe 'Get-PSTree' {
    BeforeAll {
        $newItemSplat = @{
            ItemType = 'Directory'
            Force    = $true
            Path     = Join-Path $TestDrive 'testfolder'
        }

        $testHiddenFolder = New-Item @newItemSplat

        1..10 | ForEach-Object {
            $folderSplat = @{
                ItemType = 'Directory'
                Force    = $true
                Path     = Join-Path $testHiddenFolder.FullName ('.HiddenFolder{0:D2}' -f $_)
            }
            $newFolder = New-Item @folderSplat

            $fileSplat = @{
                ItemType = 'File'
                Force    = $true
                Path     = Join-Path $newFolder.FullName ('.HiddenFile{0:D2}' -f $_)
            }

            $newFolder
            New-Item @fileSplat
        } | ForEach-Object {
            $_.Attributes = $_.Attributes -bor [System.IO.FileAttributes]::Hidden
        }
    }

    It '-Path should throw on a non existing path' {
        { Get-PSTree -Path doesnotexist } | Should -Throw
    }

    It '-LiterPath should throw on a non existing path' {
        { Get-PSTree -LiteralPath doesnotexist } | Should -Throw
    }

    It '-Path should throw on invalid Provider Path' {
        { Get-PSTree -Path function: } | Should -Throw
    }

    It '-LiteralPath should throw on invalid Provider Path' {
        { Get-PSTree -LiteralPath function: } | Should -Throw
    }

    It 'Should continue processing when multiple paths are given as input' {
        $getPSTreeSplat = @{
            LiteralPath = 'function:', $testPath, 'doesnotexist', $pwd
            ErrorAction = 'SilentlyContinue'
        }

        Get-PSTree @getPSTreeSplat |
            Should -Not -BeNullOrEmpty

        $getPSTreeSplat = @{
            Path        = 'function:', $testPath, 'doesnotexist', $pwd
            ErrorAction = 'SilentlyContinue'
        }

        Get-PSTree @getPSTreeSplat |
            Should -Not -BeNullOrEmpty
    }

    It '-Path should support wildcards' {
        Get-PSTree * | Should -Not -BeNullOrEmpty
    }

    It 'Can take pipeline input to -Path' {
        (Get-ChildItem $testPath).FullName | Get-PSTree |
            Should -Not -BeNullOrEmpty
    }

    It 'Can take pipeline input to -LiteralPath' {
        Get-ChildItem $testPath |
            Get-PSTree | Should -Not -BeNullOrEmpty
    }

    It 'Outputs PSTreeFile and PSTreeDirectory Instances' {
        Get-PSTree -LiteralPath $testPath |
            ForEach-Object GetType |
            Should -BeIn ([PSTree.PSTreeFile], [PSTree.PSTreeDirectory])
    }

    It 'Excludes PSTreeFile instances with -Directory' {
        Get-PSTree -LiteralPath $testPath -Directory |
            Should -BeOfType ([PSTree.PSTreeDirectory])
    }

    It 'Controls recursion with -Depth parameter' {
        $tree = Get-PSTree $testPath -Depth 1
        $tree | ForEach-Object Depth |
            Should -Not -BeGreaterThan 2

        $ref = (Get-ChildItem $testPath -Directory | Get-ChildItem).FullName
        $tree.FullName | Should -Not -Contain $ref
    }

    It 'Excludes child items with -Exclude parameter' {
        $exclude = '*tools*', '*build*', '*.ps1'
        Get-PSTree $testPath -Exclude * | Should -HaveCount 1
        Get-PSTree $testPath -Exclude $exclude -Recurse | ForEach-Object {
            [System.Linq.Enumerable]::Any(
                [string[]] $exclude,
                [System.Func[string, bool]] { $_.FullName -like $args[0] })
        } | Should -Not -BeTrue

        Get-ChildItem $testPath -Filter *.ps1 -Recurse |
            Get-PSTree -Exclude *.ps1 |
            Should -BeNullOrEmpty
    }

    It 'Includes child items with -Include parameter' {
        $include = '*.ps1', '*.cs'
        Get-PSTree $testPath -Include $include -Recurse | ForEach-Object {
            [System.Linq.Enumerable]::Any(
                [string[]] $include,
                [System.Func[string, bool]] {
                    $_.FullName -like $args[0] -or $_ -is [PSTree.PSTreeDirectory]
                }
            )
        } | Should -BeTrue

        Get-ChildItem $testPath -Filter *.ps1 -Recurse |
            Get-PSTree -Include *.ps1 |
            Should -Not -BeNullOrEmpty
    }

    It 'Should prioritize -Depth if used together with -Recurse' {
        $ref = (Get-ChildItem $testPath -Directory | Get-ChildItem -Recurse).FullName
        Get-PSTree $testPath -Directory -Recurse -Depth 1 |
            ForEach-Object FullName |
            Should -Not -Contain $ref
    }

    It 'Calculates the recursive size of folders with -RecursiveSize' {
        $recursiveLengths = Join-Path $testPath src |
            Get-PSTree -Directory -Depth 1 -RecursiveSize

        $lengths = Join-Path $testPath src |
            Get-PSTree -Directory -Depth 1

        $recursiveLengths | Should -HaveCount $lengths.Count

        for ($i = 0; $i -lt $recursiveLengths.Length; $i++) {
            $recursiveLengths[$i].Length |
                Should -BeGreaterThan $lengths[$i].Length
        }
    }

    It '-RecursiveSize and -Include can work together' {
        $measure = Get-ChildItem -Recurse -Include *.cs, *.ps1 |
            Measure-Object Length -Sum

        Get-PSTree -Include *.cs, *.ps1 -RecursiveSize -Depth 0 |
            ForEach-Object Length |
            Should -BeExactly $measure.Sum
    }

    It 'Should traverse all tree when -Recurse is used' {
        (Get-PSTree $testPath -Recurse | Select-Object -Skip 1 | Measure-Object).Count |
            Should -BeExactly (Get-ChildItem $testPath -Recurse | Measure-Object).Count
    }

    It 'Gets hidden files and folders with -Force switch' {
        $testHiddenFolder | Get-PSTree -Recurse | Should -HaveCount 1
        $testHiddenFolder | Get-PSTree -Recurse -Force |
            Should -HaveCount 21
    }
}
