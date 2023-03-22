try {
    Push-Location .\src\PSTree
    dotnet publish -c release -o ..\..\PSTree\ -f netstandard2.0
}
finally {
    Pop-Location
}