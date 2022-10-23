Join-Path $PSScriptRoot -ChildPath 'private' |
    Get-ChildItem -Filter *.ps1 -Recurse |
        ForEach-Object { . $_.FullName }

Join-Path $PSScriptRoot -ChildPath 'public' |
    Get-ChildItem -Filter *.ps1 -Recurse |
        ForEach-Object { . $_.FullName }