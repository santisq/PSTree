[CmdletBinding()]
param (
    [Parameter(Mandatory)]
    [String] $TestPath,

    [Parameter(Mandatory)]
    [String] $OutputFile
)

$ErrorActionPreference = 'Stop'

Get-ChildItem ([IO.Path]::Combine($PSScriptRoot, 'Modules')) -Directory |
    Import-Module -Name { $_.FullName } -Force -DisableNameChecking

[PSCustomObject] $PSVersionTable | Select-Object *, @{
    Name       = 'Architecture'
    Expression = {
        switch ([IntPtr]::Size) {
            4 { 'x86' }
            8 { 'x64' }
            default { 'Unknown' }
        }
    }
} | Format-List | Out-Host

$configuration = [PesterConfiguration]::Default
$configuration.Output.Verbosity = 'Detailed'
$configuration.Run.Path = $TestPath
$configuration.Run.Throw = $true
$configuration.TestResult.Enabled = $true
$configuration.TestResult.OutputPath = $OutputFile
$configuration.TestResult.OutputFormat = 'NUnitXml'

Invoke-Pester -Configuration $configuration -WarningAction Ignore
