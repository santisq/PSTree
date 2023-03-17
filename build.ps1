Join-Path $PSScriptRoot -ChildPath src | Push-Location
dotnet publish --framework net472 -c release -o ..\PSTree\