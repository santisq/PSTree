Describe 'PSTree Module' {
    BeforeAll {
        $ErrorActionPreference = 'Stop'
        $moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
        $manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)
        $testPath = Split-Path $PSScriptRoot

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
                Path     = Join-Path $testHiddenFolder.FullName ('HiddenFolder{0:D2}' -f $_)
            }
            $newFolder = New-Item @folderSplat

            $fileSplat = @{
                ItemType = 'File'
                Force    = $true
                Path     = Join-Path $newFolder.FullName ('HiddenFile{0:D2}' -f $_)
            }

            $newFolder
            New-Item @fileSplat
        } | ForEach-Object {
            $_.Attributes = $_.Attributes -bor [System.IO.FileAttributes]::Hidden
        }

        $testPath | Out-Null
        Import-Module $manifestPath -ErrorAction Stop
    }

    Context 'Get-PSTree' {
        It '-Path should throw on a non existing path' {
            { Get-PSTree -Path doesnotexist } | Should -Throw
        }

        It '-Path should support wildcards' {
            Get-PSTree * | Should -Not -BeNullOrEmpty
        }

        It '-LiterPath should throw on a non existing path' {
            { Get-PSTree -LiteralPath doesnotexist } | Should -Throw
        }

        It 'Can take pipeline input to -Path' {
        (Get-ChildItem $testPath).FullName |
                Get-PSTree | Should -Not -BeNullOrEmpty
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

        It 'Excludes PSTressFile instances with -Directory' {
            Get-PSTree -LiteralPath $testPath -Directory |
                Should -BeOfType ([PSTree.PSTreeDirectory])
        }

        It 'Controls recursion with -Depth parameter' {
            $method = [PSTree.PSTreeFileSystemInfo].GetProperty(
                'Depth',
                [System.Reflection.BindingFlags] 'NonPublic, Instance')

            $tree = Get-PSTree $testPath -Depth 1

            $tree | ForEach-Object { $method.GetValue($_) } |
                Should -Not -BeGreaterThan 2

            $ref = (Get-ChildItem $testPath -Directory | Get-ChildItem).FullName
            $tree.FullName | Should -Not -Contain $ref
        }

        It 'Excludes childs with -Exclude parameter' {
            $exclude = '*tools*', '*build*', '*.ps1'
            Get-PSTree $testPath -Exclude * | Should -HaveCount 1
            Get-PSTree $testPath -Exclude $exclude -Recurse | ForEach-Object {
                [System.Linq.Enumerable]::Any(
                    [string[]] $exclude,
                    [System.Func[string, bool]] { $_.FullName -like $args[0] })
            } | Should -Not -BeTrue
        }

        It 'Should prioritize -Depth if used together with -Recurse' {
            $ref = (Get-ChildItem $testPath -Directory | Get-ChildItem -Recurse).FullName
            Get-PSTree $testPath -Directory -Recurse -Depth 1 |
                ForEach-Object FullName |
                Should -Not -Contain $ref
        }

        It 'Calculates the recursive size of folders with -RecrusiveSize' {
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

    Context 'PSTreeFileSystemInfo<T>' {
        It 'Can determine if an instance is a Directory or File with its .HasFlag() method' {
            $testPath | Get-PSTree | ForEach-Object {
                if ($_.HasFlag([System.IO.FileAttributes]::Directory)) {
                    $_ | Should -BeOfType ([PSTree.PSTreeDirectory])
                    return
                }

                $_ | Should -BeOfType ([PSTree.PSTreeFile])
            }
        }
    }

    Context 'PSTreeDirectory' {
        BeforeAll {
            $parent = $testPath | Get-PSTree | Select-Object -First 1
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

    Context 'Formatting internals' {
        It 'Converts Length to their friendly representation' {
            [PSTree.Internal._Format]::GetFormattedLength(1mb) |
                Should -BeExactly '1.00 MB'
        }

        It 'Can get the source of the generated trees' {
            $testPath | Get-PSTree | ForEach-Object {
                [PSTree.Internal._Format]::GetSource($_) |
                    Should -BeExactly $testPath
            }
        }
    }
}
