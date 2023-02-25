param([string] $ModuleName)

$path = Join-Path $PSScriptRoot -ChildPath $ModuleName
Publish-Module -Path $path -NuGetApiKey $env:PSGALLERY_TOKEN