using namespace System.IO
using namespace System.Collections.Generic
using namespace System.Text

class PSTreeStatic {
    static [string] Indent ([string] $String, [Int64] $Indentation) {
        $i = ' ' * 4
        return "$($i * $Indentation)$String"
    }

    static [void] DrawHierarchy ([object[]] $InputObject, [string] $Property, [string] $Rec) {
        # Had to do this because of Windows PowerShell Default Encoding
        # Not good at enconding stuff, probably a better way. Sorry for the ugliness :(
        $bytes = @(
            '226','148','148'
            '44','226','148'
            '128','44','226'
            '148','130','44'
            '226','148','156'
        )

        $corner, $horizontal, $pipe, $connector = [Encoding]::UTF8.GetString($bytes).Split(',')
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
                    $InputObject[$z].$Property = -join $replace
                    $z--
                }

                if($InputObject[$z].$Property[$index] -eq $corner) {
                    $replace = $InputObject[$z].$Property.ToCharArray()
                    $replace[$Index] = $connector
                    $InputObject[$z].$Property = -join $replace
                }
            }
        }
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