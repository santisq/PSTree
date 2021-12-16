# Inspired from https://stackoverflow.com/a/40887001/15339544

function SizeConvert([double]$Size) {
    $suffix = "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"
    $index = 0
    while ($Size -ge 1kb) 
    {
        $Size /= 1kb
        $index++
    }
    [string]::Format(
        '{0:0.##} {1}',
        $Size, $suffix[$index]
    )
}
