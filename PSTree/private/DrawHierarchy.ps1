function DrawHierarchy {
param(
    [System.Collections.ArrayList]$Array,
    [string]$PropertyName,
    [string]$RecursionProperty
)
function DrawHierarchy {
param(
    [System.Collections.ArrayList]$Array,
    [string]$PropertyName,
    [string]$RecursionProperty
)

    # Thanks https://github.com/ninmonkey for the Contribution :)
    $bar = [char]::ConvertFromUtf32(0x2500)
    $corner = [char]::ConvertFromUtf32(0x2514)
    $pipe = [char]::ConvertFromUtf32(0x2502)
    $connector = [char]::ConvertFromUtf32(0x251c)
    
    $cornerConnector = "${corner}$(${bar}*2) "

    $Array | Group-Object $RecursionProperty |
    Select-Object -Skip 1 | ForEach-Object {
        $_.Group | ForEach-Object {
            $_.$PropertyName = $_.$PropertyName -replace '\s{4}(?=\S)', $cornerConnector
        }
    }

    for($i = 1; $i -lt $Array.Count; $i++)
    {
        $index = $Array[$i].$PropertyName.IndexOf($corner)
        if($index -ge 0)
        {
            $z = $i - 1
            while($Array[$z].$PropertyName[$index] -notmatch "$corner|\S")
            {
                $replace = $Array[$z].$PropertyName.ToCharArray()
                $replace[$Index] = $pipe
                $Array[$z].$PropertyName = -join $replace
                $z--
            }
        
            if($Array[$z].$PropertyName[$index] -eq $corner)
            {
                $replace = $Array[$z].$PropertyName.ToCharArray()
                $replace[$Index] = $connector
                $Array[$z].$PropertyName = -join $replace
            }
        }
    }
    $Array
}
    $cornerConnector = "${corner}$(${horizontal}*2) "

    $Array | Group-Object $RecursionProperty |
    Select-Object -Skip 1 | ForEach-Object {
        $_.Group | ForEach-Object {
            $_.$PropertyName = $_.$PropertyName -replace '\s{4}(?=\S)', $cornerConnector
        }
    }

    for($i = 1; $i -lt $Array.Count; $i++)
    {
        $index = $Array[$i].$PropertyName.IndexOf($corner)
        if($index -ge 0)
        {
            $z = $i - 1
            while($Array[$z].$PropertyName[$index] -notmatch "$corner|\S")
            {
                $replace = $Array[$z].$PropertyName.ToCharArray()
                $replace[$Index] = $pipe
                $Array[$z].$PropertyName = -join $replace
                $z--
            }
        
            if($Array[$z].$PropertyName[$index] -eq $corner)
            {
                $replace = $Array[$z].$PropertyName.ToCharArray()
                $replace[$Index] = $connector
                $Array[$z].$PropertyName = -join $replace
            }
        }
    }
    
    $Array
}
