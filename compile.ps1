try {
    Push-Location .\src\PSTree
    dotnet publish -c release -f netstandard2.0
}
finally {
    Pop-Location
}