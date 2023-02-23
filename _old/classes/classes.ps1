using namespace System.IO
using namespace System.Text
using namespace System.Collections.Generic

class PSTreeStatic {
    static [string] Indent ([string] $String, [Int64] $Indentation) {
        return "$('    ' * $Indentation)$String"
    }

    static [object[]] DrawHierarchy ([object[]] $InputObject, [string] $Property, [string] $Rec) {
        $corner, $horizontal, $pipe, $connector = '└', '─', '│', '├'

        $cornerConnector = "${corner}$(${horizontal}*2) "
        foreach($group in $InputObject | Group-Object $Rec | Select-Object -Skip 1) {
            foreach($item in $group.Group) {
                $item.$Property = $item.$Property -replace '\s{4}(?=\S)', $cornerConnector
            }
        }

        for($i = 1; $i -lt $InputObject.Count; $i++) {
            $index = $InputObject[$i].$Property.IndexOf($corner)
            if($index -ge 0) {
                $z = $i - 1
                while($InputObject[$z].$Property[$index] -notmatch "$corner|\S") {
                    $replace = $InputObject[$z].$Property.ToCharArray()
                    $replace[$Index] = $pipe
                    $InputObject[$z].$Property = [string]::new($replace)
                    $z--
                }

                if($InputObject[$z].$Property[$index] -eq $corner) {
                    $replace = $InputObject[$z].$Property.ToCharArray()
                    $replace[$Index] = $connector
                    $InputObject[$z].$Property = [string]::new($replace)
                }
            }
        }

        return $InputObject
    }
}

class PSTreeDirectory {
    [string] $Hierarchy
    [int64] $Length
    hidden [DirectoryInfo] $Instance
    hidden [int64] $Depth

    PSTreeDirectory() { }

    PSTreeDirectory([DirectoryInfo] $DirectoryInfo, [int64] $Depth) {
        $this.Instance  = $DirectoryInfo
        $this.Depth     = $Depth
        $this.Hierarchy = [PSTreeStatic]::Indent($DirectoryInfo.Name, $Depth)
    }

    [IEnumerable[FileSystemInfo]] EnumerateFileSystemInfos() {
        return $this.Instance.EnumerateFileSystemInfos()
    }

    [IEnumerable[DirectoryInfo]] EnumerateDirectories() {
        return $this.Instance.EnumerateDirectories()
    }

    [IEnumerable[FileInfo]] EnumerateFiles() {
        return $this.Instance.EnumerateFiles()
    }

    [string[]] GetParents([hashtable] $Index) {
        $parent  = $this.Instance.Parent
        $parents = while($parent -and $Index.ContainsKey($parent.FullName)) {
            $parent.FullName
            $parent = $parent.Parent
        }
        return $parents
    }

    [void] SetSize([Int64] $Length) {
        $this.Length = $Length
    }

    [void] AddSize([Int64] $Length) {
        $this.Length += $Length
    }

    [string] GetAbsolutePath() {
        return $this.Instance.FullName
    }

    [FileAttributes] GetAttributes() {
        return $this.Instance.Attributes
    }
}

class PSTreeFile {
    [string] $Hierarchy
    [int64] $Length
    hidden [FileInfo] $Instance
    hidden [int64] $Depth

    PSTreeFile([FileInfo] $FileInfo, [int64] $Depth) {
        $this.Instance  = $FileInfo
        $this.Depth     = $Depth
        $this.Hierarchy = [PSTreeStatic]::Indent($FileInfo.Name, $Depth)
        $this.Length    = $FileInfo.Length
    }
}