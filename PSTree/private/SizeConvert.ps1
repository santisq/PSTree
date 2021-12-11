function SizeConvert([long]$Size) {
    switch($Size)
    {
        {$_ -lt 1Kb} {
            [string]::Format(
                '{0:0.#} Bytes',
                $_
            )
            break
        }
        {$_ -lt 1Mb} {
            [string]::Format(
                '{0:0.#} Kb',
                ($_ / 1Kb)
            )            
            break
        }
        {$_ -lt 1Gb} {
            [string]::Format(
                '{0:0.#} Mb',
                ($_ / 1Mb)
            )            
            break
        }
        Default {
            [string]::Format(
                '{0:0.#} Gb',
                ($_ / 1Gb)
            )            
        }
    }
}