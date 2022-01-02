$private = Join-Path $PSScriptRoot -ChildPath private
$public = Join-Path $PSScriptRoot -ChildPath public

Get-ChildItem -LiteralPath $private -Filter *.ps1 -Recurse |
ForEach-Object { . $_.FullName }

Get-ChildItem -LiteralPath $public -Filter *.ps1 -Recurse |
ForEach-Object { . $_.FullName }

Export-ModuleMember -Function Get-PSTree -Alias gpstree
