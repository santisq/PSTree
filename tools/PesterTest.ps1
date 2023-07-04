<#
.SYNOPSIS
Run Pester test

.PARAMETER TestPath
The path to the tests to run

.PARAMETER OutputFile
The path to write the Pester test results to.
#>
[CmdletBinding()]
param (
    [Parameter(Mandatory)]
    [String]
    $TestPath,

    [Parameter(Mandatory)]
    [String]
    $OutputFile
)

$ErrorActionPreference = 'Stop'
$requirements = Import-PowerShellDataFile ([IO.Path]::Combine($PSScriptRoot, 'requiredModules.psd1'))
foreach ($req in $requirements.GetEnumerator()) {
    $importModuleSplat = @{
        Name                = ([IO.Path]::Combine($PSScriptRoot, 'Modules', $req.Key))
        Force               = $true
        DisableNameChecking = $true
    }

    Write-Host "Importing: $($importModuleSplat['Name'])"
    Import-Module @importModuleSplat
}

[PSCustomObject] $PSVersionTable |
    Select-Object -Property *, @{
        Name       = 'Architecture'
        Expression = {
            switch ([IntPtr]::Size) {
                4 {
                    'x86'
                }
                8 {
                    'x64'
                }
                default {
                    'Unknown'
                }
            }
        }
    } |
    Format-List |
    Out-Host

$configuration = [PesterConfiguration]::Default
$configuration.Output.Verbosity = 'Detailed'
$configuration.Run.Exit = $true
$configuration.Run.Path = $TestPath
$configuration.TestResult.Enabled = $true
$configuration.TestResult.OutputPath = $OutputFile
$configuration.TestResult.OutputFormat = 'NUnitXml'

Invoke-Pester -Configuration $configuration -WarningAction Ignore
