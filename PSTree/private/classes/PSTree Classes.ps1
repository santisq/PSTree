using namespace System.IO
using namespace System.Collections.Generic
using namespace System.Linq
using namespace System.Management.Automation
using namespace System.Text

class PSTreeStatic {

    static [string] Indent ([string]$String, [object]$Indentation)
    {
        $i = ' ' * 4
        return [string]::Format(
            "{0}$String", ($i * $Indentation)
        )
    }

    static [string] SizeConvert ([decimal]$Length)
    {
        # Inspired from https://stackoverflow.com/a/40887001/15339544
        
        $suffix = "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"
        $index = 0
        while ($Length -ge 1kb)
        {
            $Length /= 1kb
            $index++
        }
        return [string]::Format(
            '{0:0.##} {1}',
            $Length, $suffix[$index]
        )
    }

    static [int64] GetTotalSize ([int64[]]$Length)
    {
        if(-not $Length.Count)
        {
            return 0
        }
        return [Enumerable]::Sum($Length)
    }
    
    static [object[]] DrawHierarchy ([object[]]$InputObject, [string]$Property, [string]$Rec)
    {
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
    
        foreach($group in $InputObject | Group-Object $Rec | Select-Object -Skip 1)
        {
            foreach($item in $group.Group)
            {
                $item.$Property = $item.$Property -replace '\s{4}(?=\S)', $cornerConnector
            }
        }
    
        for($i = 1; $i -lt $InputObject.Count; $i++)
        {
            $index = $InputObject[$i].$Property.IndexOf($corner)
            if($index -ge 0)
            {
                $z = $i - 1
                while($InputObject[$z].$Property[$index] -notmatch "$corner|\S")
                {
                    $replace = $InputObject[$z].$Property.ToCharArray()
                    $replace[$Index] = $pipe
                    $InputObject[$z].$Property = -join $replace
                    $z--
                }
            
                if($InputObject[$z].$Property[$index] -eq $corner)
                {
                    $replace = $InputObject[$z].$Property.ToCharArray()
                    $replace[$Index] = $connector
                    $InputObject[$z].$Property = -join $replace
                }
            }
        }

        return $InputObject
    }

    static [void] SetDefaultMembers ([object[]]$InputObject)
    {
        $DefaultProps = @(
            'Attributes'
            'Hierarchy'
            'Size'
        )

        [PSMemberInfo[]]$standardMembers = [PSPropertySet]::new(
            'DefaultDisplayPropertySet',
            [string[]]$DefaultProps
        )

        foreach($object in $InputObject)
        {
            $object.PSObject.Members.Add(
                [PSMemberSet]::new(
                    'PSStandardMembers',
                    $standardMembers
                )
            )
        }
    }
}

class PSTreeParent {
    [FileAttributes]$Attributes
    [string]$Hierarchy
    [string]$Size
    [int64]$RawSize
    [string]$Name
    [string]$FullName
    [DirectoryInfo]$Parent
    [datetime]$CreationTime
    [datetime]$LastAccessTime
    [datetime]$LastWriteTime
    hidden [int64]$Nesting

    PSTreeParent() { }

    PSTreeParent([DirectoryInfo]$Path)
    {        
        $this.Attributes     = $Path.Attributes
        $this.Name           = $Path.Name
        $this.FullName       = $Path.FullName
        $this.Parent         = $Path.Parent
        $this.CreationTime   = $Path.CreationTime
        $this.LastAccessTime = $Path.LastAccessTime
        $this.LastWriteTime  = $Path.LastWriteTime
        $this.SetHierarchy()
        [PSTreeStatic]::SetDefaultMembers($this)
    }

    [PSTreeDirectoryInfo[]] GetFolders ([bool]$Force)
    {
        $folders = [PSTreeDirectoryInfo]::GetFolders($this.FullName, $this.Nesting + 1, $Force)
        [PSTreeStatic]::SetDefaultMembers($folders)
        return $folders
    }

    [PSTreeFileInfo[]] GetFiles ([bool]$Force)
    {
        $files        = [PSTreeFileInfo]::GetFiles($this.FullName, $this.Nesting + 1, $Force)
        $this.RawSize = [PSTreeStatic]::GetTotalSize($files.RawSize)
        $this.Size    = [PSTreeStatic]::SizeConvert($this.RawSize)
        [PSTreeStatic]::SetDefaultMembers($files)
        return $files
    }

    [void] SetHierarchy ()
    {
        $this.Hierarchy = [PSTreeStatic]::Indent($this.Name, $this.Nesting)
    }
}

class PSTreeDirectoryInfo : PSTreeParent {

    PSTreeDirectoryInfo([DirectoryInfo]$DirectoryInfo)
    {
        $this.Attributes     = $DirectoryInfo.Attributes
        $this.Name           = $DirectoryInfo.Name
        $this.FullName       = $DirectoryInfo.FullName
        $this.Parent         = $DirectoryInfo.Parent
        $this.CreationTime   = $DirectoryInfo.CreationTime
        $this.LastAccessTime = $DirectoryInfo.LastAccessTime
        $this.LastWriteTime  = $DirectoryInfo.LastWriteTime
    }

    static [PSTreeDirectoryInfo[]]
    GetFolders ([string]$Path, [int64]$Nesting, [bool]$Force)
    {    
        $dirs = [PSTreeDirectoryInfo[]][DirectoryInfo[]][Directory]::GetDirectories($Path)
    
        foreach($dir in $dirs)
        {
            $dir.Hierarchy = [PSTreeStatic]::Indent($dir.Name, $Nesting)
            $dir.Nesting   = $Nesting
        }
        
        if(-not $Force)
        {
            return $dirs.Where({
                -not ($_.Attributes -band [FileAttributes]'Hidden, System')
            })
        }
        return $dirs
    }
}

class PSTreeFileInfo : PSTreeParent {
    
    PSTreeFileInfo([FileInfo]$FileInfo)
    {
        $this.Attributes     = $FileInfo.Attributes
        $this.Name           = $FileInfo.Name
        $this.FullName       = $FileInfo.FullName
        $this.RawSize        = $FileInfo.Length
        $this.Size           = [PSTreeStatic]::SizeConvert($FileInfo.Length)
        $this.CreationTime   = $FileInfo.CreationTime
        $this.LastAccessTime = $FileInfo.LastAccessTime
        $this.LastWriteTime  = $FileInfo.LastWriteTime
    }

    static [PSTreeFileInfo[]]
    GetFiles ([string]$Path, [int64]$Nesting, [bool]$Force)
    {
        $files = [PSTreeFileInfo[]][FileInfo[]][Directory]::GetFiles($Path)
    
        foreach($file in $files)
        {
            $file.Hierarchy = [PSTreeStatic]::Indent($file.Name, $Nesting)
            $file.Nesting   = $Nesting
        }
        
        if(-not $Force)
        {
            return $files.Where({
                -not ($_.Attributes -band [FileAttributes]'Hidden, System')
            })
        }
        return $files
    }
}
