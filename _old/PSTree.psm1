'classes', 'public' | ForEach-Object { Join-Path $PSScriptRoot -ChildPath $_ } |
    Get-ChildItem -Filter *.ps1 -Recurse | ForEach-Object { . $_.FullName }