function Indent {
param(
    [String]$String,
    [Int]$Indent
)

    switch($Indent)
    {
        {-not $_} {
            return $String
        }
        Default {
            "$('    ' * $_)$String"
        }
    }
}