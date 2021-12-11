function DrawHierarchy {
param(
    [System.Collections.ArrayList]$Array,
    [string]$PropertyName,
    [string]$RecursionProperty
)

    $Array | Group-Object $RecursionProperty |
    Select-Object -Skip 1 | ForEach-Object {
        $_.Group | ForEach-Object {
            $_.$PropertyName = $_.$PropertyName -replace '\s{4}(?=\S)','└── '
        }
    }

    for($i = 1; $i -lt $Array.Count; $i++)
    {
        $index = $Array[$i].$PropertyName.IndexOf('└')
        if($index -ge 0)
        {
            $z = $i - 1
            while($Array[$z].$PropertyName[$index] -notmatch '└|\S')
            {
                $replace = $Array[$z].$PropertyName.ToCharArray()
                $replace[$Index] = '│'
                $Array[$z].$PropertyName = -join $replace
                $z--
            }
        
            if($Array[$z].$PropertyName[$index] -eq '└')
            {
                $replace = $Array[$z].$PropertyName.ToCharArray()
                $replace[$Index] = '├'
                $Array[$z].$PropertyName = -join $replace
            }
        }
    }
    $Array
}