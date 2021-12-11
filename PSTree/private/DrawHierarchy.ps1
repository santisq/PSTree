function DrawHierarchy {
param(
    [System.Collections.ArrayList]$Array,
    [string]$PropertyName,
    [string]$RecursionProperty
)
    # Had to do this because of Windows PowerShell Default Encoding
    # Sorry for the ugliness :(
    $bytes = @(
        '226','148','148'
        '44','226','148'
        '128','44','226'
        '148','130','44'
        '226','148','156'
    )
    $corner, $horizontal, $pipe, $connector =
    [System.Text.Encoding]::UTF8.GetString($bytes).Split(',')
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
